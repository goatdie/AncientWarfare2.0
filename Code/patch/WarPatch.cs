using System;
using System.Collections.Generic;
using System.Linq;
using Figurebox.Utils;
using HarmonyLib;
namespace Figurebox.patch;

internal class WarPatch
{
  private static readonly Dictionary<War, City> storedCapitals = new();
  private static readonly Dictionary<War, int> initialCityCounts = new();
  private static readonly Dictionary<War, List<City>> storedCities = new();

  [HarmonyPostfix]
  [HarmonyPatch(typeof(WarManager), "update")]
  public static void updatewarPostfix(float pElapsed, WarManager __instance) //有点问题应该让天命战争的所有参战国占领首都都算然后让储存的首都的王国变成main_attacker
  {
    List<War> listCopy = new(__instance.list);
    foreach (War war in listCopy)
    {
      if (war.main_attacker.king != null && war.main_attacker.king.hasTrait("天命") && war._asset == AssetManager.war_types_library.get("normal"))
      {
        if (war.isActive())
        {

          if (!storedCapitals.ContainsKey(war))
          {
            City c = war.main_attacker.capital;
            storedCapitals.Add(war, c);
          }
          // 使用存储的首都进行其他操作
          City storedCapital = storedCapitals[war];
          if (war != null && war.main_defender != null && war.main_defender.king != null && storedCapital != null && storedCapital.kingdom == war.main_defender)
          {
            bool tianmingBoolValue;
            bool tianmingBoolValue2;
            war.main_attacker.data.get("tianmingbool", out tianmingBoolValue);
            war.main_defender.data.get("tianmingbool", out tianmingBoolValue2);
            if (tianmingBoolValue)
            {
              war.main_attacker.data.set("tianmingbool", false);
              CityTools.loglosekingdom(war.main_attacker);
              FunctionHelper.tianmingvalue = 0;
              if (war.main_attacker.king != null)
              {
                CityTools.loglose(war.main_attacker);
                war.main_attacker.king.removeTrait("天命");
                war.main_attacker.king.addTrait("formerking");

              }

            }
            if (!tianmingBoolValue2 && war.main_defender.king != null)
            {
              war.main_defender.data.set("tianmingbool", true);
              //Debug.Log(war.main_defender.name+"tianmingBoolValue: " + tianmingBoolValue);
              //Debug.Log(war.main_attacker.name+"tianmingBoolValue: " + tianmingBoolValue2);
              FunctionHelper.tianmingvalue = 50;
              //CityTools.logUnite(war.main_attacker);
              war.main_defender.king.addTrait("天命");
              war.main_defender.king.addTrait("first");

              List<City> cityListCopy = new(war.main_attacker.cities);
              foreach (City city in cityListCopy)
              {
                if (city != war.main_attacker.capital && city.leader != null && city.neighbours_cities.Contains(storedCapital))
                {
                  city.joinAnotherKingdom(war.main_defender);
                }
              }
              // war.main_attacker.capital.joinAnotherKingdom(war.main_defender);
              //Debug.Log($" 变革天命王国{war.main_defender.name}");
              __instance.endWar(war);
              storedCapitals.Remove(war);
            }

          }

          if (!initialCityCounts.ContainsKey(war))
          {
            // 将初始城市计数添加到字典中
            initialCityCounts[war] = war.main_attacker.countCities();
          }
          else
          {
            // 使用字典中的值进行比较
            int initialCityCount = initialCityCounts[war];
            int currentCityCount = war.main_attacker.countCities();
            if (currentCityCount <= initialCityCount - 2)
            {
              FunctionHelper.notdoingwellinwar = true;
            }
            else
            {
              FunctionHelper.notdoingwellinwar = false;
            }
            // 这里进行比较或其他操作
          }
        }
      }

      if (war._asset != AssetManager.war_types_library.get("spite") &&
          war.main_defender.king != null &&
          war.main_defender.king.hasTrait("天命") &&
          (war._asset == AssetManager.war_types_library.get("normal") || war._asset == AssetManager.war_types_library.get("vassal_war") ||
           war._asset == AssetManager.war_types_library.get("whisper_of_war")))
      {
        war._asset = AssetManager.war_types_library.get("tianming");

      }

      if (war.main_attacker.king != null && war.main_attacker.king.hasTrait("天命") && (war._asset == AssetManager.war_types_library.get("rebellion") || war._asset == AssetManager.war_types_library.get("inspire")))
      {
        war._asset = AssetManager.war_types_library.get("tianmingrebel");
      }
      if (war._asset == AssetManager.war_types_library.get("tianmingrebel"))
      {
        if (!storedCapitals.ContainsKey(war))
        {
          City c = war.main_attacker.capital;
          storedCapitals.Add(war, c);
        }
        // 使用存储的首都进行其他操作
        City storedCapital = storedCapitals[war];


        if (war != null && war.main_defender != null && war.main_defender.king != null && storedCapital != null && storedCapital.kingdom == war.main_defender)
        {
          bool tianmingBoolValue;
          bool tianmingBoolValue2;
          war.main_attacker.data.get("tianmingbool", out tianmingBoolValue);
          war.main_defender.data.get("tianmingbool", out tianmingBoolValue2);
          if (tianmingBoolValue)
          {
            war.main_attacker.data.set("tianmingbool", false);
            CityTools.loglosekingdom(war.main_attacker);
            FunctionHelper.tianmingvalue = 0;
            if (war.main_attacker.king != null)
            {
              CityTools.loglose(war.main_attacker);
              war.main_attacker.king.removeTrait("天命");
              war.main_attacker.king.addTrait("formerking");

            }

          }
          if (!tianmingBoolValue2 && war.main_defender.king != null)
          {
            war.main_defender.data.set("tianmingbool", true);
            //Debug.Log(war.main_defender.name+"tianmingBoolValue: " + tianmingBoolValue);
            //Debug.Log(war.main_attacker.name+"tianmingBoolValue: " + tianmingBoolValue2);
            FunctionHelper.tianmingvalue = 50;
            //CityTools.logUnite(war.main_attacker);
            war.main_defender.king.addTrait("天命");
            war.main_defender.king.addTrait("first");

            List<City> cityListCopy = new(war.main_attacker.cities);
            foreach (City city in cityListCopy)
            {
              if (city != war.main_attacker.capital && city.leader != null && city.neighbours_cities.Contains(storedCapital))
              {
                city.joinAnotherKingdom(war.main_defender);
              }
            }
            // war.main_attacker.capital.joinAnotherKingdom(war.main_defender);
            //Debug.Log($" 变革天命王国{war.main_defender.name}");
            __instance.endWar(war);
            storedCapitals.Remove(war);
          }
        }

      }
      if (war._asset == AssetManager.war_types_library.get("tianming"))
      {
        if (!storedCapitals.ContainsKey(war))
        {
          City c = war.main_defender.capital;
          storedCapitals.Add(war, c);
        }
        // 使用存储的首都进行其他操作
        City storedCapital = storedCapitals[war];
        if (war.main_attacker != null)
        {
          if (war.main_defender == null || storedCapital.kingdom == war.main_attacker)
          {
            bool tianmingBoolValue;
            bool tianmingBoolValue2;
            war.main_attacker.data.get("tianmingbool", out tianmingBoolValue);
            war.main_defender.data.get("tianmingbool", out tianmingBoolValue2);
            if (tianmingBoolValue2)
            {
              war.main_defender.data.set("tianmingbool", false);
              FunctionHelper.tianmingvalue = 0;
              CityTools.loglosekingdom(war.main_defender);
              if (war.main_defender.king != null)
              {
                CityTools.loglose(war.main_defender);
                war.main_defender.king.removeTrait("天命");
                war.main_defender.king.addTrait("formerking");

              }

            }
            if (!tianmingBoolValue)
            {
              war.main_attacker.data.set("tianmingbool", true);
              FunctionHelper.tianmingvalue = 50;
              //CityTools.logUnite(war.main_attacker);
              war.main_attacker.king.addTrait("天命");
              war.main_attacker.king.addTrait("first");
              List<City> cityListCopy = new(war.main_defender.cities);
              foreach (City city in cityListCopy)
              {
                if (city != war.main_defender.capital && city.leader != null && city.neighbours_cities.Contains(war.main_defender.capital))
                {
                  city.joinAnotherKingdom(war.main_attacker);
                }
              }
              // war.main_defender.capital.joinAnotherKingdom(war.main_attacker);
              //Debug.Log($" 变革天命王国{war.main_attacker.name}");
              __instance.endWar(war);
            }

          }
        }

      }
    }


  }


