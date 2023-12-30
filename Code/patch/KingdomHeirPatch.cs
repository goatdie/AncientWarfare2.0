using System.Collections.Generic;
using ai.behaviours;
using Figurebox.core;
using Figurebox.Utils;
using HarmonyLib;
using UnityEngine;
namespace Figurebox;



class KingdomHeirPatch
{
   
    [HarmonyPrefix]
    [HarmonyPatch(typeof(KingdomBehCheckKing), "findKing")]
    public static bool Checkheir_Pretfix(KingdomBehCheckKing __instance, Kingdom pKingdom)
    {
        AW_Kingdom awKingdom = pKingdom as AW_Kingdom;

        if (awKingdom.hasHeir())
        {
            Actor heir = awKingdom.heir;
            if (awKingdom.king == null)
            {
                __instance._units.Clear();
                awKingdom.setKing(heir);
                awKingdom.clearHeirData();
                WorldLog.logNewKing(awKingdom);
                return false;
            }

        }
        return true;

    }
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Kingdom), "setKing")]
    public static bool Clearheir_Pretfix(Kingdom __instance, Actor pActor)
    {
        AW_Kingdom awKingdom = __instance as AW_Kingdom;
        awKingdom.clearHeirData();
        return true;
    }
}
