using System;
using System.Collections.Generic;
using System.Linq;
using Figurebox.Utils;
using HarmonyLib;
using NeoModLoader.General;
using UnityEngine;
using Random = System.Random;
namespace Figurebox
{
  class FunctionHelper //失去天命后首都周围地减低轻微忠诚度，飞地直接独立或者加入其他国家 天命减少 国王的氏族绝嗣,（没有开国之君特质) 收复战争得先判断双方实力
  {
    public static FunctionHelper instance;
    public static Dictionary<string, int> kingYearData = new Dictionary<string, int>();
    public static Dictionary<string, int> YearData = new Dictionary<string, int>();
    public static Dictionary<string, string> TmkingData = new Dictionary<string, string>();
    public static Dictionary<string, string> kingdomYearNameData = new Dictionary<string, string>();
    public static Dictionary<string, int> kingdomVassalEstablishmentTime = new Dictionary<string, int>();
    public static Dictionary<string, int> kingdomVassalEndTime = new Dictionary<string, int>();



    public static int tianmingvalue = 0;
    public static bool notdoingwellinwar = false;

    public static List<Actor> kingsThatHaveLogged = new();

    // 创建一个Dictionary用于保存War Id和Name
    public static Dictionary<string, string> warIdNameDict = new Dictionary<string, string>();
    public static Dictionary<string, double> WarStartDate = new Dictionary<string, double>();
    public static Dictionary<string, List<string>> Attackers = new Dictionary<string, List<string>>();
    public static Dictionary<string, List<string>> Defenders = new Dictionary<string, List<string>>();
    public static Dictionary<string, int> WarEndDate = new Dictionary<string, int>();
    public static Dictionary<string, double> WarEndDateFloat = new Dictionary<string, double>();

    public static Dictionary<War, List<City>> storedDefenderCities = new Dictionary<War, List<City>>();

    public static Dictionary<string, string> kingdomCityData = new Dictionary<string, string>();
    public static Dictionary<string, string> kingdomCityNameyData = new Dictionary<string, string>();
    public static Dictionary<string, Dictionary<string, Tuple<int, int, int>>> CityYearData = new Dictionary<string, Dictionary<string, Tuple<int, int, int>>>();
    public static Dictionary<string, int> KingStartYearInKingdom = new Dictionary<string, int>();
    public static Dictionary<string, List<string>> KingKingdomName = new Dictionary<string, List<string>>();
    public static Dictionary<string, string> KingName = new Dictionary<string, string>();
    public static Dictionary<string, List<string>> KingKingdoms = new Dictionary<string, List<string>>();
    public static List<string> kingdomids = new List<string>();
    public static Dictionary<string, int> KingEndYearInKingdom = new Dictionary<string, int>();
    public static bool is_chinese => LocalizedTextManager.instance.language == "cz" || LocalizedTextManager.instance.language == "ch";