  [HarmonyPostfix]
  [HarmonyPatch(typeof(WarManager), "update")]
  public static void wardata_Postfix(float pElapsed, WarManager __instance)
  {
    List<War> listCopy = new(__instance.list);
    foreach (War war in listCopy)
    {
      string warId = war.data.id;
      // 只有当warIdNameDict不包含给定的战争ID时，才更新字典
      if (!FunctionHelper.warIdNameDict.ContainsKey(warId))
      {
        FunctionHelper.warIdNameDict[warId] = war.data.name;
        FunctionHelper.WarStartDate[warId] = war.data.created_time;
        FunctionHelper.Attackers[warId] = new List<string>(war.data.list_attackers);
        FunctionHelper.Defenders[warId] = new List<string>(war.data.list_defenders);
      }
    }
  }
  [HarmonyPostfix]
  [HarmonyPatch(typeof(WarManager), "endWar")]
  public static void warenddata_Postfix(War pWar, WarManager __instance)
  {
    int yeardata = World.world.mapStats.getCurrentYear();
    double yearfloat = World.world.getCurWorldTime();
    string warId = pWar.data.id;
    // 只有当warIdNameDict不包含给定的战争ID时，才更新字典
    if (!FunctionHelper.WarEndDate.ContainsKey(warId))
    {
      FunctionHelper.WarEndDate[warId] = yeardata;
      FunctionHelper.WarEndDateFloat[warId] = yearfloat;
    }

  }

