using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;
namespace Figurebox.patch;

internal class KingActorPatch
{
  public static void updateKingHistory(Actor __instance, UnitProfession pType)
  {
    string kingId = __instance.data.id;
    string kingdomId = __instance.kingdom.data.id;
    int yeardata = World.world.mapStats.getCurrentYear();
    string kingdomname = __instance.kingdom.data.name;
    int count = 1;
    if (kingdomId != "nomads_" + __instance.asset.race)
    {
      if (!FunctionHelper.KingStartYearInKingdom.ContainsKey($"{kingId}-{kingdomId}-{count}"))
      {
        // 这是国王在这个王国的第一次上位，直接记录
        string kingAndKingdom = $"{kingId}-{kingdomId}-{count}";
        FunctionHelper.KingStartYearInKingdom[kingAndKingdom] = yeardata;
      }
      while (FunctionHelper.KingEndYearInKingdom.ContainsKey($"{kingId}-{kingdomId}-{count}"))
      {
        count++;
      }
      if (FunctionHelper.KingEndYearInKingdom.ContainsKey($"{kingId}-{kingdomId}-{count}"))
      {
        // 这个国王在这个王国已经退位过，他的第二次上位是真实的

        string kingAndKingdom = $"{kingId}-{kingdomId}-{count}";
        FunctionHelper.KingStartYearInKingdom[kingAndKingdom] = yeardata;
      }
    }


    if (!FunctionHelper.KingName.ContainsKey(__instance.data.id) && !FunctionHelper.KingKingdomName.ContainsKey(kingdomname) && !FunctionHelper.KingKingdoms.ContainsKey(__instance.data.id))
    {

      FunctionHelper.KingName.Add(__instance.data.id, __instance.getName());
      FunctionHelper.KingKingdomName[__instance.data.id] = new List<string>();
      FunctionHelper.KingKingdoms[__instance.data.id] = new List<string>();
    }
    FunctionHelper.KingKingdoms[__instance.data.id].Add(kingdomId);
    FunctionHelper.KingKingdomName[__instance.data.id].Add(kingdomname);
    if (FunctionHelper.kingYearData.ContainsKey(__instance.data.id))
    {
      FunctionHelper.kingYearData[__instance.data.id]++; // 如果字典中已经包含了当前 king 的数据，则更新它
      // 更新 year 数据
    }
    else
    {
      // 向字典中添加新的数据
      FunctionHelper.kingYearData.Add(__instance.data.id, 1); // 添加 year 数据并将其设置为 1
    }
    if (!FunctionHelper.kingdomids.Contains(__instance.kingdom.data.id))
    {
      FunctionHelper.kingdomids.Add(__instance.kingdom.data.id);

    }
  }

  public static void setKing_Postfix(Actor __instance, UnitProfession pType)
  {
    if (!FunctionHelper.is_chinese) return;

    if (__instance.hasTrait("zhuhou")) return;
    foreach (War war in World.world.wars.getWars(__instance.kingdom))
    {
      foreach (Kingdom defender in war.getDefenders())
      {
        //Debug.Log("叛乱国家列:"+defender.data.name);
        if (defender == __instance.kingdom && war._asset == AssetManager.war_types_library.get("inspire"))
        {
          //Debug.Log("inspire不执行setking");
          __instance.addTrait("rebel");
          return;
        }
        if (defender == __instance.kingdom && war._asset == AssetManager.war_types_library.get("rebellion"))
        {
          __instance.addTrait("rebel");
          //Debug.Log("rebel不执行setking");
          return;
        }
      }
    }
    //给他反抗军特质完了反抗军特质执行一次setprofe然后同时删除反抗军特质
    __instance.addTrait("zhuhou");
    /* __instance.addTrait("guizu");
       __instance.removeTrait("noble");
       __instance.removeTrait("shibing");*/
    if (__instance == null || __instance.city == null || __instance.kingdom.capital == null) return;
    __instance.city = __instance.kingdom.capital;
    string[] array1 =
    {
      "洪", "建", "永", "弘", "正", "景", "康", "嘉", "兴", "明", "平", "盛", "隆"
    };
    string[] array2 =
    {
      "武", "文", "乐", "熙", "治", "德", "泰", "运", "阳", "靖", "统", "乾", "庆"
    };
    //var list2 = new List<string> { "", "", "" };


    var result = FunctionHelper.GetRandomStrings(array1, array2);
    string kingId = __instance.data.id;
    FunctionHelper.kingdomYearNameData[kingId] = result;

    // Use result from the kingdomYearNameData dictionary
    string yearName = FunctionHelper.kingdomYearNameData[kingId];
    int dashIndex = __instance.kingdom.data.name.IndexOf("-", StringComparison.Ordinal);
    if (dashIndex >= 0)
    {
      __instance.kingdom.data.name = __instance.kingdom.data.name.Substring(0, dashIndex);
    }

    __instance.kingdom.data.name = __instance.kingdom.data.name + "-" + yearName + "元年";

  }
  public static void setNewCapital(Actor __instance, UnitProfession pType)
  {
    bool tianmingBoolValue;
    __instance.kingdom.data.get("tianmingbool", out tianmingBoolValue);
    if (tianmingBoolValue) { __instance.addTrait("天命"); }

    if (__instance.kingdom.getEnemiesKingdoms().Count == 0 && __instance.kingdom.capital != null)
    {
      City newCapital = __instance.kingdom.cities
        .Select(city =>
        {
          double score = (city.getAge() - __instance.kingdom.capital.getAge()) * 1 +
                         (city.getPopulationTotal() - __instance.kingdom.capital.getPopulationTotal()) * 2 +
                         (city.zones.Count - __instance.kingdom.capital.zones.Count) * 0.35 +
                         (city.neighbours_cities.SetEquals(city.neighbours_cities_kingdom) ? 50 : 0);


          Debug.Log($"City: {city.data.name}, Score: {score}");
          return new
          {
            City = city,
            Score = score
          };
        })
        .OrderByDescending(cityScore => cityScore.Score)
        .Select(cityScore => cityScore.City)
        .FirstOrDefault();

      if (newCapital != null)
      {
        __instance.kingdom.capital = newCapital;
        Debug.Log("New capital set to " + newCapital.data.name);
      }
    }
  }
  [HarmonyPostfix]
  [HarmonyPatch(typeof(Actor), nameof(Actor.setProfession))]
  public static void beKing_Postfix(Actor __instance, UnitProfession pType)
  {
    if (pType != UnitProfession.King) return;
    setNewCapital(__instance, pType);
    setKing_Postfix(__instance, pType);
    updateKingHistory(__instance, pType);

  }
  [HarmonyPrefix]
  [HarmonyPatch(typeof(Actor), "killHimself")]
  public static bool getkingleft_prefix(Actor __instance)
  {
    if (__instance == null || __instance.kingdom == null)
    {
      Debug.LogWarning("Either __instance, __instance.kingdom is null");
      return true;
    }

    string kingId = __instance.data.id;
    string kingdomId = __instance.kingdom.data.id;
    int yeardata = World.world.mapStats.getCurrentYear();

    if (__instance.kingdom.king == __instance)
    {
      // Find the first available count for this king and kingdom.
      int count = 1;
      while (FunctionHelper.KingEndYearInKingdom.ContainsKey(kingId + "-" + kingdomId + "-" + count))
      {
        count++;
      }

      string kingAndKingdom = kingId + "-" + kingdomId + "-" + count;
      FunctionHelper.KingEndYearInKingdom[kingAndKingdom] = yeardata;
    }
    return true;
  }
}