using System.Collections.Generic;
using Figurebox.core;
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

    
  }
}