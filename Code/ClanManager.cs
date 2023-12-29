using System.Collections.Generic;
using Figurebox.Utils;
using HarmonyLib;
using UnityEngine;


namespace Figurebox
{
  public class ClansManager
  {
    public static Dictionary<string, int> LastReclaimWarYears = new Dictionary<string, int>();
    public static Dictionary<string, int> LastUsurpationYears = new Dictionary<string, int>();

    public static bool HasEnoughTimePassedSinceLastUsurpation(Kingdom kingdom)
    {
      int currentYear = World.world.mapStats.getCurrentYear();
      string kingdomId = kingdom.data.id;

      // 如果这个王国从未发生过篡位事件，那么可以发生篡位
      if (!LastUsurpationYears.ContainsKey(kingdomId))
      {
        return true;
      }

      int lastUsurpationYear = LastUsurpationYears[kingdomId];
      return currentYear - lastUsurpationYear >= 10; // 假设篡位事件的冷却期为10年
    }

    /*[HarmonyPostfix]
    [HarmonyPatch(typeof(ClanManager), "checkClanMembers")]
    public static void checkClanMembers_post(ref ClanManager __instance)
    {
        // Try new Reclaim War plot
                   for (int index = 0; index < __instance.list.Count; ++index)
        {
        foreach (var pActor in __instance.list[index].units.Values)
        {
            if (!pActor.isFighting())
                tryPlotReclaimWar(pActor, MorePlots.reclaim_war);
        }
        }
    }*/
    [HarmonyPostfix]
    [HarmonyPatch(typeof(ClanManager), "checkActionKing")]
    public static void checkKingAction_post(Actor pActor, ref ClanManager __instance)
    {
      // 如果该角色正在战斗或者不是国王，就返回
      if (pActor.isFighting())
        return;

      // 尝试启动夺回失地的情节
      bool flag = tryPlotReclaimWar(pActor, AssetManager.plots_library.get("reclaim_war"));


      if (flag)
      {
        // 如果成功启动了夺回失地，更新时间戳
        Traverse.Create(__instance).Field("_timestamp_last_plot").SetValue(World.world.getCurWorldTime());
      }
    }


    public static Kingdom GetReclaimTarget(Kingdom kingdom)
    {
      foreach (var otherKingdom in World.world.kingdoms.list_civs)
      {
        if (otherKingdom == kingdom)
        {
          continue;
        }
        foreach (var city in otherKingdom.cities)
        {
          if (CityTools.WasOccupiedByKingdomInLast5To100Years(city, kingdom))
          {
            return otherKingdom;
          }
        }
      }
      return null;
    }
    public static bool HasEnoughTimePassedSinceLastReclaimWar(Kingdom kingdom)
    {
      int currentYear = World.world.mapStats.getCurrentYear();
      string kingdomId = kingdom.data.id;

      // 如果这个王国从未发起过收复战争，那么可以发起
      if (!LastReclaimWarYears.ContainsKey(kingdomId))
      {
        return true;
      }

      int lastReclaimWarYear = LastReclaimWarYears[kingdomId];
      return currentYear - lastReclaimWarYear >= 10;
    }


    public static bool HasEnoughMilitaryPower(Kingdom initiator, Kingdom target)
    {
      int initiatorPower = initiator.getArmy();

      int targetPower;
      // If target is a vassal, calculate combined power of the suzerain and its vassals
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
      Kingdom vassalTarget = KingdomVassals.GetVassalTargettoabsorb(pActor.kingdom); // 注意这个函数可能需要根据你的需要进行修改
      if (vassalTarget != null)
      {
        return false;
      }
      if (!basePlotChecks(pActor, pPlotAsset))
      {
        return false;
      }
      if (!HasEnoughTimePassedSinceLastReclaimWar(pActor.kingdom)) { return false; }
      // Debug.Log("尝试启动夺回失地的情节");
      Kingdom reclaimTarget = GetReclaimTarget(pActor.kingdom);
      if (reclaimTarget == null)
      { //Debug.Log("reclaimTarget null"+pActor.kingdom.data.name);
        return false;
      }
      // Debug.Log(pActor.kingdom.data.name+"存在收复目标");
      if (!HasEnoughMilitaryPower(pActor.kingdom, reclaimTarget))
      {
        return false;
      }
      Debug.Log(pActor.kingdom.data.name + "的reclaimTarget 是" + reclaimTarget.data.name);
      Plot plot = World.world.plots.newPlot(pActor, pPlotAsset);
      plot.rememberInitiators(pActor);
      plot.target_kingdom = reclaimTarget;

      if (!plot.checkInitiatorAndTargets())
      {
        Debug.Log("tryPlotReclaimWar is missing start requirements");
        return false;
      }

      return true;
    }
    [HarmonyPrefix]
    [HarmonyPatch(typeof(ClanManager), "tryPlotWar")]
    static bool Prefix(Actor pActor, PlotAsset pPlotAsset)
    {
      // 如果有土地需要收复
      Kingdom reclaimTarget = GetReclaimTarget(pActor.kingdom);
      // Kingdom vassaltarget = KingdomVassals.GetVassalTarget(pActor.kingdom);
      if (reclaimTarget != null)
      {
        // 不执行原方法
        return false;
      }

      // 否则执行原方法
      return true;
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


  }
}