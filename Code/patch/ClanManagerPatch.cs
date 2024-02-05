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

        // 尝试启动
        bool flag1 = tryPlotReclaimWar(pActor, AssetManager.plots_library.get("reclaim_war"));
        bool flag2 = tryPlotVassalWar(pActor, AssetManager.plots_library.get("vassal_war"));
        bool flagAbsorbVassal = tryPlotAbsorbVassal(pActor, AssetManager.plots_library.get("absorb_vassal"));
        bool flagIndependence = tryPlotIndependence(pActor, AssetManager.plots_library.get("Independence_War"));

        if (flagAbsorbVassal || flagIndependence || flag1 || flag2)
        {
            __instance._timestamp_last_plot = World.world.getCurWorldTime();
        }



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
        int targetPower = target.getArmy(); // 默认为目标国家自身的军事力量

        // 将 target 转换为 AW_Kingdom，以便访问 IsVassal 和 IsSuzerain 方法
        AW_Kingdom awTarget = MoHTools.ConvertKtoAW(target);

        // 检查目标国家是否是附庸或宗主国
        if (awTarget.IsVassal())
        {
            // 如果是附庸或宗主国，计算其整个联盟的军事力量
            targetPower = getSuzerainArmy(awTarget.suzerain);
        }
        else if (awTarget.IsSuzerain())
        {
            targetPower = getSuzerainArmy(awTarget);
        }

        // 计算发起者和目标国家军事力量的平均值
        int sum = (initiatorPower + targetPower) / 2;

        // 判断发起者的军事力量是否大于或等于这个平均值
        return initiatorPower >= sum;
    }

    public static int getSuzerainArmy(AW_Kingdom suzerain)
    {
        int armyCount = suzerain.getArmy();

        // Get all vassals of the suzerain
        List<AW_Kingdom> vassals = suzerain.GetVassals();

        // Add up the army count of each vassal
        foreach (AW_Kingdom vassal in vassals)
        {
            armyCount += vassal.getArmy();
        }

        return armyCount;
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
    public static bool tryPlotAbsorbVassal(Actor pActor, PlotAsset pPlotAsset)
    {
        // 基本的情节检查
        if (!basePlotChecks(pActor, pPlotAsset))
        {
            return false;
        }
        Main.LogInfo($"吸收自检目标过:{pActor.kingdom.name}");
        // 确定吞并的附庸目标
        Kingdom vassalTarget = GetVassalTargettoabsorb(pActor.kingdom);
        if (vassalTarget == null)
        {
            return false;
        }

        // 检查是否有足够的军事力量来进行吞并
        if (!HasEnoughMilitaryPower(pActor.kingdom, vassalTarget))
        {
            return false;
        }
        //Debug.Log("军事过");
        // 获取最新的附庸关系

        //Debug.Log("过");
        // 如果所有条件都满足，那么就创建一个新的情节，并设置目标国家
        Plot plot = World.world.plots.newPlot(pActor, pPlotAsset);
        plot.rememberInitiators(pActor);
        plot.target_kingdom = vassalTarget;

        if (!plot.checkInitiatorAndTargets())
        {
            // Debug.Log("tryPlotAbsorbVassal is missing start requirements");
            return false;
        }

        return true;
    }
    public static Kingdom GetVassalTargettoabsorb(Kingdom kingdom)
    {
        AW_Kingdom k = kingdom as AW_Kingdom;
        List<AW_Kingdom> vassals = k.GetVassals();

        foreach (AW_Kingdom vassal in vassals)
        {

            if (k.getArmy() < vassal.getArmy())
            {
                Main.LogInfo($"吸收目标军队过多:{vassal.name}");
                continue;
            }

            if (vassal.hasEnemies())
            {
                Main.LogInfo($"吸收目标有敌人:{vassal.name}");
                continue;
            }

            // 如果附庸满足所有条件，那么它就是我们的目标
            Main.LogInfo($"吸收目标过:{vassal.name}");
            return vassal;
        }

        // 如果没有找到满足条件的附庸，那么返回 null
        return null;
    }
    public static bool tryPlotIndependence(Actor pActor, PlotAsset pPlotAsset)
    {
        // 基本的情节检查
        if (!basePlotChecks(pActor, pPlotAsset))
        {
            return false;
        }



        // 确定战争的目标
        // 获取宗主国
        AW_Kingdom suzerain = MoHTools.ConvertKtoAW(pActor.kingdom).suzerain;
        if (suzerain == null)
        {
            return false;
        }




        // 如果所有条件都满足，那么就创建一个新的情节，并设置目标国家
        Plot plot = World.world.plots.newPlot(pActor, pPlotAsset);
        plot.rememberInitiators(pActor);
        plot.target_kingdom = suzerain;
        if (pActor.kingdom != null && pActor.kingdom.capital != null)
        {
            foreach (Kingdom neighbourKingdom in World.world.kingdoms.list_civs)
            {
                AW_Kingdom neighbour = neighbourKingdom as AW_Kingdom;
                if (neighbour != null)
                {


                    if (suzerain != null)
                    {
                        KingdomOpinion opinion;
                        opinion = BehaviourActionBase<Kingdom>.world.diplomacy.getOpinion(suzerain, neighbourKingdom);
                        int adjustedTotal = opinion.total;
                        if (neighbour.IsVassal() && neighbour.suzerain == suzerain)
                        {
                            adjustedTotal -= 1000;
                        }
                        if ((adjustedTotal <= 0) && neighbourKingdom.king != null && neighbourKingdom != suzerain && neighbourKingdom != pActor.kingdom)
                        {
                            plot.addSupporter(neighbourKingdom.king);
                        }
                    }
                }
            }
        }

        if (!plot.checkInitiatorAndTargets())
        {
            // Debug.Log("tryPlotindeWar is missing start requirements");
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