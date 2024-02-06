using Figurebox.core;
using Figurebox.Utils.MoH;
using HarmonyLib;
using Figurebox.attributes;
using System.Collections.Generic;
using System.Data.SQLite;
using Figurebox.Utils.extensions;

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
        /* Kingdom vassaltarget = GetVassalTarget(pActor.kingdom);
         if (vassaltarget != null && vassaltarget == warTarget)
         {
             return false;
         }*/
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
        bool flagIndependence = tryPlotIndependence(pActor, AssetManager.plots_library.get("Independence_War"));
        bool flagAbsorbVassal = tryPlotAbsorbVassal(pActor, AssetManager.plots_library.get("absorb_vassal"));


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
                int yearsSinceOccupation = awc.GetYearsSinceOccupation(kingdom.AW());
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
        AW_Kingdom awTarget = target.AW();

        // 检查目标国家是否是附庸或宗主国
        if (awTarget.IsVassal() && awTarget.suzerain != initiator)
        {
            // 如果是附庸或宗主国，计算其整个联盟的军事力量
            targetPower = getSuzerainArmy(awTarget.suzerain);
        }
        else if (awTarget.IsSuzerain() && awTarget != initiator.AW().suzerain)
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
        if (pActor.kingdom.cities.Count < 2)
        {
            return false;
        }

        // 检查是否有足够的军事力量来进行战争
        if (!HasEnoughMilitaryPower(pActor.kingdom, vassalTarget))
        {
            return false;
        }
        if (vassalTarget.AW().CompareTitle(pActor.kingdom.AW()))
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
        // 确定吞并的附庸目标
        Kingdom vassalTarget = GetVassalTargettoabsorb(pActor.kingdom);
        if (vassalTarget == null)
        {
            return false;
        }
        if (GetYearsSinceVassalageStarted(pActor.kingdom.data.id, vassalTarget.data.id) <= 10)//暂时设置为10
        {
            return false;
        }
        double absorbTimestamp = pActor.kingdom.AW().absorb_timestamp;
        if (absorbTimestamp > 0 && World.world.getYearsSince(absorbTimestamp) <= 10)
        {
            return false;
        }
        Main.LogInfo($"吞并 目标{vassalTarget.name}");
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

                continue;
            }

            if (vassal.hasEnemies())
            {

                continue;
            }

            // 如果附庸满足所有条件，那么它就是我们的目标

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
        AW_Kingdom suzerain = pActor.kingdom.AW().suzerain;
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
    public static int GetYearsSinceVassalageStarted(string suzerainKingdomId, string vassalId)
    {
        // 初始化变量来存储开始时间
        double startTime = -1;

        // 查询数据库获取特定宗主国与特定附庸关系的开始时间
        using (var cmd = new SQLiteCommand(EventsManager.Instance.OperatingDB))
        {
            // 在SQL查询中同时考虑宗主国ID和附庸ID
            cmd.CommandText = "SELECT START_TIME FROM VASSAL WHERE SKID = @SKID AND KID = @KID AND END_TIME = -1 ORDER BY START_TIME DESC LIMIT 1";
            cmd.Parameters.AddWithValue("@SKID", suzerainKingdomId);
            cmd.Parameters.AddWithValue("@KID", vassalId);

            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    // 获取查询到的开始时间
                    int startTimeIndex = reader.GetOrdinal("START_TIME");
                    if (!reader.IsDBNull(startTimeIndex))
                    {
                        startTime = reader.GetDouble(startTimeIndex);
                    }
                }
            }
        }

        // 如果找到了开始时间，计算并返回自那时起经过的年数
        if (startTime != -1)
        {
            int yearsSince = World.world.mapStats.getYearsSince(startTime);
            Main.LogInfo($"查到了，自附庸关系开始以来经过了 {yearsSince} 年{vassalId}");
            return yearsSince;
        }

        // 如果没有找到相关记录，返回-1表示没有找到或没有经过任何年数
        Main.LogInfo("未查到宗主国与附庸之间的关系开始时间");
        return -1;
    }

}