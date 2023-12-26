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

        }
        public static bool decline(BaseSimObject pTarget, WorldTile pTile = null)
        {
            Actor a = (Actor)pTarget;


            return true;
        }

    }

}