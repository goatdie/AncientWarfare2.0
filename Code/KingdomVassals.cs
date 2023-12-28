using System;
using System.Collections.Generic;
using System.Linq;
using Figurebox.Utils;
using HarmonyLib;
using UnityEngine;
namespace Figurebox //注意到的问题 附庸关系缺少结束年份 导致历史上出现bug 服用关系是一次性的这样不行
{
  public class KingdomVassals : MonoBehaviour
  {
    public static void init()
    {
    }

    // 设置Kingdom对象的id
    public static bool IsVassal(Kingdom kingdom)
    {
      if (kingdom == null || kingdom.data == null)
      {
        return false;
      }

      string suzerainId;
      kingdom.data.get("vassels", out suzerainId);
      return suzerainId != null && suzerainId != "";
    }




    public static void SetKingdom(Kingdom kingdom, Kingdom lord)
    {
      if (IsVassal(lord))
      {
        // 如果是，那么它不能成为宗主国，所以我们直接返回
        return;
      }
      if (IsLord(kingdom))
      {
        // If it is, it can't become a vassal, so we simply return
        return;
      }

      string kingdomId = lord.data.id;
      kingdom.data.set("originalColorID", kingdom.data.colorID);
      ColorAsset originalColor = kingdom.getColor();
      string serializedOriginalColor = Serialize(originalColor);

      // 将序列化后的字符串存储在kingdom的data字典中
      kingdom.data.set("originalColor", serializedOriginalColor);

      kingdom.data.set("vassels", kingdomId);
      kingdom.data.colorID = lord.data.colorID;
      ColorAsset lordcolor = lord.getColor();
      kingdom.updateColor(lordcolor);
      World.world.zoneCalculator.setDrawnZonesDirty();
      World.world.zoneCalculator.clearCurrentDrawnZones(true);
      World.world.zoneCalculator.redrawZones();

      int yeardata = World.world.mapStats.getCurrentYear();

      string baseKey = lord.data.id + "-" + kingdom.data.id;

      // 查找最新的附庸关系
      int counter = 1;
      while (FunctionHelper.kingdomVassalEstablishmentTime.ContainsKey(baseKey + "-" + counter))
      {
        counter++;
      }

      string key = baseKey + "-" + counter;
      FunctionHelper.kingdomVassalEstablishmentTime[key] = yeardata;
      if (World.world.wars.isInWarWith(kingdom, lord))
      {
        // 如果存在，结束它
        War ongoingWar = World.world.wars.getWar(kingdom, lord, false);
        if (ongoingWar != null)
        {
          World.world.wars.endWar(ongoingWar);
        }
      }
    }

    public static void RemoveVassalAndBanner(Kingdom pKingdom, Kingdom vassal, int yeardata, bool isVassal)
    {
      // 如果 pKingdom 为空，我们假设 vassal 是一个附庸国，因此需要找到它的宗主
      if (pKingdom == null && isVassal)
      {
        pKingdom = GetSuzerain(vassal);
      }

      // 无论是否为附庸国，我们都将宗主国的ID放在前面
      string baseKey = pKingdom.data.id + "-" + vassal.data.id;

      // 查找最新的附庸关系
      int counter = 1;
      while (FunctionHelper.kingdomVassalEndTime.ContainsKey(baseKey + "-" + counter))
      {
        counter++;
      }

      string key = baseKey + "-" + counter;
      FunctionHelper.kingdomVassalEndTime[key] = yeardata;

      // 如果是vassal，清除附庸的数据
      if (isVassal)
      {
        vassal.data.set("vassels", "");

        // 更新颜色
        UpdateColor(vassal);
      }

      // 清除旗帜
      List<string> relatedKeys = bannerLoaders.Keys.Where(key => key.StartsWith(pKingdom.data.id + "-") || key.EndsWith("-" + pKingdom.data.id)).ToList();

      foreach (string relatedKey in relatedKeys)
      {
        Debug.Log("Destroying banner for kingdom " + relatedKey);
        BannerLoader banner_suzerain = bannerLoaders[relatedKey];
        Destroy(banner_suzerain.gameObject);
        bannerLoaders.Remove(relatedKey);
      }
    }


