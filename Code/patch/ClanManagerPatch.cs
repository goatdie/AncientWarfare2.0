using Figurebox.core;
using Figurebox.Utils.MoH;
using HarmonyLib;
using Figurebox.ai;

namespace Figurebox.patch;

internal static class ClanManagerPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(ClanManager), "tryPlotWar")]
    public static bool reclaimnam_Prefix(Actor pActor, PlotAsset pPlotAsset)
    {
        Kingdom reclaimTarget = GetReclaimTarget(pActor.kingdom);
        if (reclaimTarget != null)
        {
            return false; // 不执行原方法，因为有收复目标
        }

        return true; // 否则执行原方法
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
                bool isCurrentlyOwned = awc.kingdom == kingdom; // 假设有一个currentOwner属性

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

}