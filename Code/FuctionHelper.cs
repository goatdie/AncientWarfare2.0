using UnityEngine;
using HarmonyLib;
using ReflectionUtility;
using Figurebox.Utils;
using NCMS.Utils;
using ai;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Reflection;
using Figurebox;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;
using System.ComponentModel;
using System.Globalization;

namespace Figurebox
{
    class FunctionHelper           //失去天命后首都周围地减低轻微忠诚度，飞地直接独立或者加入其他国家 天命减少 国王的氏族绝嗣,（没有开国之君特质) 收复战争得先判断双方实力
    {
        public static FunctionHelper instance;
        public static Dictionary<string, int> kingYearData = new Dictionary<string, int>();
        public static Dictionary<string, int> YearData = new Dictionary<string, int>();
        public static Dictionary<string, string> TmkingData = new Dictionary<string, string>();
        public static Dictionary<string, string> kingdomYearNameData = new Dictionary<string, string>();
        public static Dictionary<string, int> kingdomVassalEstablishmentTime = new Dictionary<string, int>();
        public static Dictionary<string, int> kingdomVassalEndTime = new Dictionary<string, int>();



        public static int tianmingvalue = 0;
        public static bool is_chinese = false;
        public static bool notdoingwellinwar = false;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(WorldLogMessageExtensions), "getFormatedText")]
        public static void getFormatedText(ref string __result, ref WorldLogMessage pMessage)
        {

            switch (pMessage.text)
            {
                case "baseLog":
                    __result = Localization.getLocalization(pMessage.text);
                    break;
                case "historicalMessage":
                    string text = Localization.getLocalization(pMessage.text);
                    if (pMessage.unit == null)
                    {
                        pMessage.icon = "iconDeathMark";
                        text = text.Replace("$ren$", string.Concat(new string[] { pMessage.special1 }));
                        __result = text;
                        return;
                    }
                    text = text.Replace("$ren$", string.Concat(new string[] { "<color=", Toolbox.colorToHex(pMessage.color_special1, true), ">", pMessage.unit.getName(), "</color>" }));
                    pMessage.icon = "iconDocument";
                    __result = text;
                    break;
                case "mandateofheavenMessage":
                    text = Localization.getLocalization(pMessage.text);
                    if (pMessage.unit == null)
                    {
                        pMessage.icon = "iconDead";
                        text = text.Replace("$king$", string.Concat(new string[] { pMessage.special2 }));
                        text = text.Replace("$kingdom$", string.Concat(new string[] { pMessage.special1 }));
                        __result = text;
                        return;
                    }
                    text = text.Replace("$king$", string.Concat(new string[] { "<color=", Toolbox.colorToHex(pMessage.color_special1, true), ">", pMessage.unit.GetData().name, "</color>" }));
                    text = text.Replace("$kingdom$", string.Concat(new string[] { "<color=", Toolbox.colorToHex(pMessage.color_special1, true), ">", pMessage.kingdom.name, "</color>" }));
                    pMessage.icon = "iconKingdom";
                    __result = text;
                    break;
                case "losemandateofheavenMessage":
                    text = Localization.getLocalization(pMessage.text);
                    if (pMessage.unit == null)
                    {
                        pMessage.icon = "iconDead";
                        text = text.Replace("$king$", string.Concat(new string[] { pMessage.special2 }));
                        text = text.Replace("$kingdom$", string.Concat(new string[] { pMessage.special1 }));
                        __result = text;
                        return;
                    }
                    text = text.Replace("$king$", string.Concat(new string[] { "<color=", Toolbox.colorToHex(pMessage.color_special1, true), ">", pMessage.unit.GetData().name, "</color>" }));
                    text = text.Replace("$kingdom$", string.Concat(new string[] { "<color=", Toolbox.colorToHex(pMessage.color_special1, true), ">", pMessage.kingdom.name, "</color>" }));
                    pMessage.icon = "iconDocument";
                    __result = text;
                    break;
                case "warmandateofheavenMessage":
                    text = Localization.getLocalization(pMessage.text);
                    text = text.Replace("$kingdom$", string.Concat(new string[] { "<color=", Toolbox.colorToHex(pMessage.color_special1, true), ">", pMessage.special2, "</color>" }));
                    text = text.Replace("$kingdom2$", string.Concat(new string[] { "<color=", Toolbox.colorToHex(pMessage.color_special2, true), ">", pMessage.special1, "</color>" }));
                    pMessage.icon = "iconDocument";
                    __result = text;
                    break;
                case "losekingdommandateofheavenMessage":
                    text = Localization.getLocalization(pMessage.text);
                    text = text.Replace("$kingdom$", string.Concat(new string[] { "<color=", Toolbox.colorToHex(pMessage.color_special1, true), ">", pMessage.special1, "</color>" }));
                    pMessage.icon = "iconDocument";
                    __result = text;
                    break;
                case "joinanotherkingdomMessage":
                    text = Localization.getLocalization(pMessage.text);

                    text = text.Replace("$kingdom$", string.Concat(new string[] { "<color=", Toolbox.colorToHex(pMessage.color_special1, true), ">", pMessage.special2, "</color>" }));
                    text = text.Replace("$kingdom2$", string.Concat(new string[] { "<color=", Toolbox.colorToHex(pMessage.color_special2, true), ">", pMessage.special1, "</color>" }));
                    pMessage.icon = "iconDocument";
                    __result = text;
                    break;
                case "reclaimwarendMessage":
                    text = Localization.getLocalization(pMessage.text);
                    text = text.Replace("$kingdom$", string.Concat(new string[] { "<color=", Toolbox.colorToHex(pMessage.color_special1, true), ">", pMessage.special2, "</color>" }));
                    text = text.Replace("$kingdom2$", string.Concat(new string[] { "<color=", Toolbox.colorToHex(pMessage.color_special2, true), ">", pMessage.special1, "</color>" }));
                    text = text.Replace("$winner$", string.Concat(new string[] { "<color=", Toolbox.colorToHex(pMessage.color_special3, true), ">", pMessage.special3, "</color>" }));
                    pMessage.icon = "iconDocument";
                    __result = text;
                    break;
                case "usurpationMessage":
                    text = Localization.getLocalization(pMessage.text);
                    text = text.Replace("$kingdom$", string.Concat(new string[] { "<color=", Toolbox.colorToHex(pMessage.color_special1, true), ">", pMessage.special2, "</color>" }));
                    text = text.Replace("$first$", string.Concat(new string[] { "<color=", Toolbox.colorToHex(pMessage.color_special1, true), ">", pMessage.special1, "</color>" }));
                    pMessage.icon = "iconDocument";
                    __result = text;
                    break;
                case "vassalWarStartMessage":
                    text = Localization.getLocalization(pMessage.text);
                    text = text.Replace("$first$", string.Concat(new string[] { "<color=", Toolbox.colorToHex(pMessage.color_special1, true), ">", pMessage.special2, "</color>" }));
                    text = text.Replace("$kingdom2$", string.Concat(new string[] { "<color=", Toolbox.colorToHex(pMessage.color_special2, true), ">", pMessage.special1, "</color>" }));
                    pMessage.icon = "iconDocument"; // 修改为你想要的图标
                    __result = text;
                    break;
                case "vassalWarEndMessage":
                    text = Localization.getLocalization(pMessage.text);
                    text = text.Replace("$first$", string.Concat(new string[] { "<color=", Toolbox.colorToHex(pMessage.color_special1, true), ">", pMessage.special2, "</color>" }));
                    text = text.Replace("$kingdom2$", string.Concat(new string[] { "<color=", Toolbox.colorToHex(pMessage.color_special2, true), ">", pMessage.special1, "</color>" }));
                    pMessage.icon = "iconDocument"; // 修改为你想要的图标
                    __result = text;
                    break;
                case "IndependenceWarMessage":
                    text = Localization.getLocalization(pMessage.text);
                    text = text.Replace("$kingdom$", string.Concat(new string[] { "<color=", Toolbox.colorToHex(pMessage.color_special1, true), ">", pMessage.special2, "</color>" }));
                    text = text.Replace("$kingdom2$", string.Concat(new string[] { "<color=", Toolbox.colorToHex(pMessage.color_special2, true), ">", pMessage.special1, "</color>" }));
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
        [HarmonyPatch(typeof(Actor), "setProfession")]
        public static void setKing_Postfix(Actor __instance, UnitProfession pType)
        {
            is_chinese = LocalizedTextManager.instance.language == "cz" || LocalizedTextManager.instance.language == "ch";
            if (is_chinese)
            {
                string[] array1 = new string[] { "洪", "建", "永", "弘", "正", "景", "康", "嘉", "兴", "明", "平", "盛", "隆" };
                string[] array2 = new string[] { "武", "文", "乐", "熙", "治", "德", "泰", "运", "阳", "靖", "乾", "庆" };
                //var list2 = new List<string> { "", "", "" };


                var result = GetRandomStrings(array1, array2);


                if (pType == UnitProfession.King && !__instance.hasTrait("zhuhou"))
                {
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
                    if (__instance != null && __instance.city != null && __instance.kingdom.capital != null)
                    {
                        __instance.city = __instance.kingdom.capital;
                        string kingId = __instance.data.id;
                        if (kingdomYearNameData.ContainsKey(kingId))
                        {
                            kingdomYearNameData[kingId] = result;
                        }
                        else
                        {
                            kingdomYearNameData.Add(kingId, result);
                        }

                        // Use result from the kingdomYearNameData dictionary
                        string yearName = kingdomYearNameData[kingId];
                        int dashIndex = __instance.kingdom.data.name.IndexOf("-");
                        if (dashIndex >= 0)
                        {
                            __instance.kingdom.data.name = __instance.kingdom.data.name.Substring(0, dashIndex);
                        }

                        __instance.kingdom.data.name = __instance.kingdom.data.name + "-" + yearName + "元年";
                    }
                }

                /*if (pType == UnitProfession.Unit)
               {
                   __instance.removeTrait("zhuhou");

               }*/

            }
        }

        static List<Actor> kingsThatHaveLogged = new List<Actor>();
        //static int Actionlib.year = 1;

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
                if (!TmkingData.ContainsKey(kingName))
                {
                    TmkingData.Add(kingName, kingdomName);
                }
                else
                {
                    // 如果字典中已存在此kingName，则更新kingdomName的值
                    TmkingData[kingName] = kingdomName;
                }
                if (!YearData.ContainsKey(kingName))
                {
                    YearData.Add(kingName, yeardata);
                }
                else
                {
                    // 如果字典中已存在此kingName，则不更新 yeardata的值
                    //YearData[kingName] = kingdomName;
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

            if (tianmingBoolValue && !kingsThatHaveLogged.Contains(__instance.king))
            {
                CityTools.logUnite(__instance);
                // 将当前 king 添加到列表中，表示已经执行过 CityTools.logUnite 方法
                kingsThatHaveLogged.Add(__instance.king);
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

            if (kingYearData.ContainsKey(kingId))
            {
                kingYearData[kingId]++;                                    // 如果字典中已经包含了当前 king 的数据，则更新它
                                                                           // 更新 year 数据
            }
            else
            {
                // 向字典中添加新的数据
                kingYearData.Add(kingId, 1); // 添加 year 数据并将其设置为 1
            }
            is_chinese = LocalizedTextManager.instance.language == "cz" || LocalizedTextManager.instance.language == "ch";
            if (is_chinese)
            {
                if (prevCurrentYear != currentYear && __instance.king.hasTrait("zhuhou") && __instance.data.name.Length >= 6)
                {//prevCurrentYear =currentYear;
                 // Debug.Log("年"+ kingYearData[kingId]);
                 // 更新特定 kingId 的 year 数据
                 //kingYearData[kingId]++;

                    int hyphenIndex = __instance.data.name.LastIndexOf('-');
                    if (hyphenIndex >= 0)
                    {
                        // 删除"-"后面的第三个字符（包括第三个字符）以及之后的所有字符
                        __instance.data.name = __instance.data.name.Substring(0, hyphenIndex + 3) + kingYearData[kingId] + "年";
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

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Actor), "setProfession")]
        public static void setNewCapital_Postfix(Actor __instance, UnitProfession pType)
        {
            if (pType == UnitProfession.King)
            {
                bool tianmingBoolValue;
                __instance.kingdom.data.get("tianmingbool", out tianmingBoolValue);
                if (tianmingBoolValue) { __instance.addTrait("天命"); }

                if (__instance.kingdom.getEnemiesKingdoms().Count == 0 && __instance.kingdom.capital != null)
                {
                    City newCapital = __instance.kingdom.cities
                        .Select(city =>
                        {
                            double score = (city.data.age - __instance.kingdom.capital.data.age) * 1 +
                                           (city.getPopulationTotal() - __instance.kingdom.capital.getPopulationTotal()) * 2 +
                                           (city.zones.Count - __instance.kingdom.capital.zones.Count) * 0.35 +
                                           (city.neighbours_cities.SetEquals(city.neighbours_cities_kingdom) ? 50 : 0);


                            Debug.Log($"City: {city.data.name}, Score: {score}");
                            return new { City = city, Score = score };
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
        }


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



        // ...



        private static Dictionary<War, City> storedCapitals = new Dictionary<War, City>();
        private static Dictionary<War, int> initialCityCounts = new Dictionary<War, int>();


        [HarmonyPostfix]
        [HarmonyPatch(typeof(WarManager), "update")]
        public static void updatewarPostfix(float pElapsed, WarManager __instance) //有点问题应该让天命战争的所有参战国占领首都都算然后让储存的首都的王国变成main_attacker
        {
            List<War> listCopy = new List<War>(__instance.list);
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
                                tianmingvalue = 0;
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
                                tianmingvalue = 50;
                                //CityTools.logUnite(war.main_attacker);
                                war.main_defender.king.addTrait("天命");
                                war.main_defender.king.addTrait("first");

                                List<City> cityListCopy = new List<City>(war.main_attacker.cities);
                                foreach (City city in cityListCopy)
                                {
                                    if (city != war.main_attacker.capital && city.leader != null && city.neighbours_cities.Contains(storedCapital))
                                    {
                                        city.joinAnotherKingdom(war.main_defender);
                                    }
                                }
                                // war.main_attacker.capital.joinAnotherKingdom(war.main_defender);
                                //Debug.Log($" 变革天命王国{war.main_defender.name}");
                                __instance.endWar(war, true);
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
                                notdoingwellinwar = true;
                            }
                            else
                            {
                                notdoingwellinwar = false;
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
                            tianmingvalue = 0;
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
                            tianmingvalue = 50;
                            //CityTools.logUnite(war.main_attacker);
                            war.main_defender.king.addTrait("天命");
                            war.main_defender.king.addTrait("first");

                            List<City> cityListCopy = new List<City>(war.main_attacker.cities);
                            foreach (City city in cityListCopy)
                            {
                                if (city != war.main_attacker.capital && city.leader != null && city.neighbours_cities.Contains(storedCapital))
                                {
                                    city.joinAnotherKingdom(war.main_defender);
                                }
                            }
                            // war.main_attacker.capital.joinAnotherKingdom(war.main_defender);
                            //Debug.Log($" 变革天命王国{war.main_defender.name}");
                            __instance.endWar(war, true);
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
                        if ((war.main_defender == null || storedCapital.kingdom == war.main_attacker))
                        {
                            bool tianmingBoolValue;
                            bool tianmingBoolValue2;
                            war.main_attacker.data.get("tianmingbool", out tianmingBoolValue);
                            war.main_defender.data.get("tianmingbool", out tianmingBoolValue2);
                            if (tianmingBoolValue2)
                            {
                                war.main_defender.data.set("tianmingbool", false);
                                tianmingvalue = 0;
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
                                tianmingvalue = 50;
                                //CityTools.logUnite(war.main_attacker);
                                war.main_attacker.king.addTrait("天命");
                                war.main_attacker.king.addTrait("first");
                                List<City> cityListCopy = new List<City>(war.main_defender.cities);
                                foreach (City city in cityListCopy)
                                {
                                    if (city != war.main_defender.capital && city.leader != null && city.neighbours_cities.Contains(war.main_defender.capital))
                                    {
                                        city.joinAnotherKingdom(war.main_attacker);
                                    }
                                }
                                // war.main_defender.capital.joinAnotherKingdom(war.main_attacker);
                                //Debug.Log($" 变革天命王国{war.main_attacker.name}");
                                __instance.endWar(war, true);
                            }

                        }
                    }

                }
            }


        }
        // 创建一个Dictionary用于保存War Id和Name
        public static Dictionary<string, string> warIdNameDict = new Dictionary<string, string>();
        public static Dictionary<string, double> WarStartDate = new Dictionary<string, double>();
        public static Dictionary<string, List<string>> Attackers = new Dictionary<string, List<string>>();
        public static Dictionary<string, List<string>> Defenders = new Dictionary<string, List<string>>();
        public static Dictionary<string, int> WarEndDate = new Dictionary<string, int>();
        public static Dictionary<string, double> WarEndDateFloat = new Dictionary<string, double>();


        [HarmonyPostfix]
        [HarmonyPatch(typeof(WarManager), "update")]
        public static void wardata_Postfix(float pElapsed, WarManager __instance)
        {
            List<War> listCopy = new List<War>(__instance.list);
            foreach (War war in listCopy)
            {
                string warId = war.data.id;
                // 只有当warIdNameDict不包含给定的战争ID时，才更新字典
                if (!warIdNameDict.ContainsKey(warId))
                {
                    warIdNameDict[warId] = war.data.name;
                    WarStartDate[warId] = war.data.created_time;
                    Attackers[warId] = new List<string>(war.data.list_attackers);
                    Defenders[warId] = new List<string>(war.data.list_defenders);
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
            if (!WarEndDate.ContainsKey(warId))
            {
                WarEndDate[warId] = yeardata;
                WarEndDateFloat[warId] = yearfloat;
            }

        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(WarManager), "update")]
        public static void rebeljoinanotherkingdom_Postfix(float pElapsed, WarManager __instance)
        {
            List<War> listCopy = new List<War>(__instance.list);
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
                                if ((maxOpinionKingdom.getAlliance() == null || !maxOpinionKingdom.getAlliance().kingdoms_hashset.Contains(war.main_attacker)))
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
                                        List<City> cityListCopy = new List<City>(defender.cities);
                                        foreach (City dcity in cityListCopy)
                                        {
                                            dcity.joinAnotherKingdom(maxOpinionKingdom);
                                        }
                                        CityTools.logjoinanotherkingdom(defender, maxOpinionKingdom);
                                        if (KingdomVassals.IsVassal(maxOpinionKingdom))
                                        {
                                            Kingdom suzerain = KingdomVassals.GetKingdom(maxOpinionKingdom);
                                            if (war.main_attacker.getArmy() + 30 > KingdomVassals.getSuzerainArmy(suzerain))
                                            {
                                                World.world.diplomacy.startWar(war.main_attacker, maxOpinionKingdom, AssetManager.war_types_library.get("reclaim"), true);

                                            }
                                        }
                                        else
                                        {
                                            if (war.main_attacker.getArmy() + 30 > maxOpinionKingdom.getArmy())
                                            {
                                                World.world.diplomacy.startWar(war.main_attacker, maxOpinionKingdom, AssetManager.war_types_library.get("reclaim"), true);

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


        private static Dictionary<War, List<City>> storedCities = new Dictionary<War, List<City>>();
        public static Dictionary<War, List<City>> storedDefenderCities = new Dictionary<War, List<City>>();
        //下次做个移交友军领土的
        [HarmonyPostfix]
        [HarmonyPatch(typeof(WarManager), "update")]
        public static void reclaimendwar_Postfix(float pElapsed, WarManager __instance)
        {

            List<War> listCopy = new List<War>(__instance.list);
            foreach (War war in listCopy)
            {
                if (war._asset == AssetManager.war_types_library.get("reclaim"))
                {
                    // 储存开始战争时攻击者的城市列表
                    if (!storedCities.ContainsKey(war))
                    {

                        List<City> cityListCopy = new List<City>(war.main_attacker.cities);
                        storedCities.Add(war, cityListCopy);
                    }
                    if (!storedDefenderCities.ContainsKey(war))
                    {
                        List<City> cityListCopy = new List<City>(war.main_defender.cities);
                        storedDefenderCities.Add(war, cityListCopy);
                    }

                    if (war.isActive())
                    {
                        // 检查是否所有涉及的城市的数据都存在于CityYearData中
                        bool allCitiesDataExists = true;
                        foreach (var city in war.main_defender.cities)
                        {
                            if (!CityYearData.ContainsKey(city.data.id))
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
                                    foreach (var city in storedDefenderCities[war])
                                    {
                                        if (city.kingdom == attacker)
                                        {
                                            city.joinAnotherKingdom(war.main_attacker);


                                            // Remove the occupation data of the attacker
                                            string cityId = city.data.id;
                                            string attackerId = attacker.data.id;

                                            if (CityYearData.ContainsKey(cityId))
                                            {
                                                // Find all occupation entries by the attacker in this city
                                                var attackerEntries = CityYearData[cityId].Where(entry => entry.Key.StartsWith(attackerId + "-")).ToList();

                                                foreach (var entry in attackerEntries)
                                                {
                                                    // Remove this entry from CityYearData
                                                    CityYearData[cityId].Remove(entry.Key);
                                                }
                                            }
                                        }
                                    }

                                }
                            }

                            // 如果攻击者已经重新占领了所有最近的领土
                            if (HasRecapturedAllRecentTerritories(war)) //如果友军占领了这个土地 直接把土地交过去
                            {
                                int compensation = war.getDeadAttackers() * 10;
                                if (war.main_attacker.capital != null && war.main_defender.capital != null)
                                {
                                    war.main_attacker.capital.data.storage.change("gold", compensation);

                                    war.main_defender.capital.data.storage.change("gold", -compensation);

                                    __instance.endWar(war, true);
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
                                        __instance.endWar(war, true);
                                        CityTools.logreclaim(war.main_defender, war.main_attacker, war.main_defender);
                                    }

                                }
                            }
                        }
                    }
                }
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







        public static Dictionary<string, string> kingdomCityData = new Dictionary<string, string>();
        public static Dictionary<string, string> kingdomCityNameyData = new Dictionary<string, string>();
        public static Dictionary<string, Dictionary<string, Tuple<int, int, int>>> CityYearData = new Dictionary<string, Dictionary<string, Tuple<int, int, int>>>();
        public static Dictionary<string, int> KingStartYearInKingdom = new Dictionary<string, int>();
        public static Dictionary<string, List<string>> KingKingdomName = new Dictionary<string, List<string>>();
        public static Dictionary<string, string> KingName = new Dictionary<string, string>();
        public static Dictionary<string, List<string>> KingKingdoms = new Dictionary<string, List<string>>();
        public static List<string> kingdomids = new List<string>();

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Actor), "setProfession")]
        public static void setKinghistory_Postfix(Actor __instance, UnitProfession pType)
        {
            if (pType == UnitProfession.King)
            {
                string kingId = __instance.data.id;
                string kingdomId = __instance.kingdom.data.id;
                int yeardata = World.world.mapStats.getCurrentYear();
                string kingdomname = __instance.kingdom.data.name;
                int count = 1;
                if (kingdomId != "nomads_" + __instance.asset.race)
                {
                    if (!KingStartYearInKingdom.ContainsKey($"{kingId}-{kingdomId}-{count}"))
                    {
                        // 这是国王在这个王国的第一次上位，直接记录
                        string kingAndKingdom = $"{kingId}-{kingdomId}-{count}";
                        KingStartYearInKingdom[kingAndKingdom] = yeardata;
                    }
                    while (KingEndYearInKingdom.ContainsKey($"{kingId}-{kingdomId}-{count}"))
                    {
                        count++;
                    }
                    if (KingEndYearInKingdom.ContainsKey($"{kingId}-{kingdomId}-{count}"))
                    {
                        // 这个国王在这个王国已经退位过，他的第二次上位是真实的

                        string kingAndKingdom = $"{kingId}-{kingdomId}-{count}";
                        KingStartYearInKingdom[kingAndKingdom] = yeardata;
                    }
                }


                if (!KingName.ContainsKey(__instance.data.id) && !KingKingdomName.ContainsKey(kingdomname) && !KingKingdoms.ContainsKey(__instance.data.id))
                {

                    KingName.Add(__instance.data.id, __instance.getName());
                    KingKingdomName[__instance.data.id] = new List<string>();
                    KingKingdoms[__instance.data.id] = new List<string>();
                }
                KingKingdoms[__instance.data.id].Add(kingdomId);
                KingKingdomName[__instance.data.id].Add(kingdomname);
                if (kingYearData.ContainsKey(__instance.data.id))
                {
                    kingYearData[__instance.data.id]++;                                    // 如果字典中已经包含了当前 king 的数据，则更新它
                                                                                           // 更新 year 数据
                }
                else
                {
                    // 向字典中添加新的数据
                    kingYearData.Add(__instance.data.id, 1); // 添加 year 数据并将其设置为 1
                }
                if (!kingdomids.Contains(__instance.kingdom.data.id))
                {
                    kingdomids.Add(__instance.kingdom.data.id);

                }
            }
        }
        public static Dictionary<string, int> KingEndYearInKingdom = new Dictionary<string, int>();

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
                while (KingEndYearInKingdom.ContainsKey(kingId + "-" + kingdomId + "-" + count))
                {
                    count++;
                }

                string kingAndKingdom = kingId + "-" + kingdomId + "-" + count;
                KingEndYearInKingdom[kingAndKingdom] = yeardata;
            }
            return true;
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
                while (KingEndYearInKingdom.ContainsKey(kingId + "-" + kingdomId + "-" + count))
                {
                    count++;
                }

                string kingAndKingdom = kingId + "-" + kingdomId + "-" + count;
                KingEndYearInKingdom[kingAndKingdom] = yeardata;
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
                { combinedKey, new Tuple<int, int, int>(recaptureCount, yeardata, 0) }
            };
                }
            }
            else if (kingdomCityData[cityId] != kingdomId)
            {
                kingdomCityData[cityId] = kingdomId;
                kingdomCityNameyData[kingdomId] = __instance.kingdom.data.name;

                // 查找当前收复次数
                if (CityYearData.ContainsKey(cityId))
                {
                    recaptureCount = CityYearData[cityId].Where(entry => entry.Key.StartsWith(kingdomId + "-")).Count() + 1;
                }
                else
                {
                    CityYearData[cityId] = new Dictionary<string, Tuple<int, int, int>>();
                    recaptureCount = 1;
                }

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










        [HarmonyPrefix]
        [HarmonyPatch(typeof(CityWindow), "OnEnable")]
        public static bool cityOnEnable_Prefix(CityWindow __instance)
        {
            CityHistoryWindow.currentCity = __instance.city;
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(KingdomWindow), "OnEnable")]
        public static bool KingdomOnEnable_Prefix(KingdomWindow __instance)
        {
            KingdomHistoryWindow.currentKingdom = __instance.kingdom;
            return true;
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

        [HarmonyPostfix]
        [HarmonyPatch(typeof(SaveManager), "saveToCurrentPath")]
        public static void SaveMapDataPostfix()
        {
            Debug.Log("保存自定义数据");
            string savePath = SaveManager.currentSavePath;
            string customDataPath = Path.Combine(savePath, "customData.json");
            string warDataSavePath = Path.Combine(savePath, "WarDataSave.json");


            CustomData customData = new CustomData()
            {
                DkingYearData = kingYearData,
                DYearData = YearData,
                DTmkingData = TmkingData,
                Dtianmingvalue = tianmingvalue,
                DkingdomYearNameData = kingdomYearNameData,
                DkingdomCityData = kingdomCityData,
                DkingdomCityNameyData = kingdomCityNameyData,
                DCityYearData = CityYearData,
                DKingStartYearInKingdom = KingStartYearInKingdom,
                DKingName = KingName,
                DKingKingdomName = KingKingdomName,
                DKingKingdoms = KingKingdoms,
                Dkingdomids = kingdomids,
                DKingEndYearInKingdom = KingEndYearInKingdom,
                DkingdomVassalEstablishmentTime = kingdomVassalEstablishmentTime,
                DkingdomVassalEndTime = kingdomVassalEndTime



            };
            WarDataSave warDataSave = new WarDataSave
            {
                DwarIdNameDict = warIdNameDict,
                DWarStartDate = WarStartDate,// 将 war 的开始日期数据赋值到 WarStartDate，
                DAttackers = Attackers,// 将攻击者数据赋值到 Attackers，
                DDefenders = Defenders,
                DWarEndDate = WarEndDate,
                DWarEndDateFloat = WarEndDateFloat

            };
            File.WriteAllText(warDataSavePath, JsonConvert.SerializeObject(warDataSave, Formatting.Indented));



            //settings.StringEscapeHandling = StringEscapeHandling.EscapeNonAscii; // 添加这一行


            File.WriteAllText(customDataPath, JsonConvert.SerializeObject(customData, Formatting.Indented));

            string backupFilePath = Path.Combine(savePath, "customData_backup.json");
            File.Copy(customDataPath, backupFilePath, true);
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MapBox), "generateNewMap")]
        public static void createMapPostfix()
        {

            cleanData();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(SaveManager), "loadData")]
        public static void LoadDataPostfix(SavedMap pData)
        {
            Debug.Log("加载自定义数据");

            int curslot = SaveManager.currentSlot + 1;
            string savePath = SaveManager.currentSavePath;
            Debug.Log(savePath);

            string customDataPath = Path.Combine(savePath, "customData.json");
            string warDataSavePath = Path.Combine(savePath, "WarDataSave.json");


            if (File.Exists(warDataSavePath))
            {
                string jsonData = File.ReadAllText(warDataSavePath);

                // 从 JSON 数据反序列化出 warDataSave 对象
                WarDataSave warDataSave = JsonConvert.DeserializeObject<WarDataSave>(jsonData);

                WarStartDate = warDataSave.DWarStartDate;
                Attackers = warDataSave.DAttackers;
                Defenders = warDataSave.DDefenders;
                warIdNameDict = warDataSave.DwarIdNameDict;
                WarEndDateFloat = warDataSave.DWarEndDateFloat;
                WarEndDate = warDataSave.DWarEndDate;
            }

            if (File.Exists(customDataPath))
            {
                string jsonData = File.ReadAllText(customDataPath);


                // settings.Converters.Add(new CityYearKeyTypeConverter());

                CustomData customData = JsonConvert.DeserializeObject<CustomData>(jsonData);
                cleanData();
                kingYearData = customData.DkingYearData;
                YearData = customData.DYearData;
                TmkingData = customData.DTmkingData;
                tianmingvalue = customData.Dtianmingvalue;
                kingdomYearNameData = customData.DkingdomYearNameData;
                kingdomCityData = customData.DkingdomCityData;
                kingdomCityNameyData = customData.DkingdomCityNameyData;
                CityYearData = customData.DCityYearData;
                KingStartYearInKingdom = customData.DKingStartYearInKingdom;
                KingName = customData.DKingName;
                KingKingdomName = customData.DKingKingdomName;
                KingKingdoms = customData.DKingKingdoms;
                kingdomids = customData.Dkingdomids;
                KingEndYearInKingdom = customData.DKingEndYearInKingdom;
                kingdomVassalEstablishmentTime = customData.DkingdomVassalEstablishmentTime;
                kingdomVassalEndTime = customData.DkingdomVassalEndTime;



            }

        }


        public static void cleanData()
        {
            Debug.Log("清理数据");
            kingYearData.Clear();
            YearData.Clear();
            TmkingData.Clear();
            tianmingvalue = 0;
            kingdomYearNameData.Clear();
            kingdomCityData.Clear();
            kingdomCityNameyData.Clear();
            CityYearData.Clear();
            KingName.Clear();
            KingStartYearInKingdom.Clear();
            KingKingdomName.Clear();
            KingKingdoms.Clear();
            kingdomids.Clear();
            warIdNameDict.Clear();
            WarStartDate.Clear();
            Attackers.Clear();
            Defenders.Clear();
            WarEndDateFloat.Clear();
            WarEndDate.Clear();
            KingEndYearInKingdom.Clear();
            kingdomVassalEstablishmentTime.Clear();
            kingdomVassalEndTime.Clear();
            KingdomVassals.bannerLoaders.Clear();

            foreach (var Toggleitem in PowerButtons.ToggleValues.ToList())
            {
                if (PowerButtons.GetToggleValue(Toggleitem.Key))
                {

                    PowerButtons.ToggleButton(Toggleitem.Key);
                }
            }
        }

        public static string GetRandomStrings(string[] array1, string[] array2)
        {
            var rand = new System.Random();
            var result = "";

            var index1 = rand.Next(array1.Length);
            result += array1[index1];

            var index2 = rand.Next(array2.Length);
            result += array2[index2];

            return result;
        }



    }
    class CustomData
    {
        public Dictionary<string, int> DkingYearData = new Dictionary<string, int>();
        public Dictionary<string, int> DYearData = new Dictionary<string, int>();
        public Dictionary<string, string> DTmkingData = new Dictionary<string, string>();
        public Dictionary<string, string> DkingdomYearNameData = new Dictionary<string, string>();
        public Dictionary<string, string> DkingdomCityData = new Dictionary<string, string>();
        public Dictionary<string, string> DkingdomCityNameyData = new Dictionary<string, string>();
        //[JsonConverter(typeof(CityYearKeyDictionaryConverter))]
        public Dictionary<string, Dictionary<string, Tuple<int, int, int>>> DCityYearData { get; set; }
        public Dictionary<string, int> DKingStartYearInKingdom = new Dictionary<string, int>();
        public Dictionary<string, string> DKingName { get; set; }
        public Dictionary<string, List<string>> DKingKingdoms = new Dictionary<string, List<string>>();
        public Dictionary<string, List<string>> DKingKingdomName = new Dictionary<string, List<string>>();
        public List<string> Dkingdomids = new List<string>();
        public Dictionary<string, int> DKingEndYearInKingdom = new Dictionary<string, int>();
        public Dictionary<string, int> DkingdomVassalEstablishmentTime = new Dictionary<string, int>();
        public Dictionary<string, int> DkingdomVassalEndTime = new Dictionary<string, int>();


        public int Dtianmingvalue = 0;

    }
    public class WarDataSave
    {
        public Dictionary<string, string> DwarIdNameDict = new Dictionary<string, string>();
        public Dictionary<string, double> DWarStartDate = new Dictionary<string, double>();
        public Dictionary<string, List<string>> DAttackers = new Dictionary<string, List<string>>();
        public Dictionary<string, List<string>> DDefenders = new Dictionary<string, List<string>>();
        public Dictionary<string, int> DWarEndDate = new Dictionary<string, int>();
        public Dictionary<string, double> DWarEndDateFloat = new Dictionary<string, double>();

        // 其他需要保存的信息
    }
}