    // 获取Kingdom对象
    public static Kingdom GetSuzerain(Kingdom kingdom)
    {
      string kingdomId;
      kingdom.data.get("vassels", out kingdomId);
      if (String.IsNullOrEmpty(kingdomId)) { kingdomId = kingdom.data.id; }
      Kingdom suzerain = World.world.kingdoms.getKingdomByID(kingdomId);

      return suzerain;
    }
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Kingdom), "update")]
    public static void VassalsPostfix_update(Kingdom __instance, float pElapsed)
    {
      // 如果是附庸国
      if (IsVassal(__instance))
      {

        Kingdom suzerain = GetSuzerain(__instance);
        if (suzerain == null)
        {
          return;
        }
        // 如果附庸国已经在联盟中，它应退出这个联盟
        Alliance vassalAlliance = __instance.getAlliance();
        Alliance suzerainAlliance = suzerain.getAlliance();
        if (vassalAlliance != null && vassalAlliance != suzerainAlliance)
        {
          //__instance.allianceLeave(vassalAlliance);
          vassalAlliance.leave(__instance, true);
          vassalAlliance.recalculate();

        }

        // 如果宗主国有联盟

        if (suzerainAlliance != null)
        {
          suzerainAlliance.recalculate();
          // 如果附庸国不在该联盟中，则加入
          // if (vassalAlliance==null||vassalAlliance != suzerainAlliance)  {


          suzerainAlliance.join(__instance, true);
          __instance.allianceJoin(suzerainAlliance);
          suzerainAlliance.kingdoms_hashset.Add(__instance);
          suzerainAlliance.data.timestamp_member_joined = World.world.getCurWorldTime();
          suzerainAlliance.recalculate();
          // }
        }

        // 获取宗主国的战争列表
        ListPool<War> suzerainWars = suzerain.getWars();
        foreach (War war in suzerainWars)
        {
          // 如果战争处于活动状态
          if (war.isActive())
          {
            // 如果宗主国是攻击方，附庸国加入攻击方
            if (war.isAttacker(suzerain))
            {
              war.joinAttackers(__instance);
            }
            // 如果宗主国是防守方，附庸国加入防守方
            else if (war.isDefender(suzerain))
            {
              war.joinDefenders(__instance);
            }
          }

        }
        ListPool<War> vassalWars = __instance.getWars();
        foreach (War war in vassalWars)
        {
          // 如果战争处于活动状态，并且宗主国还没有参与这场战争
          if (war.isActive() && !war.isAttacker(suzerain) && !war.isDefender(suzerain))
          {
            // 如果附庸国是攻击方，宗主国加入攻击方
            if (war.isAttacker(__instance))
            {
              war.joinAttackers(suzerain);
            }
            // 如果附庸国是防守方，宗主国加入防守方
            else if (war.isDefender(__instance))
            {
              war.joinDefenders(suzerain);
            }
          }
        }


      }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(ZoneCalculator), "isBorderColor_kingdoms")]
    public static bool NewIsBorderColorKingdoms(ZoneCalculator __instance, TileZone pZone, City pCity, ref bool __result, bool pCheckFriendly = false)
    {
      // 这是你的新版方法的代码：
      if (pCheckFriendly && pZone != null && pZone.city != pCity && pZone.city != null && __instance.checkKingdom && pZone.city.kingdom == pCity.kingdom)
      {
        __result = false;
        return false;
      }

      if (__instance.checkKingdom)
      {
        Kingdom suzerain = GetSuzerain(pCity.kingdom);
        List<Kingdom> vassals = GetVassals(suzerain);
        if (suzerain != null && pZone != null && pZone.city != null && (pZone.city.kingdom == suzerain || vassals.Contains(pZone.city.kingdom)))
        {
          __result = false;
          return false;
        }
        __result = pZone == null || pZone.city == null || pZone.city.kingdom != pCity.kingdom;
        return false;
      }

      __result = pZone == null || pZone.city != pCity;
      return false;
    }
    public static List<Kingdom> GetVassals(Kingdom kingdom)
    {
      List<Kingdom> vassals = new List<Kingdom>();

      //遍历所有国家
      foreach (var otherKingdom in World.world.kingdoms.list_civs)
      {
        // 检查是否是自己
        if (otherKingdom == kingdom)
          continue;

        // 使用你的 GetKingdom 方法来获取其他国家的宗主国
        Kingdom suzerain = GetSuzerain(otherKingdom);

        // 如果宗主国是 kingdom，那么 otherKingdom 就是 kingdom 的一个附庸国
        if (suzerain == kingdom)
        {
          vassals.Add(otherKingdom);

        }
      }

      return vassals;
    }
    public static bool IsLord(Kingdom kingdom)
    {
      // Get the list of vassals for the kingdom
      List<Kingdom> vassals = GetVassals(kingdom);

      // If the list of vassals is not empty, then the kingdom is a lord
      return vassals.Count > 0;
    }
    public static int getSuzerainArmy(Kingdom suzerain)
    {
      int armyCount = suzerain.getArmy();

      // Get all vassals of the suzerain
      List<Kingdom> vassals = GetVassals(suzerain);

      // Add up the army count of each vassal
      foreach (Kingdom vassal in vassals)
      {
        armyCount += vassal.getArmy();
      }

      return armyCount;
    }


    [HarmonyPrefix]
    [HarmonyPatch(typeof(ClanManager), "tryPlotJoinAlliance")]
    public static bool Prefix_tryPlotJoinAlliance(Actor pActor, PlotAsset pPlotAsset)
    {
      Kingdom kingdom = pActor.kingdom;
      Kingdom suzerain = GetSuzerain(kingdom);
      return suzerain == kingdom;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(ClanManager), "tryPlotDissolveAlliance")]
    public static bool Prefix_tryPlotDissolveAlliance(Actor pActor, PlotAsset pPlotAsset)
    {
      Kingdom kingdom = pActor.kingdom;
      Kingdom suzerain = GetSuzerain(kingdom);
      return suzerain == kingdom;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(ClanManager), "tryPlotNewAlliance")]
    public static bool Prefix_tryPlotNewAlliance(Actor pActor, PlotAsset pPlotAsset)
    {
      Kingdom kingdom = pActor.kingdom;
      Kingdom suzerain = GetSuzerain(kingdom);
      return suzerain == kingdom;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(ClanManager), "tryPlotWar")]
    public static bool Prefix_tryPlotWar(Actor pActor, PlotAsset pPlotAsset)
    {
      Kingdom kingdom = pActor.kingdom;
      Kingdom suzerain = GetSuzerain(kingdom);
      return suzerain == kingdom;
    }
    // 储存防御者战争开始时的所有城市
    public static Dictionary<War, City> storedDefenderCapital = new Dictionary<War, City>();

    [HarmonyPostfix]
    [HarmonyPatch(typeof(WarManager), "update")]
    public static void VassalWarPostfix(float pElapsed, WarManager __instance)
    {
      List<War> listCopy = new List<War>(__instance.list);
      foreach (War war in listCopy)
      {
        if (war._asset == AssetManager.war_types_library.get("vassal_war"))
        {
          // 储存开始战争时防御者的首都
          if (!storedDefenderCapital.ContainsKey(war))
          {
            storedDefenderCapital.Add(war, war.main_defender.capital);
          }

          if (war.isActive())
          {
            // 我们假设war.main_attacker是战胜国，war.main_defender是战败国
            Kingdom winner = war.main_attacker;
            Kingdom loser = war.main_defender;

            // 检查防御方首都是否被占领
            if (storedDefenderCapital[war] != null && storedDefenderCapital[war].kingdom != null && storedDefenderCapital[war].kingdom != loser)
            {
              // 将除首都外且忠诚度低于150的所有城市转交给战胜国
              List<City> loserCitiesCopy = new List<City>(loser.cities);

              foreach (var city in loserCitiesCopy)
              {
                if (city != storedDefenderCapital[war] && city.getLoyalty() < 100)
                {
                  city.joinAnotherKingdom(winner);
                }
              }

              // 将首都转交给附庸国
              if (storedDefenderCapital[war] == null)
              {
                Debug.LogError("storedDefenderCapital[war] is null");
              }
              else if (loser == null)
              {
                Debug.LogError("loser is null");
              }
              else
              {
                if (storedDefenderCapital[war] != null && loser != null)
                {
                  storedDefenderCapital[war].joinAnotherKingdom(loser);
                }
                else
                {
                  Debug.LogWarning("Either storedDefenderCapital[war] or loser is null");
                  return;
                }

              }

              // 设置战败国为战胜国的附庸

              CityTools.LogVassalWarEnd(loser, winner);

              // 结束战争
              ListPool<War> loserWars = loser.getWars();
              foreach (var warToBeEnded in loserWars)
              {
                __instance.endWar(warToBeEnded, true);
              }
              SetKingdom(loser, winner);

              // 从存储的首都列表中移除这场战争的记录
              storedDefenderCapital.Remove(war);
            }
          }
        }
      }
    }
    public static Dictionary<War, City> storedAttackerCapital = new Dictionary<War, City>();

    [HarmonyPostfix]
    [HarmonyPatch(typeof(WarManager), "update")]
    public static void independenceWarPostfix(float pElapsed, WarManager __instance)
    {
      List<War> listCopy = new List<War>(__instance.list);
      foreach (War war in listCopy)
      {
        if (war._asset == AssetManager.war_types_library.get("independence_war"))
        {
          // 储存开始战争时防御者的首都
          if (!storedDefenderCapital.ContainsKey(war))
          {
            storedDefenderCapital.Add(war, war.main_defender.capital);
          }

          // 储存开始战争时攻击者的首都
          if (!storedAttackerCapital.ContainsKey(war))
          {
            storedAttackerCapital.Add(war, war.main_attacker.capital);
          }

          if (war.isActive())
          {
            // 我们假设war.main_attacker是战胜国，war.main_defender是战败国
            Kingdom winner = war.main_attacker;
            Kingdom loser = war.main_defender;

            // 检查防御方首都是否被占领
            if (storedDefenderCapital[war].kingdom != loser)
            {
              // 创建一个集合来储存邻居城市
              // 获取战胜国首都的所有相邻城市
              if (winner.capital != null)
              {
                HashSet<City> neighbourCities = winner.capital.neighbours_cities;

                // 遍历邻居城市集合，如果城市属于战败国，就让它加入战胜国
                foreach (var neighbourCity in neighbourCities)
                {
                  if (neighbourCity.kingdom == loser)
                  {
                    neighbourCity.joinAnotherKingdom(winner);
                  }
                }
              }




              CityTools.LogIndependenceWarMessage(loser, winner);

              // 结束战争

              __instance.endWar(war, true);


              // 从存储的首都列表中移除这场战争的记录
              storedDefenderCapital.Remove(war);
              storedAttackerCapital.Remove(war);
            }

            // 如果攻击者首都被占领，继续作为防御者的附庸
            else if (storedAttackerCapital[war].kingdom != winner)
            {
              if (storedAttackerCapital[war] == null)
              {
                Debug.LogWarning("storedAttackerCapital[war] is null");
              }
              else if (winner == null)
              {
                Debug.LogWarning("winner is null");
              }
              else
              {
                if (storedAttackerCapital[war] != null && winner != null)
                {
                  storedAttackerCapital[war].joinAnotherKingdom(winner);
                  __instance.endWar(war, true);
                }
                else
                {
                  Debug.LogWarning("Either storedAttackerCapital[war] or winner is null");
                  return;
                }

              }

              // 设置战胜国为战败国的附庸
              SetKingdom(winner, loser);

            }

          }
        }
      }
    }







    /*[HarmonyPostfix]
    [HarmonyPatch(typeof(KingdomManager), "makeNewCivKingdom")]
    public static void initiate_Post(KingdomManager  __instance,Kingdom __result)
    {

      foreach (Kingdom kingdom in World.world.kingdoms.list_civs)
        {
            if(kingdom!=__result)
            {
SetKingdom(__result,kingdom);
            }
        }
    }*/

    // 使用Dictionary存储BannerLoader
    public static Dictionary<string, BannerLoader> bannerLoaders = new Dictionary<string, BannerLoader>();

    [HarmonyPostfix]
    [HarmonyPatch(typeof(MapText), "showTextKingdom")]
    public static void PostfixShowTextKingdom(MapText __instance, Kingdom pKingdom)
    {
      string suzerainId;
      pKingdom.data.get("vassels", out suzerainId);

      // 使用 pKingdom.data.id 作为 key
      string key = pKingdom.data.id + "-" + suzerainId;
      if (suzerainId != null && suzerainId != pKingdom.data.id && IsVassal(pKingdom))
      {
        Kingdom suzerain = World.world.kingdoms.getKingdomByID(suzerainId);
        List<Kingdom> vassals = GetVassals(suzerain);
        if (suzerain != null && vassals != null && vassals.Contains(pKingdom))
        {
          BannerLoader banner_suzerain;

          // 检查字典中是否已经有这个键对应的 BannerLoader
          if (!bannerLoaders.ContainsKey(key))
          {
            // 如果没有，创建一个新的 BannerLoader 并添加到字典中
            GameObject tempObject = new GameObject();
            tempObject.transform.position = new Vector3(25, 0, 0);
            Transform pTransform = tempObject.transform;
            banner_suzerain = Instantiate<BannerLoader>(__instance.banner_kingdoms, pTransform);
            bannerLoaders[key] = banner_suzerain;
          }
          else
          {
            // 从字典中获取 BannerLoader
            banner_suzerain = bannerLoaders[key];
          }

          banner_suzerain.load(suzerain);
          banner_suzerain.gameObject.SetActive(true);

          BannerContainer bannerContainer = BannerGenerator.dict[suzerain.race.banner_id];
          Transform bannerTransform = __instance.banner_kingdoms.transform;

          Vector3 currentPosition = bannerTransform.position;
          Vector3 newPosition = new Vector3(currentPosition.x - 30, currentPosition.y, currentPosition.z);
          banner_suzerain.transform.SetParent(bannerTransform);
          banner_suzerain.transform.position = newPosition;
          banner_suzerain.transform.localScale = new Vector3(1, 1, 1);
        }
      }
      foreach (var pair in bannerLoaders)
      {
        string key1 = pair.Key;
        BannerLoader bannerLoader = pair.Value;

        // 从 key 中获取两个国家的 ID
        string[] ids = key1.Split('-');
        string vassalId = ids[0];
        string suzerainId1 = ids[1];

        // 查找这两个国家的对象
        Kingdom vassal = World.world.kingdoms.getKingdomByID(vassalId);
        Kingdom suzerain = World.world.kingdoms.getKingdomByID(suzerainId1);

        // 如果 vassal 不再是 suzerain 的附庸，或者 suzerain 不存在了，那就销毁 BannerLoader
        if (suzerain == null || !IsVassal(vassal) || !GetVassals(suzerain).Contains(vassal))
        {
          Destroy(bannerLoader.gameObject);
          bannerLoaders.Remove(key1);
          Debug.Log("Destroying banner for kingdom foreach " + key1);

          // 在使用 Remove 方法后立即退出循环，因为你不能在遍历字典的过程中修改它
          break;
        }
      }

    }
    [HarmonyPrefix]
    [HarmonyPatch(typeof(KingdomManager), "destroyKingdom")]
    public static void Prefix_suzerainDestroyKingdom(Kingdom pKingdom)
    {
      if (IsLord(pKingdom))
      {
        // 获取所有附庸国
        List<Kingdom> vassals = GetVassals(pKingdom);

        int yeardata = World.world.mapStats.getCurrentYear();

        // 对于每个附庸国，调用移除附庸和旗帜的函数
        foreach (var vassal in vassals)
        {
          RemoveVassalAndBanner(pKingdom, vassal, yeardata, false);
          RemoveVassalAndBanner(pKingdom, vassal, yeardata, true);
        }
      }
    }


    public static void UpdateColor(Kingdom kingdom)
    {
      // 获取并检查原始颜色
      string serializedOriginalColor = "";
      kingdom.data.get("originalColor", out serializedOriginalColor);
      if (!string.IsNullOrEmpty(serializedOriginalColor))
      {
        ColorAsset originalColor = Deserialize(serializedOriginalColor);
        kingdom.updateColor(originalColor);
        World.world.zoneCalculator.setDrawnZonesDirty();
        World.world.zoneCalculator.clearCurrentDrawnZones(true);
        World.world.zoneCalculator.redrawZones();
      }

      // 获取并检查原始颜色ID
      int originalColorID = -1;
      kingdom.data.get("originalColorID", out originalColorID);
      if (originalColorID != -1)
      {
        kingdom.data.colorID = originalColorID;
      }
    }


    [HarmonyPrefix]
    [HarmonyPatch(typeof(KingdomManager), "destroyKingdom")]
    public static void PrefixDestroyKingdom(Kingdom pKingdom)
    {
      if (IsVassal(pKingdom))
      {
        // 获取宗主国
        Kingdom suzerain = GetSuzerain(pKingdom);

        string baseKey = suzerain.data.id + "-" + pKingdom.data.id;
        int yeardata = World.world.mapStats.getCurrentYear();

        // 查找最新的附庸关系
        int counter = 1;
        while (FunctionHelper.kingdomVassalEndTime.ContainsKey(baseKey + "-" + counter))
        {
          counter++;
        }

        string key = baseKey + "-" + counter;
        FunctionHelper.kingdomVassalEndTime[key] = yeardata;
      }



      // 获取所有与该国家相关的键
      List<string> relatedKeys = bannerLoaders.Keys.Where(key => key.StartsWith(pKingdom.data.id + "-") || key.EndsWith("-" + pKingdom.data.id)).ToList();

      foreach (string key in relatedKeys)
      {
        Debug.Log("Destroying banner for kingdom " + key);
        BannerLoader banner_suzerain = bannerLoaders[key];
        Destroy(banner_suzerain.gameObject);
        bannerLoaders.Remove(key);
      }
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



    public static bool tryPlotVassalWar(Actor pActor, PlotAsset pPlotAsset)
    {
      // 基本的情节检查
      if (!basePlotChecks(pActor, pPlotAsset))
      {
        return false;
      }

      // 检查自上次附庸战争以来是否已经过了足够的时间
      if (!HasEnoughTimePassedSinceLastVassalWar(pActor.kingdom))
      {
        return false;
      }

      // 确定附庸战争的目标
      Kingdom vassalTarget = GetVassalTarget(pActor.kingdom);
      if (vassalTarget == null)
      {
        return false;
      }

      // 检查目标国家的城市数量是否大于或等于2
      if (vassalTarget.cities.Count < 2)
      {
        return false;
      }

      // 检查是否有足够的军事力量来进行战争
      if (!HasEnoughMilitaryPower(pActor.kingdom, vassalTarget))
      {
        return false;
      }

      // 如果所有条件都满足，那么就创建一个新的情节，并设置目标国家
      Plot plot = World.world.plots.newPlot(pActor, pPlotAsset);
      plot.rememberInitiators(pActor);
      plot.target_kingdom = vassalTarget;

      if (!plot.checkInitiatorAndTargets())
      {
        Debug.Log("tryPlotVassalWar is missing start requirements");
        return false;
      }

      return true;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ClanManager), "checkActionKing")]
    public static void checkKingActionvassals_post(Actor pActor, ref ClanManager __instance)
    {

      if (pActor.isFighting())
        return;

      bool flag = tryPlotVassalWar(pActor, AssetManager.plots_library.get("vassal_war"));


      if (flag)
      {

        Traverse.Create(__instance).Field("_timestamp_last_plot").SetValue(World.world.getCurWorldTime());
      }
    }
    [HarmonyPostfix]
    [HarmonyPatch(typeof(ClanManager), "checkActionKing")]
    public static void checkKingActionvassals_absorb_post(Actor pActor, ref ClanManager __instance)
    {
      if (pActor.isFighting())
        return;

      bool flagAbsorbVassal = tryPlotAbsorbVassal(pActor, AssetManager.plots_library.get("absorb_vassal"));
      bool flagIndependence = tryPlotIndependence(pActor, AssetManager.plots_library.get("Independence_War"));

      if (flagAbsorbVassal || flagIndependence)
      {
        Traverse.Create(__instance).Field("_timestamp_last_plot").SetValue(World.world.getCurWorldTime());
      }
    }



    public static bool tryPlotAbsorbVassal(Actor pActor, PlotAsset pPlotAsset)
    {
      // 基本的情节检查
      if (!basePlotChecks(pActor, pPlotAsset))
      {
        return false;
      }
      //Debug.Log("自检过");
      // 检查自上次吞并附庸以来是否已经过了足够的时间
      if (!HasEnoughTimePassedSinceLastAbsorbVassal(pActor.kingdom))
      {
        return false;
      }
      //Debug.Log("自检时间过");
      // 确定吞并的附庸目标
      Kingdom vassalTarget = GetVassalTargettoabsorb(pActor.kingdom);
      if (vassalTarget == null)
      {
        return false;
      }
      //Debug.Log("目标过");
      // 检查是否有足够的军事力量来进行吞并
      if (!HasEnoughMilitaryPower(pActor.kingdom, vassalTarget))
      {
        return false;
      }
      //Debug.Log("军事过");
      // 获取最新的附庸关系
      var relevantKeys = FunctionHelper.kingdomVassalEstablishmentTime.Keys
        .Where(key => key.StartsWith(pActor.kingdom.id + "-" + vassalTarget.id + "-"))
        .ToList();
      if (relevantKeys.Count > 0)
      {
        var latestKey = relevantKeys.Aggregate((maxKey, key) => int.Parse(maxKey.Split('-').Last()) > int.Parse(key.Split('-').Last()) ? maxKey : key);

        int establishmentYear = FunctionHelper.kingdomVassalEstablishmentTime[latestKey];
        if (World.world.mapStats.getCurrentYear() - establishmentYear < 50)
        {
          return false;
        }
        //Debug.Log("关系过");
      }

      else
      {
        // 如果字典中没有找到vassalKey，那么认为这不是一个有效的附庸关系
        return false;
      }
      //Debug.Log("过");
      // 如果所有条件都满足，那么就创建一个新的情节，并设置目标国家
      Plot plot = World.world.plots.newPlot(pActor, pPlotAsset);
      plot.rememberInitiators(pActor);
      plot.target_kingdom = vassalTarget;

      if (!plot.checkInitiatorAndTargets())
      {
        Debug.Log("tryPlotAbsorbVassal is missing start requirements");
        return false;
      }

      return true;
    }
    public static bool tryPlotIndependence(Actor pActor, PlotAsset pPlotAsset)
    {
      // 基本的情节检查
      if (!basePlotChecks(pActor, pPlotAsset))
      {
        return false;
      }

      //距离上次独立已经过了足够的时间
      if (!HasEnoughTimePassedSinceLastIndependence(pActor.kingdom))
      {
        return false;
      }

      // 确定战争的目标
      // 获取宗主国
      Kingdom suzerain = GetSuzerain(pActor.kingdom);
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
          if (neighbourKingdom != null)
          {
            KingdomOpinion opinion;
            try
            {
              opinion = BehaviourActionBase<Kingdom>.world.diplomacy.getOpinion(suzerain, neighbourKingdom);
            }
            catch (NullReferenceException)
            {
              continue;
            }
            if (opinion != null)
            {
              int adjustedTotal = opinion.total;
              if (IsVassal(neighbourKingdom) && GetSuzerain(neighbourKingdom).Equals(suzerain))
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
        Debug.Log("tryPlotindeWar is missing start requirements");
        return false;
      }

      return true;
    }


    public static Dictionary<string, int> LastVassalWarYears = new Dictionary<string, int>();

    public static bool HasEnoughTimePassedSinceLastVassalWar(Kingdom kingdom)
    {
      int currentYear = World.world.mapStats.getCurrentYear();
      string kingdomId = kingdom.data.id;

      // 如果这个王国从未发起过附庸战争，那么可以发起
      if (!LastVassalWarYears.ContainsKey(kingdomId))
      {
        return true;
      }

      int lastVassalWarYear = LastVassalWarYears[kingdomId];
      return currentYear - lastVassalWarYear >= 10; // 你可以根据你的需求调整这个时间
    }
    public static Dictionary<string, int> LastIndependenceWarYears = new Dictionary<string, int>();
    public static bool HasEnoughTimePassedSinceLastIndependence(Kingdom kingdom)
    {
      int currentYear = World.world.mapStats.getCurrentYear();
      string kingdomId = kingdom.data.id;

      // 如果这个王国从未发起过独立战争，那么可以发起
      if (!LastIndependenceWarYears.ContainsKey(kingdomId))
      {
        return true;
      }

      int lastIndependenceYear = LastIndependenceWarYears[kingdomId];
      return currentYear - lastIndependenceYear >= 10; // 你可以根据你的需求调整这个时间
    }
    public static Dictionary<string, int> LastAbsorbVassalYears = new Dictionary<string, int>();

    public static bool HasEnoughTimePassedSinceLastAbsorbVassal(Kingdom kingdom)
    {
      int currentYear = World.world.mapStats.getCurrentYear();
      string kingdomId = kingdom.data.id;

      // If this kingdom has never absorbed a vassal, it is allowed to do so
      if (!LastAbsorbVassalYears.ContainsKey(kingdomId))
      {
        return true;
      }

      int lastAbsorbVassalYear = LastAbsorbVassalYears[kingdomId];
      return currentYear - lastAbsorbVassalYear >= 10; // You can adjust this time according to your requirements
    }
    public static Kingdom GetVassalTargettoabsorb(Kingdom kingdom)
    {
      List<Kingdom> vassals = GetVassals(kingdom);

      foreach (Kingdom vassal in vassals)
      {

        if (kingdom.getArmy() < vassal.getArmy())
        { //Debug.Log("vasssss+"+kingdom.getArmy()+" "+vassal.getArmy());
          continue;
        }

        if (vassal.hasEnemies())
        { //Debug.Log("has ene");
          continue;
        }

        // 如果附庸满足所有条件，那么它就是我们的目标
        return vassal;
      }

      // 如果没有找到满足条件的附庸，那么返回 null
      return null;
    }

    public static bool HasEnoughMilitaryPower(Kingdom initiator, Kingdom target)
    {
      int initiatorPower = initiator.getArmy();
      int targetPower;

      // If target is a vassal
      if (IsVassal(target))
      {
        Kingdom suzerain = GetSuzerain(target);

        // If the suzerain of the vassal is the initiator itself
        if (suzerain == initiator)
        {
          targetPower = target.getArmy();
        }
        else
        {
          // Calculate combined power of the suzerain and its vassals
          targetPower = getSuzerainArmy(suzerain);
        }
      }
      else
      {
        targetPower = target.getArmy();
      }

      int sum = (initiatorPower + targetPower) / 2;

      return initiatorPower >= sum;
    }


    private static bool basePlotChecks(Actor pActor, PlotAsset pPlotAsset)
    {
      if (pActor == null || pPlotAsset == null) { return false; }

      if (!World.world.worldLaws.world_law_rebellions.boolVal || !(pActor.getInfluence() >= pPlotAsset.cost && pPlotAsset.checkInitiatorPossible(pActor) && pPlotAsset.check_launch(pActor, pActor.kingdom)))
      {
        return false;
      }

      return true;
    }





    public static string Serialize(ColorAsset colorAsset)
    {
      return $"{colorAsset.color_main},{colorAsset.color_main_2},{colorAsset.color_banner},{colorAsset.index_id}";
    }

    public static ColorAsset Deserialize(string s)
    {
      var parts = s.Split(',');
      if (parts.Length != 4)
      {
        throw new ArgumentException("Invalid input string");
      }
      var pColorMain = parts[0];
      var pColorMain2 = parts[1];
      var pColorBanner = parts[2];
      var indexId = int.Parse(parts[3]);

      var result = new ColorAsset(pColorMain, pColorMain2, pColorBanner)
      {
        index_id = indexId
      };

      result.initColor(); // 重新计算颜色值
      return result;
    }

    public static void CheckAndCleanCityList()
    {
      List<City> cityList = World.world.cities.list;

      // Create a copy of the list
      List<City> copyOfCityList = new List<City>(cityList);

      // Destroy invalid cities
      foreach (City city in copyOfCityList)
      {
        if (city == null || city.data == null || city.data.storage == null || city.getTile() == null)
        {
          World.world.cities.destroyCity(city);
        }
      }

      // Then, remove invalid cities from the list
      cityList.RemoveAll(city => city == null || city.data == null || city.data.storage == null || city.getTile() == null || city.getPopulationTotal(true) == 0);
    }
    /*
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CityBehCheckCulture), "recalcMainCulture")]
        public static bool PrefixRecalcMainCulture(CityBehCheckCulture __instance, City pCity)
        {
            // Check if the City instance is null
            if (pCity == null)
            {
                Debug.LogWarning("City instance is null in recalcMainCulture");
                return false;
            }

            // Check if any Actor in the City's units is null
            foreach (Actor actor in pCity.units)
            {
                if (actor == null)
                {
                    Debug.LogWarning("Actor instance is null in recalcMainCulture");
                    return false;
                }
            }

            return true;  // If everything checks out, proceed with the original method
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(City), "giveItem")]
        public static bool PrefixGiveItem(Actor pActor, List<ItemData> pItems, City pCity)
        {
            // Check if the Actor instance is null
            if (pActor == null)
            {
                Debug.LogWarning("Actor instance is null in giveItem");
                return false;
            }

            // Check if the City instance is null


            return true;  // If everything checks out, proceed with the original method
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BehJoinCity), "isPossibleToJoin")]
        public static bool PrefixIsPossibleToJoin(Actor pActor)
        {
            // Check if the Actor instance is null
            if (pActor == null)
            {
                Debug.LogWarning("Actor instance is null in isPossibleToJoin");
                return false;
            }



            return true;  // If everything checks out, proceed with the original method
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(City), "getFoodItem")]
        public static bool PrefixGetFoodItem(City __instance, ref ResourceAsset __result, string pFavoriteFood)
        {
            // Check if the City instance is null
            if (__instance == null)
            {
                Debug.LogWarning("City instance is null in getFoodItem");
                return false;
            }

            return true;  // If everything checks out, proceed with the original method
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(City), "updateCitizens")]
        public static bool PrefixUpdateCitizens(City __instance)
        {
            if (__instance == null)
            {
                Debug.LogWarning("City instance is null in updateCitizens");
                return false;
            }

            List<Actor> simpleList = __instance.units.getSimpleList();
            for (int j = 0; j < simpleList.Count; j++)
            {
                Actor actor = simpleList[j];
                if (actor == null)
                {
                    Debug.LogWarning("Actor instance is null in updateCitizens");
                    __instance.units.Remove(actor);
                    return false;
                }
            }

            // Rest of your code...

            return true;  // If everything checks out, proceed with the original method
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(City), "updateCityStatus")]
        public static bool PrefixUpdateCityStatus(City __instance)
        {
            if (__instance == null)
            {
                Debug.LogWarning("City instance is null in updateCityStatus");
                return false;
            }

            List<Actor> simpleList = __instance.units.getSimpleList();
            for (int i = 0; i < simpleList.Count; i++)
            {
                Actor actor = simpleList[i];
                if (actor == null)
                {
                    Debug.LogWarning("Actor instance is null in updateCityStatus");
                    return false;
                }
            }

            // Rest of your code...

            return true;  // If everything checks out, proceed with the original method
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Actor), "setProfession")]
        public static bool PrefixSetProfession(Actor __instance)
        {
            // Check if the Actor instance is null
            if (__instance == null)
            {
                Debug.LogWarning("Actor instance is null in setProfession");
                return false;
            }

            // Check if the City instance is null


            return true;  // If everything checks out, proceed with the original method
        }
        */
  }
}