  [HarmonyPostfix]
  [HarmonyPatch(typeof(WarManager), "update")]
  public static void rebeljoinanotherkingdom_Postfix(float pElapsed, WarManager __instance)
  {
    List<War> listCopy = new(__instance.list);
    foreach (War war in listCopy)
    {
      if (war._asset == AssetManager.war_types_library.get("rebellion") || war._asset == AssetManager.war_types_library.get("inspire"))
      {
        if (war.isActive())
        {
          foreach (Kingdom defender in war.getDefenders())
          {
            Kingdom maxOpinionKingdom = null;
            float maxOpinionValue = float.MinValue;
            if (defender != null && defender.capital != null)
            {
              foreach (Kingdom neighbourKingdom in defender.capital.neighbours_kingdoms)
              {
                if (neighbourKingdom != null)
                {
                  KingdomOpinion opinion;
                  try
                  {
                    opinion = BehaviourActionBase<Kingdom>.world.diplomacy.getOpinion(defender, neighbourKingdom);
                  }
                  catch (NullReferenceException)
                  {
                    continue;
                  }
                  if (opinion != null)
                  {
                    if (defender.getCulture() == neighbourKingdom.getCulture() && opinion.total > maxOpinionValue)
                    {
                      maxOpinionValue = opinion.total;
                      maxOpinionKingdom = neighbourKingdom;
                    }
                  }
                }
              }
            }

            if (maxOpinionKingdom != null)
            {
              if (maxOpinionKingdom.getAlliance() == null || !maxOpinionKingdom.getAlliance().kingdoms_hashset.Contains(war.main_attacker))
              {
                KingdomOpinion opinionBetweenMaxOpinionKingdomAndAttacker;
                try
                {
                  opinionBetweenMaxOpinionKingdomAndAttacker = BehaviourActionBase<Kingdom>.world.diplomacy.getOpinion(maxOpinionKingdom, war.main_attacker);
                }
                catch (NullReferenceException)
                {
                  continue;
                }
                if (opinionBetweenMaxOpinionKingdomAndAttacker != null && opinionBetweenMaxOpinionKingdomAndAttacker.total <= 10)
                {
                  List<City> cityListCopy = new(defender.cities);
                  foreach (City dcity in cityListCopy)
                  {
                    dcity.joinAnotherKingdom(maxOpinionKingdom);
                  }
                  CityTools.logjoinanotherkingdom(defender, maxOpinionKingdom);
                  if (KingdomVassals.IsVassal(maxOpinionKingdom))
                  {
                    Kingdom suzerain = KingdomVassals.GetSuzerain(maxOpinionKingdom);
                    if (war.main_attacker.getArmy() + 30 > KingdomVassals.getSuzerainArmy(suzerain))
                    {
                      World.world.diplomacy.startWar(war.main_attacker, maxOpinionKingdom, AssetManager.war_types_library.get("reclaim"));

                    }
                  }
                  else
                  {
                    if (war.main_attacker.getArmy() + 30 > maxOpinionKingdom.getArmy())
                    {
                      World.world.diplomacy.startWar(war.main_attacker, maxOpinionKingdom, AssetManager.war_types_library.get("reclaim"));

                    }
                  }

                }
              }
            }
          }
        }
      }
    }
  }
  //下次做个移交友军领土的
  [HarmonyPostfix]
  [HarmonyPatch(typeof(WarManager), "update")]
  public static void reclaimendwar_Postfix(float pElapsed, WarManager __instance)
  {

    List<War> listCopy = new(__instance.list);
    foreach (War war in listCopy)
    {
      if (war._asset == AssetManager.war_types_library.get("reclaim"))
      {
        // 储存开始战争时攻击者的城市列表
        if (!storedCities.ContainsKey(war))
        {

          List<City> cityListCopy = new(war.main_attacker.cities);
          storedCities.Add(war, cityListCopy);
        }
        if (!FunctionHelper.storedDefenderCities.ContainsKey(war))
        {
          List<City> cityListCopy = new(war.main_defender.cities);
          FunctionHelper.storedDefenderCities.Add(war, cityListCopy);
        }

        if (war.isActive())
        {
          // 检查是否所有涉及的城市的数据都存在于CityYearData中
          bool allCitiesDataExists = true;
          foreach (var city in war.main_defender.cities)
          {
            if (!FunctionHelper.CityYearData.ContainsKey(city.data.id))
            {
              allCitiesDataExists = false;
              break;
            }
          }

          // 如果所有涉及的城市的数据都存在于CityYearData中
          if (allCitiesDataExists)
          {
            foreach (Kingdom attacker in war.getAttackers())
            {
              if (attacker != war.main_attacker)
              {
                foreach (var city in FunctionHelper.storedDefenderCities[war])
                {
                  if (city.kingdom == attacker)
                  {
                    city.joinAnotherKingdom(war.main_attacker);


                    // Remove the occupation data of the attacker
                    string cityId = city.data.id;
                    string attackerId = attacker.data.id;

                    if (FunctionHelper.CityYearData.ContainsKey(cityId))
                    {
                      // Find all occupation entries by the attacker in this city
                      var attackerEntries = FunctionHelper.CityYearData[cityId].Where(entry => entry.Key.StartsWith(attackerId + "-")).ToList();

                      foreach (var entry in attackerEntries)
                      {
                        // Remove this entry from CityYearData
                        FunctionHelper.CityYearData[cityId].Remove(entry.Key);
                      }
                    }
                  }
                }

              }
            }

            // 如果攻击者已经重新占领了所有最近的领土
            if (FunctionHelper.HasRecapturedAllRecentTerritories(war)) //如果友军占领了这个土地 直接把土地交过去
            {
              int compensation = war.getDeadAttackers() * 10;
              if (war.main_attacker.capital != null && war.main_defender.capital != null)
              {
                war.main_attacker.capital.data.storage.change("gold", compensation);

                war.main_defender.capital.data.storage.change("gold", -compensation);

                __instance.endWar(war);
                CityTools.logreclaim(war.main_defender, war.main_attacker, war.main_attacker);
              }
            }
            else
            {
              // 检查防御者是否占领了攻击者的两个或更多的城市
              int defenderCityCaptureCount = 0;
              foreach (var city in storedCities[war])
              {
                if (city.kingdom == war.main_defender)
                {
                  defenderCityCaptureCount++;
                  if (defenderCityCaptureCount >= 2)
                  {
                    break;
                  }
                }
              }

              // 如果防御者占领了攻击者的任何一块土地
              if (defenderCityCaptureCount >= 2)
              {
                // 计算赔款金额，比如每死一个防守者，攻击者要赔款10金币
                int compensation = war.getDeadDefenders() * 10;
                // 从攻击者的金币中扣除赔款
                if (war.main_attacker.capital != null && war.main_defender.capital != null)
                {
                  war.main_attacker.capital.data.storage.change("gold", -compensation);
                  // 将赔款给予防守者
                  war.main_defender.capital.data.storage.change("gold", compensation);
                  // 结束战争，攻击者失败
                  __instance.endWar(war);
                  CityTools.logreclaim(war.main_defender, war.main_attacker, war.main_defender);
                }

              }
            }
          }
        }
      }
    }
  }
}