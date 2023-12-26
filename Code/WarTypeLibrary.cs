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

namespace Figurebox
{
    class warTypeLibrary
    {


        public static void init()
        {
            WarTypeAsset tianming = new WarTypeAsset();
            tianming.id = "tianming";
            addWarTypeToLocalizedLibrary("en", tianming.id, "Ancient Warfare");
            addWarTypeToLocalizedLibrary("cz", tianming.id, "天命战争");
            tianming.name_template = "war_conquest";
            tianming.localized_type = "war_type_tianming";
            tianming.path_icon = "ui/wars/war_tianming";
            tianming.kingdom_for_name_attacker = true;
            tianming.forced_war = false;
            tianming.total_war = false;
            tianming.alliance_join = false;
            AssetManager.war_types_library.add(tianming);
            WarTypeAsset tianmingrebel = new WarTypeAsset();
            tianmingrebel.id = "tianmingrebel";
            addWarTypeToLocalizedLibrary("en", tianmingrebel.id, "Ancient Warfare-Rebel");
            addWarTypeToLocalizedLibrary("cz", tianmingrebel.id, "天命战争-起义");
            tianmingrebel.name_template = "war_rebellion";
            tianmingrebel.localized_type = "war_type_tianmingrebel";
            tianmingrebel.path_icon = "ui/wars/war_tianmingrebel";
            tianmingrebel.kingdom_for_name_attacker = true;
            tianmingrebel.forced_war = false;
            tianmingrebel.total_war = false;
            tianmingrebel.alliance_join = false;
            AssetManager.war_types_library.add(tianmingrebel);

            WarTypeAsset reclaim = new WarTypeAsset();
            reclaim.id = "reclaim";
            addWarTypeToLocalizedLibrary("en", reclaim.id, "Reclaim");
            addWarTypeToLocalizedLibrary("cz", reclaim.id, "收复战争");
            reclaim.name_template = "war_reclaim";
            reclaim.localized_type = "war_type_reclaim";
            reclaim.path_icon = "ui/wars/war_reclaim";
            reclaim.kingdom_for_name_attacker = true;
            reclaim.forced_war = false;
            reclaim.total_war = false;
            reclaim.alliance_join = true;
            AssetManager.war_types_library.add(reclaim);
            WarTypeAsset vassalWar = new WarTypeAsset();
            vassalWar.id = "vassal_war";
            addWarTypeToLocalizedLibrary("en", vassalWar.id, "Vassal War");
            addWarTypeToLocalizedLibrary("cz", vassalWar.id, "附庸战争");
            vassalWar.name_template = "war_conquest";
            vassalWar.localized_type = "war_type_vassal_war";
            vassalWar.path_icon = "ui/wars/war_vassal";
            vassalWar.kingdom_for_name_attacker = true;
            vassalWar.forced_war = false;
            vassalWar.total_war = false;
            vassalWar.alliance_join = false;
            AssetManager.war_types_library.add(vassalWar);
            WarTypeAsset IndependenceWar = new WarTypeAsset();
            IndependenceWar.id = "independence_war";
            addWarTypeToLocalizedLibrary("en", IndependenceWar.id, "Independence War");
            addWarTypeToLocalizedLibrary("cz", IndependenceWar.id, "独立战争");
            IndependenceWar.name_template = "war_conquest";
            IndependenceWar.localized_type = "war_type_independence_war";
            IndependenceWar.path_icon = "ui/wars/war_independent";
            IndependenceWar.kingdom_for_name_attacker = true;
            IndependenceWar.forced_war = false;
            IndependenceWar.total_war = false;
            IndependenceWar.alliance_join = false;
            AssetManager.war_types_library.add(IndependenceWar);



        }



        private static void addWarTypeToLocalizedLibrary(string pLanguage, string id, string name)
        {
            string language = Reflection.GetField(LocalizedTextManager.instance.GetType(), LocalizedTextManager.instance, "language") as string;
            if (language != "en" && language != "ch" && language != "cz")
            {
                pLanguage = "en";
            }
            if (language == "ch")
            {
                pLanguage = "cz";
            }
            if (pLanguage == language)
            {
                Dictionary<string, string> localizedText = Reflection.GetField(LocalizedTextManager.instance.GetType(), LocalizedTextManager.instance, "localizedText") as Dictionary<string, string>;
                localizedText.Add("war_type_" + id, name);
            }
        }














    }

}