    [HarmonyPostfix]
    [HarmonyPatch(typeof(WorldLogMessageExtensions), "getFormatedText")]
    public static void getFormatedText(ref string __result, ref WorldLogMessage pMessage)
    {

      switch (pMessage.text)
      {
        case "baseLog":
          __result = LM.Get(pMessage.text);
          break;
        case "historicalMessage":
          string text = LM.Get(pMessage.text);
          if (pMessage.unit == null)
          {
            pMessage.icon = "iconDeathMark";
            text = text.Replace("$ren$", string.Concat(new string[]
            {
              pMessage.special1
            }));
            __result = text;
            return;
          }
          text = text.Replace("$ren$", string.Concat(new string[]
          {
            "<color=", Toolbox.colorToHex(pMessage.color_special1, true), ">", pMessage.unit.getName(), "</color>"
          }));
          pMessage.icon = "iconDocument";
          __result = text;
          break;
        case "mandateofheavenMessage":
          text = LM.Get(pMessage.text);
          if (pMessage.unit == null)
          {
            pMessage.icon = "iconDead";
            text = text.Replace("$king$", string.Concat(new string[]
            {
              pMessage.special2
            }));
            text = text.Replace("$kingdom$", string.Concat(new string[]
            {
              pMessage.special1
            }));
            __result = text;
            return;
          }
          text = text.Replace("$king$", string.Concat(new string[]
          {
            "<color=", Toolbox.colorToHex(pMessage.color_special1, true), ">", pMessage.unit.GetData().name, "</color>"
          }));
          text = text.Replace("$kingdom$", string.Concat(new string[]
          {
            "<color=", Toolbox.colorToHex(pMessage.color_special1, true), ">", pMessage.kingdom.name, "</color>"
          }));
          pMessage.icon = "iconKingdom";
          __result = text;
          break;
        case "losemandateofheavenMessage":
          text = LM.Get(pMessage.text);
          if (pMessage.unit == null)
          {
            pMessage.icon = "iconDead";
            text = text.Replace("$king$", string.Concat(new string[]
            {
              pMessage.special2
            }));
            text = text.Replace("$kingdom$", string.Concat(new string[]
            {
              pMessage.special1
            }));
            __result = text;
            return;
          }
          text = text.Replace("$king$", string.Concat(new string[]
          {
            "<color=", Toolbox.colorToHex(pMessage.color_special1, true), ">", pMessage.unit.GetData().name, "</color>"
          }));
          text = text.Replace("$kingdom$", string.Concat(new string[]
          {
            "<color=", Toolbox.colorToHex(pMessage.color_special1, true), ">", pMessage.kingdom.name, "</color>"
          }));
          pMessage.icon = "iconDocument";
          __result = text;
          break;
        case "warmandateofheavenMessage":
          text = LM.Get(pMessage.text);
          text = text.Replace("$kingdom$", string.Concat(new string[]
          {
            "<color=", Toolbox.colorToHex(pMessage.color_special1, true), ">", pMessage.special2, "</color>"
          }));
          text = text.Replace("$kingdom2$", string.Concat(new string[]
          {
            "<color=", Toolbox.colorToHex(pMessage.color_special2, true), ">", pMessage.special1, "</color>"
          }));
          pMessage.icon = "iconDocument";
          __result = text;
          break;
        case "losekingdommandateofheavenMessage":
          text = LM.Get(pMessage.text);
          text = text.Replace("$kingdom$", string.Concat(new string[]
          {
            "<color=", Toolbox.colorToHex(pMessage.color_special1, true), ">", pMessage.special1, "</color>"
          }));
          pMessage.icon = "iconDocument";
          __result = text;
          break;
        case "joinanotherkingdomMessage":
          text = LM.Get(pMessage.text);

          text = text.Replace("$kingdom$", string.Concat(new string[]
          {
            "<color=", Toolbox.colorToHex(pMessage.color_special1, true), ">", pMessage.special2, "</color>"
          }));
          text = text.Replace("$kingdom2$", string.Concat(new string[]
          {
            "<color=", Toolbox.colorToHex(pMessage.color_special2, true), ">", pMessage.special1, "</color>"
          }));
          pMessage.icon = "iconDocument";
          __result = text;
          break;
        case "reclaimwarendMessage":
          text = LM.Get(pMessage.text);
          text = text.Replace("$kingdom$", string.Concat(new string[]
          {
            "<color=", Toolbox.colorToHex(pMessage.color_special1, true), ">", pMessage.special2, "</color>"
          }));
          text = text.Replace("$kingdom2$", string.Concat(new string[]
          {
            "<color=", Toolbox.colorToHex(pMessage.color_special2, true), ">", pMessage.special1, "</color>"
          }));
          text = text.Replace("$winner$", string.Concat(new string[]
          {
            "<color=", Toolbox.colorToHex(pMessage.color_special3, true), ">", pMessage.special3, "</color>"
          }));
          pMessage.icon = "iconDocument";
          __result = text;
          break;
        case "usurpationMessage":
          text = LM.Get(pMessage.text);
          text = text.Replace("$kingdom$", string.Concat(new string[]
          {
            "<color=", Toolbox.colorToHex(pMessage.color_special1, true), ">", pMessage.special2, "</color>"
          }));
          text = text.Replace("$first$", string.Concat(new string[]
          {
            "<color=", Toolbox.colorToHex(pMessage.color_special1, true), ">", pMessage.special1, "</color>"
          }));
          pMessage.icon = "iconDocument";
          __result = text;
          break;
        case "vassalWarStartMessage":
          text = LM.Get(pMessage.text);
          text = text.Replace("$first$", string.Concat(new string[]
          {
            "<color=", Toolbox.colorToHex(pMessage.color_special1, true), ">", pMessage.special2, "</color>"
          }));
          text = text.Replace("$kingdom2$", string.Concat(new string[]
          {
            "<color=", Toolbox.colorToHex(pMessage.color_special2, true), ">", pMessage.special1, "</color>"
          }));
          pMessage.icon = "iconDocument"; // 修改为你想要的图标
          __result = text;
          break;
        case "vassalWarEndMessage":
          text = LM.Get(pMessage.text);
          text = text.Replace("$first$", string.Concat(new string[]
          {
            "<color=", Toolbox.colorToHex(pMessage.color_special1, true), ">", pMessage.special2, "</color>"
          }));
          text = text.Replace("$kingdom2$", string.Concat(new string[]
          {
            "<color=", Toolbox.colorToHex(pMessage.color_special2, true), ">", pMessage.special1, "</color>"
          }));
          pMessage.icon = "iconDocument"; // 修改为你想要的图标
          __result = text;
          break;
        case "IndependenceWarMessage":
          text = LM.Get(pMessage.text);
          text = text.Replace("$kingdom$", string.Concat(new string[]
          {
            "<color=", Toolbox.colorToHex(pMessage.color_special1, true), ">", pMessage.special2, "</color>"
          }));
          text = text.Replace("$kingdom2$", string.Concat(new string[]
          {
            "<color=", Toolbox.colorToHex(pMessage.color_special2, true), ">", pMessage.special1, "</color>"
          }));
          pMessage.icon = "iconDocument";
          __result = text;
          break;


      }
    }
    public static bool CheckLastChars(string str, int numChars, string[] array)
    {
      int startIndex = str.Length - numChars;
      if (startIndex < 0)
      {
        return false;
      }
      string lastChars = str.Substring(startIndex, numChars);
      return array.Contains(lastChars);
    }
    [HarmonyPostfix]
    [HarmonyPatch(typeof(MapBox), "updateObjectAge")]
    public static void UpdateTianmingTrait(MapBox __instance)
    {
      foreach (Kingdom k in World.world.kingdoms.list_civs)
      {
        bool tianmingcunzai;
        k.data.get("tianmingbool", out tianmingcunzai, false);

        if (!tianmingcunzai)
        {
          k.data.set("tianmingbool", false);
        }
      }

      if (tianmingvalue >= 40)
      {
        foreach (Kingdom kdm in World.world.kingdoms.list_civs)
        {
          bool tianmingBoolValue;
          kdm.data.get("tianmingbool", out tianmingBoolValue);

          if (tianmingBoolValue && kdm.king != null)
          {
            kdm.king.addStatusEffect("tianming0", 100f);
          }
        }
      }
      if (tianmingvalue <= 20)
      {
        foreach (Kingdom kdm in World.world.kingdoms.list_civs)
        {
          bool tianmingBoolValue;
          kdm.data.get("tianmingbool", out tianmingBoolValue);

          if (tianmingBoolValue && kdm.king != null)
          {
            kdm.king.addStatusEffect("tianmingm1", 100f);
          }
        }
      }

      if (tianmingvalue <= -20)
      {
        Kingdom maxKingdom = DiplomacyManager.kingdom_supreme;

        if (maxKingdom != null)
        {
          foreach (Kingdom k in World.world.kingdoms.list_civs)
          {
            bool tianmingBoolValue;
            k.data.get("tianmingbool", out tianmingBoolValue);

            if (tianmingBoolValue && k != maxKingdom)
            {
              k.data.set("tianmingbool", false);
              if (k.king != null)
              {
                k.king.removeTrait("天命");
                k.king.addTrait("formerking");
                tianmingvalue = 0;
                k.data.set("tianmingbool", false);
              }
              List<City> cityListCopy = new List<City>(k.cities);
              foreach (City city in cityListCopy)
              {
                PlotAsset rebellionPlot = AssetManager.plots_library.get("rebellion");
                if (city != k.capital && city.leader != null && !city.neighbours_cities.Contains(k.capital))
                {
                  Plot pPlot = World.world.plots.newPlot(city.leader, rebellionPlot);

                  pPlot.initiator_city = city;
                  pPlot._leader = city.leader;
                  pPlot.initiator_actor = city.leader;
                  pPlot.initiator_kingdom = k;
                  pPlot.target_city = city;

                  rebellionPlot.action(pPlot);
                  WorldLog.logCityRevolt(city);
                }
              }
            }
          }
        }
      }

      bool anyKingdomTrue = false;
      foreach (Kingdom k in World.world.kingdoms.list_civs)
      {
        bool tianmingBoolValue;
        k.data.get("tianmingbool", out tianmingBoolValue);

        if (tianmingBoolValue)
        {
          anyKingdomTrue = true;
          break;
        }
      }

      if (!anyKingdomTrue)
      {
        foreach (Kingdom k in World.world.kingdoms.list_civs)
        {
          if (k.isSupreme() && k.king != null)
          {
            if (k.king.hasTrait("first"))
            {
              k.data.set("tianmingbool", true);
              if (!k.king.hasTrait("天命"))
              {
                k.king.addTrait("天命");
              }
              tianmingvalue = 50;
            }
            break;
          }
        }
      }

      if (anyKingdomTrue)
      {
        foreach (Kingdom k in World.world.kingdoms.list_civs)
        {
          bool tianmingBoolValue;
          k.data.get("tianmingbool", out tianmingBoolValue);

          if (k.isSupreme() && k.king != null)
          {
            if (k != DiplomacyManager.kingdom_supreme && tianmingBoolValue)
            {
              k.data.set("tianmingbool", false);
              if (k.king != null)
              {
                k.king.removeTrait("天命");
              }
            }
          }
        }
      }

      foreach (Kingdom k in World.world.kingdoms.list_civs)
      {
        bool tianmingBoolValue;
        k.data.get("tianmingbool", out tianmingBoolValue);

        if (tianmingBoolValue && k.getEnemiesKingdoms().Count == 0)
        {
          tianmingvalue += 1;
        }
        if (k.king != null)
        {
          if (tianmingBoolValue && k.king.hasTrait("first"))
          {
            tianmingvalue += 3;
          }
        }
        if (k.king != null)
        {
          if (tianmingBoolValue && k.king.getAge() <= 24)
          {
            tianmingvalue -= 1;
          }
          if (tianmingBoolValue && !k.isSupreme())
          {
            tianmingvalue -= 2;
          }
        }
        Clan kclan = BehaviourActionBase<Kingdom>.world.clans.get(k.data.royal_clan_id);
        if (kclan != null && tianmingBoolValue && kclan.units.Count <= 2)
        {
          tianmingvalue -= 1;
        }
        else if (kclan != null && tianmingBoolValue && kclan.units.Count >= 10)
        {
          tianmingvalue += 1;
        }
      }

      int i = 0;

      foreach (Kingdom k in World.world.kingdoms.list_civs)
      {
        foreach (War war in World.world.wars.getWars(k))
        {
          if ((war.main_attacker == k && war._asset == AssetManager.war_types_library.get("tianmingrebel"))
              || (war.main_defender == k && war._asset == AssetManager.war_types_library.get("tianming")))
          {
            i++;
          }
        }
      }

      if (i > 5)
      {
        i = 5;
      }
      if (notdoingwellinwar)
      {
        tianmingvalue -= 1;
      }

      tianmingvalue -= i;

      if (tianmingvalue >= 100)
      {
        tianmingvalue = 100;
      }
      if (tianmingvalue <= -35)
      {
        tianmingvalue = 0;
      }
    }
    public static bool HasRecapturedAllRecentTerritories(War war)
    {
      if (war == null)
      {
        Debug.Log("War object is null");
        return false;
      }

      Kingdom attacker = war.main_attacker;
      Kingdom defender = war.main_defender;

      if (attacker == null)
      {
        Debug.Log("Main attacker in War object is null");
        return false;
      }

      if (defender == null)
      {
        Debug.Log("Main defender in War object is null");
        return false;
      }

      if (!storedDefenderCities.ContainsKey(war))
      {
        Debug.Log("War object is not a key in storedDefenderCities");
        return false;
      }

      if (World.world == null || World.world.mapStats == null)
      {
        Debug.Log("World.world or World.world.mapStats is null");
        return false;
      }

      int currentYear = World.world.mapStats.getCurrentYear();

      foreach (var city in storedDefenderCities[war])
      {
        if (city == null)
        {
          Debug.Log("City in storedDefenderCities is null");
          continue;
        }

        if (city.data == null || city.data.id == null)
        {
          Debug.Log("City data or city data id is null");
          continue;
        }

        string cityId = city.data.id;
        string attackerId = attacker.data.id;

        if (!CityYearData.ContainsKey(cityId))
        {
          Debug.Log("CityId is not a key in CityYearData");
          continue;
        }

        var entries = CityYearData[cityId].OrderByDescending(e => e.Value.Item2).ToList();

        foreach (var entry in CityYearData[cityId])
        {
          int yearsSinceCapture = currentYear - entry.Value.Item2;
          if (entry.Key.StartsWith(attackerId + "-") && yearsSinceCapture >= 5 && yearsSinceCapture < 100)
          {
            if (city.kingdom == null || city.kingdom.data == null || city.kingdom.data.id == null)
            {
              Debug.Log("City's kingdom or city's kingdom data or city's kingdom data id is null");
              continue;
            }

            if (city.kingdom.data.id != attackerId)
            {
              return false;
            }
          }
        }

        for (int i = 1; i < entries.Count; i++)
        {
          if (entries[i - 1].Value.Item2 >= currentYear - 100 && entries[i - 1].Key.Split('-')[0] != attackerId)
          {
            if (entries[i].Key.Split('-')[0] == attackerId && entries[i - 1].Value.Item2 - entries[i].Value.Item2 >= 60)
            {
              if (city.kingdom == null || city.kingdom.data == null || city.kingdom.data.id == null)
              {
                Debug.Log("City's kingdom or city's kingdom data or city's kingdom data id is null");
                continue;
              }

              if (city.kingdom.data.id != attackerId)
              {
                return false;
              }
            }
          }
        }
      }

      return true;
    }
    [HarmonyPostfix]
    [HarmonyPatch(typeof(City), "update")]
    public static void CityAgePostfix(City __instance)
    {
      if (__instance.kingdom == null) { return; }
      string cityId = __instance.data.id;
      string kingdomId = __instance.kingdom.data.id;
      int yeardata = World.world.mapStats.getCurrentYear();

      // 添加一个新的变量来跟踪收复次数
      int recaptureCount = 1;

      if (!kingdomCityData.ContainsKey(cityId))
      {
        kingdomCityData[cityId] = kingdomId;
        kingdomCityNameyData[kingdomId] = __instance.kingdom.data.name;

        string combinedKey = kingdomId + "-" + recaptureCount;
        if (!CityYearData.ContainsKey(cityId))
        {
          CityYearData[cityId] = new Dictionary<string, Tuple<int, int, int>>
          {
            {
              combinedKey, new Tuple<int, int, int>(recaptureCount, yeardata, 0)
            }
          };
        }
      }
      else if (kingdomCityData[cityId] != kingdomId)
      {
        kingdomCityData[cityId] = kingdomId;
        kingdomCityNameyData[kingdomId] = __instance.kingdom.data.name;

        // 查找当前收复次数
        if (!CityYearData.TryGetValue(cityId, out Dictionary<string, Tuple<int, int, int>> value))
        {
          value = new Dictionary<string, Tuple<int, int, int>>();
          CityYearData[cityId] = value;
        }
        recaptureCount = value.Count(entry => entry.Key.StartsWith(kingdomId + "-")) + 1;

        string combinedKey = kingdomId + "-" + recaptureCount;
        if (!CityYearData[cityId].ContainsKey(combinedKey))
        {
          CityYearData[cityId].Add(combinedKey, new Tuple<int, int, int>(recaptureCount, yeardata, recaptureCount - 1));
        }
        else
        {
          CityYearData[cityId][combinedKey] = new Tuple<int, int, int>(recaptureCount, yeardata, CityYearData[cityId][combinedKey].Item3);
        }
      }
    }

    public static int CalculateKingdomValue(Kingdom k)
    {
      int populationTotal = k.getPopulationTotal();
      int cityCount = k.cities.Count * 100;
      int armySize = k.getArmy();
      int stewardship = 8;
      if (k.king != null)
      {
        stewardship = k.king.GetData().stewardship * 10;


      }
      return populationTotal + cityCount + armySize + stewardship;
    }

    public static string GetRandomStrings(string[] array1, string[] array2)
    {
      var rand = new Random();
      var result = "";

      var index1 = rand.Next(array1.Length);
      result += array1[index1];

      var index2 = rand.Next(array2.Length);
      result += array2[index2];

      return result;
    }
  }
}