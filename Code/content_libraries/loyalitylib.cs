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
    class LoyaltyLibrary
    {
        public static void init()
        {
            LoyaltyAsset tianming = new LoyaltyAsset();
            tianming.id = "tianming";
            tianming.translation_key = "tianming";
            tianming.calc = delegate (City pCity)
            {
                int result = 0;
                Actor king = pCity.kingdom.king;
                if (king != null && king.hasStatus("tianming0"))//if has tianming1 tianming2.....
                {
                    result = (int)(king.stats[S.diplomacy] + king.stats[S.stewardship] * 1f);
                }
                if (king != null && king.hasStatus("tianmingm1"))//if has tianming1 tianming2.....
                {
                    result = -(int)(king.stats[S.diplomacy] + king.stats[S.stewardship] * 2f);
                }
                return result;
            };
            AssetManager.loyalty_library.add(tianming);
            OpinionAsset tianminggo = new OpinionAsset();
            tianminggo.id = "tianminggo";
            tianminggo.translation_key = "tianminggo";
            AssetManager.opinion_library.add(tianminggo);
            tianminggo.calc = delegate (Kingdom pMain, Kingdom pTarget)
            {
                int result = 0;
                bool tianmingBoolValue;
                pMain.data.get("tianmingbool", out tianmingBoolValue);
                if (tianmingBoolValue)
                {
                    result = -200;
                }
                return result;
            };

            OpinionAsset vassels = new OpinionAsset();
            vassels.id = "vassels";
            vassels.translation_key = "vassels";
            AssetManager.opinion_library.add(vassels);

            vassels.calc = delegate (Kingdom pMain, Kingdom pTarget)
            {
                int result = 0;
                string vassalId;
                string vassaltarget;
                pMain.data.get("vassels", out vassalId);
                pTarget.data.get("vassels", out vassaltarget);
                if (vassalId != pMain.data.id && vassalId == pTarget.data.id)
                {
                    result = 1000;
                }
                if (vassaltarget == pMain.data.id && String.IsNullOrEmpty(vassalId))
                {
                    result = 500;
                }
                if (vassalId == vassaltarget && !string.IsNullOrEmpty(vassalId))
                {
                    result = 500;
                }
                return result;
            };
        }
    }
}