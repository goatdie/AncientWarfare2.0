using System.Collections.Generic;
using ai.behaviours;
using Figurebox.core;
using Figurebox.Utils;
using HarmonyLib;
using UnityEngine;
namespace Figurebox;

class KingdomPatch
{
  [HarmonyPostfix]
  [HarmonyPatch(typeof(Kingdom), "updateAge")]
  public static void CheckTianmingstatus_Postfix(Kingdom __instance)
  {
    if (__instance.king == null) { return; }
    if (__instance.king.hasStatus("tianming0"))
    {
      List<Actor> simpleList = __instance.units.getSimpleList();
      foreach (Actor a in simpleList)
      {
        if (a != null && a.data.profession == UnitProfession.Warrior)
        {
          a.addStatusEffect("powerup", 50f);


        }

      }
      foreach (City city in __instance.cities)
      {

        city.data.storage.change("bread", 10);
      }




    }
    if (__instance.king.hasStatus("tianmingm1"))
    {

      foreach (City city in __instance.cities)
      {

        city.data.storage.change("bread", -5);
      }
    }
  }
  [HarmonyPrefix]
  [HarmonyPatch(typeof(Kingdom), "removeUnit")]
  public static bool getkingleft1_prefix(Actor pUnit, Kingdom __instance)
  {
    if (pUnit == null || __instance == null)
    {
      Debug.LogWarning("Either __instance, pUnit is null");
      return true;
    }

    string kingId = pUnit.data.id;
    string kingdomId = __instance.data.id;
    int yeardata = World.world.mapStats.getCurrentYear();

    if (__instance.king == pUnit)
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
 

  [HarmonyPostfix]
  [HarmonyPatch(typeof(Kingdom), "updateAge")]
  public static void getKing_Postfix(Kingdom __instance)
  {
    if (__instance == null || __instance.king == null)
    {
      return;
    }
    bool tianmingBoolValue;
    __instance.data.get("tianmingbool", out tianmingBoolValue);
    if (tianmingBoolValue)
    {
      string kingName = __instance.king.data.name;
      string kingdomName = __instance.data.name;
      int yeardata = World.world.mapStats.getCurrentYear();

      // 检查字典中是否已经存在这个kingName
      // 如果字典中已存在此kingName，则更新kingdomName的值
      FunctionHelper.TmkingData[kingName] = kingdomName;
      if (!FunctionHelper.YearData.ContainsKey(kingName))
      {
        FunctionHelper.YearData.Add(kingName, yeardata);
      }
    }
  }

  [HarmonyPostfix]
  [HarmonyPatch(typeof(Kingdom), "updateAge")]
  public static void setKing_Postfix(Kingdom __instance)
  {
    if (__instance.king != null && __instance.king.hasTrait("immortal")) { __instance.king.removeTrait("immortal"); }

    if (__instance == null || __instance.king == null)
    {
      return;
    }

    bool tianmingBoolValue;
    __instance.data.get("tianmingbool", out tianmingBoolValue);

    if (tianmingBoolValue && !FunctionHelper.kingsThatHaveLogged.Contains(__instance.king))
    {
      CityTools.logUnite(__instance);
      // 将当前 king 添加到列表中，表示已经执行过 CityTools.logUnite 方法
      FunctionHelper.kingsThatHaveLogged.Add(__instance.king);
    }
    //string kname = __instance.king.getName();
    //int yearm1 =Actionlib.year -1 ;
    int pop = __instance.getPopulationTotal();
    if (__instance.king == null)
    {
      return;
    }


    string kingId = __instance.king.data.id;
    int month = World.world.mapStats.getCurrentMonth();

    int prevCurrentYear = World.world.mapStats._last_year;
    //Debug.Log(prevCurrentYear);
    // 在下一次调用 World.world.mapStats.getCurrentYear() 函数之前
    int currentYear = World.world.mapStats.getCurrentYear();

    if (FunctionHelper.kingYearData.ContainsKey(kingId))
    {
      FunctionHelper.kingYearData[kingId]++; // 如果字典中已经包含了当前 king 的数据，则更新它
      // 更新 year 数据
    }
    else
    {
      // 向字典中添加新的数据
      FunctionHelper.kingYearData.Add(kingId, 1); // 添加 year 数据并将其设置为 1
    }
    if (FunctionHelper.is_chinese)
    {
      if (prevCurrentYear != currentYear && __instance.king.hasTrait("zhuhou") && __instance.data.name.Length >= 6)
      { //prevCurrentYear =currentYear;
        // Debug.Log("年"+ kingYearData[kingId]);
        // 更新特定 kingId 的 year 数据
        //kingYearData[kingId]++;

        int hyphenIndex = __instance.data.name.LastIndexOf('-');
        if (hyphenIndex >= 0)
        {
          // 删除"-"后面的第三个字符（包括第三个字符）以及之后的所有字符
          __instance.data.name = __instance.data.name.Substring(0, hyphenIndex + 3) + FunctionHelper.kingYearData[kingId] + "年";
        }

      }
    }
    /*if (Actionlib.year == 1 )
    {//Debug.Log("元年");
    string lastChar = __instance.data.name.Substring(__instance.data.name.Length - 2);
        if(lastChar == "元年"&& __instance.king != null){
            return;
        }
               __instance.data.name = __instance.data.name.Remove(__instance.data.name.Length - 2)+"元年";



    }*/

  }
}