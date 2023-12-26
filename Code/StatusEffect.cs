using System;
using System.Threading;
using System.Reflection;
using NCMS;
using Unity;
using ReflectionUtility;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using Figurebox.Utils;
using NCMS.Utils;


namespace Figurebox
{
    class StatusEffectLib
    {
        public static void init()
        {

            StatusEffect tianming0 = new StatusEffect();
            tianming0.id = "tianming0";
            tianming0.texture = "tianming0";
            tianming0.name = "status_title_tianming0";
            tianming0.animated = true;
            tianming0.animation_speed = 0.1f;
            tianming0.base_stats[S.armor] = 10;
            tianming0.base_stats[S.damage] = 60;
            tianming0.base_stats[S.knockback_reduction] = 1f;
            tianming0.duration = 5f;
            //tianming0.action = new WorldAction(StatusLibrary.ashFeverEffect),
            tianming0.description = "status_description_tianming0";
            tianming0.path_icon = "ui/effects/tianming0/moh";
            AssetManager.status.add(tianming0);
            addTraitToLocalizedLibrary("cz", tianming0.id, "天命稳定", "天命帝国正处于稳定发展的时期");
            addTraitToLocalizedLibrary("ch", tianming0.id, "天命稳定", "天命帝国正处于稳定发展的时期");
            addTraitToLocalizedLibrary("en", tianming0.id, "moh is steady", "The empire now is steadily developing");
            StatusEffect tianmingm1 = new StatusEffect();
            tianmingm1.id = "tianmingm1";
            tianmingm1.texture = "tianmingm1";
            tianmingm1.name = "status_title_tianmingm1";
            tianmingm1.animated = true;
            tianmingm1.animation_speed = 0.1f;
            //tianmingm1.base_stats[S.armor] = 10;
            //tianmingm1.base_stats[S.damage] = 60;
            tianmingm1.base_stats[S.knockback_reduction] = 1f;
            tianmingm1.duration = 5f;
            //tianmingm1.action = new WorldAction(StatusEffectLib.decline),
            tianmingm1.description = "status_description_tianmingm1";
            tianmingm1.path_icon = "ui/effects/tianming0/moh-1";
            AssetManager.status.add(tianmingm1);
            addTraitToLocalizedLibrary("cz", tianmingm1.id, "天命摇摇欲坠", "天命不稳定，已经有崩溃之势");
            addTraitToLocalizedLibrary("ch", tianmingm1.id, "天命摇摇欲坠", "天命不稳定，已经有崩溃之势");
            addTraitToLocalizedLibrary("en", tianmingm1.id, "moh is declining", "The empire is now declining");

        }
        private static void addTraitToLocalizedLibrary(string pLanguage, string id, string name, string description)
        {
            string language = Reflection.GetField(LocalizedTextManager.instance.GetType(), LocalizedTextManager.instance, "language") as string;
            if (language != "en" && language != "ch" && language != "cz")
            {
                pLanguage = "en";
            }
            if (pLanguage == language)
            {
                Dictionary<string, string> localizedText = Reflection.GetField(LocalizedTextManager.instance.GetType(), LocalizedTextManager.instance, "localizedText") as Dictionary<string, string>;
                localizedText.Add("status_title_" + id, name);
                localizedText.Add("status_description_" + id, description);
            }
        }
        public static bool decline(BaseSimObject pTarget, WorldTile pTile = null)
        {
            Actor a = (Actor)pTarget;


            return true;
        }

    }

}