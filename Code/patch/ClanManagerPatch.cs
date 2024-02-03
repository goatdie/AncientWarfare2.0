using Figurebox.core;
using Figurebox.Utils.MoH;
using HarmonyLib;
using Figurebox.attributes;
using System.Collections.Generic;
namespace Figurebox.patch;

internal class ClanManagerPatch
{

    [MethodReplace(nameof(ClanManager.tryPlotWar))]
    private new bool tryPlotWar(Actor pActor, PlotAsset pPlotAsset)
    {
        if (!World.world.clans.basePlotChecks(pActor, pPlotAsset))
        {
            return false;
        }
        Kingdom warTarget = DiplomacyHelpers.getWarTarget(pActor.kingdom);
        if (warTarget == null)
        {
            return false;
        }
        Kingdom reclaimTarget = GetReclaimTarget(pActor.kingdom);
        if (reclaimTarget != null && reclaimTarget == warTarget)
        {
            return false; // 不执行原方法，因为有收复目标
        }
        Kingdom vassaltarget = GetVassalTarget(pActor.kingdom);
        if (vassaltarget != null && vassaltarget == warTarget)
        {
            return false;
        }
        Plot plot = World.world.plots.newPlot(pActor, pPlotAsset);
        plot.rememberInitiators(pActor);
        plot.target_kingdom = warTarget;
        if (!plot.checkInitiatorAndTargets())
        {
            Main.LogInfo("tryPlotWar  is missing start requirements");
            return true;
        }
        return true;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ClanManager), "checkActionKing")]
    public static void checkKingAction_post(Actor pActor, ref ClanManager __instance)
    {
        // 如果该角色正在战斗或者不是国王，就返回
        if (pActor.isFighting())
            return;

        // 尝试启动夺回失地的情节
        tryPlotReclaimWar(pActor, AssetManager.plots_library.get("reclaim_war"));
        tryPlotVassalWar(pActor, AssetManager.plots_library.get("vassal_war"));


    }
    [HarmonyPrefix]
    [HarmonyPatch(typeof(ClanManager), "tryPlotJoinAlliance")]
    public static bool Prefix_tryPlotJoinAlliance(Actor pActor, PlotAsset pPlotAsset)
    {
        AW_Kingdom kingdom = pActor.kingdom as AW_Kingdom;

        return kingdom.suzerain == kingdom;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(ClanManager), "tryPlotDissolveAlliance")]
    public static bool Prefix_tryPlotDissolveAlliance(Actor pActor, PlotAsset pPlotAsset)
    {
        AW_Kingdom kingdom = pActor.kingdom as AW_Kingdom;

        return kingdom.suzerain == kingdom;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(ClanManager), "tryPlotNewAlliance")]
    public static bool Prefix_tryPlotNewAlliance(Actor pActor, PlotAsset pPlotAsset)
    {
        AW_Kingdom kingdom = pActor.kingdom as AW_Kingdom;

        return kingdom.suzerain == kingdom;
    }



    // Assuming a basePlotChecks method that validates common conditions
    private static bool basePlotChecks(Actor pActor, PlotAsset pPlotAsset)
    {
        if (pActor == null || pPlotAsset == null) { return false; }

        if (!World.world.worldLaws.world_law_rebellions.boolVal || !(pActor.getInfluence() >= pPlotAsset.cost && pPlotAsset.checkInitiatorPossible(pActor) && pPlotAsset.check_launch(pActor, pActor.kingdom)))
        {
            return false;
        }

        return true;
    }
    public static Kingdom GetReclaimTarget(Kingdom kingdom)
    {
        //  Main.LogInfo($"开始寻找 {kingdom.data.name} 的收复战争目标...");

        foreach (var otherKingdom in World.world.kingdoms.list_civs)
        {
            // 跳过自己的王国
            if (otherKingdom == kingdom)
            {
                continue;
            }

            foreach (var city in otherKingdom.cities)
            {
                AW_City awc = city as AW_City;
                int yearsSinceOccupation = awc.GetYearsSinceOccupation(MoHTools.ConvertKtoAW(kingdom));
                bool isCurrentlyOwned = awc.kingdom == kingdom;

                //Main.LogInfo($"城市 {awc.name} 脱离 {kingdom.data.name} 占领的年数：{yearsSinceOccupation}. 当前所有者是 {awc.kingdom?.data.name ?? "无"}.");

                if (yearsSinceOccupation <= 100 && yearsSinceOccupation != -1 && !isCurrentlyOwned)
                {
                    Main.LogInfo($"找到有效目标：{otherKingdom.data.name} 城市 {awc.name} 脱离 {kingdom.data.name} 占领的年数：{yearsSinceOccupation}.");
                    return otherKingdom;
                }
            }
        }

        //  Main.LogInfo("没有找到有效的收复战争目标。");
        return null;
    }
    public static bool HasEnoughMilitaryPower(Kingdom initiator, Kingdom target)
    {
        int initiatorPower = initiator.getArmy();

        int targetPower;
        if (KingdomVassals.IsVassal(target))
        {
            Kingdom suzerain = KingdomVassals.GetSuzerain(target);
            targetPower = KingdomVassals.getSuzerainArmy(suzerain);
        }
        else
        {
            targetPower = target.getArmy();
        }

        int sum = (initiatorPower + targetPower) / 2;

        return initiatorPower >= sum;
    }
    public static bool tryPlotReclaimWar(Actor pActor, PlotAsset pPlotAsset)
    {
        if (!World.world.clans.basePlotChecks(pActor, pPlotAsset))
        {
            return false;
        }

        Kingdom reclaimTarget = GetReclaimTarget(pActor.kingdom);
        if (reclaimTarget == null || !HasEnoughMilitaryPower(pActor.kingdom, reclaimTarget))
        {
            return false;
        }

        //Debug.Log(pActor.kingdom.data.name + "的reclaimTarget 是" + reclaimTarget.data.name);
        Plot plot = World.world.plots.newPlot(pActor, pPlotAsset);
        plot.rememberInitiators(pActor);
        plot.target_kingdom = reclaimTarget;

        if (!plot.checkInitiatorAndTargets())
        {
            //     Debug.Log("tryPlotReclaimWar is missing start requirements");
            return false;
        }

        return true;
    }
    public static bool tryPlotVassalWar(Actor pActor, PlotAsset pPlotAsset)
    {
        // 基本的情节检查
        if (!basePlotChecks(pActor, pPlotAsset))
        {
            return false;
        }

        // 检查自上次附庸战争以来是否已经过了足够的时间


        // 确定附庸战争的目标
        Kingdom vassalTarget = GetVassalTarget(pActor.kingdom);
        if (vassalTarget == null)
        {
            return false;
        }

        // 检查目标国家的城市数量是否大于或等于2
        if (vassalTarget.cities.Count < 2 || pActor.kingdom.cities.Count < 2)
        {
            return false;
        }

        // 检查是否有足够的军事力量来进行战争
        if (!HasEnoughMilitaryPower(pActor.kingdom, vassalTarget))
        {
            return false;
        }
        if (MoHTools.ConvertKtoAW(vassalTarget).CompareTitle(MoHTools.ConvertKtoAW(pActor.kingdom)))
        {
            return false;
        }

        // 如果所有条件都满足，那么就创建一个新的情节，并设置目标国家
        Plot plot = World.world.plots.newPlot(pActor, pPlotAsset);
        plot.rememberInitiators(pActor);
        plot.target_kingdom = vassalTarget;
        Main.LogInfo($"获得附庸目标 ：{vassalTarget.name}");
        if (!plot.checkInitiatorAndTargets())
        {

            return false;
        }

        return true;
    }
    public static HashSet<Kingdom> GetNearbyKingdoms(Kingdom kingdom)
    {
        HashSet<Kingdom> nearbyKingdoms = new HashSet<Kingdom>();

        foreach (City city in kingdom.cities)
        {
            foreach (Kingdom neighbour in city.neighbours_kingdoms)
            {
                nearbyKingdoms.Add(neighbour);
            }
        }
        return nearbyKingdoms;
    }


    public static Kingdom GetVassalTarget(Kingdom kingdom)
    {
        // First get the nearby kingdoms
        HashSet<Kingdom> nearbyKingdoms = GetNearbyKingdoms(kingdom);

        foreach (var otherKingdom in nearbyKingdoms)
        {
            if (otherKingdom == null || kingdom == null)
            {
                continue;
            }
            if (otherKingdom == kingdom)
            {
                continue;
            }

            // Check whether kingdom has enough military power to maintain a vassal relationship
            if (!HasEnoughMilitaryPower(kingdom, otherKingdom))
            {
                continue;
            }

            // Check the relationship between kingdom and otherKingdom
            // Here you can add more conditions, such as whether they have common enemies or allies
            // ...

            // If all conditions are satisfied, then otherKingdom is a possible vassal target

            return otherKingdom;
        }

        // If no suitable vassal target is found, return null
        return null;
    }
}