using HarmonyLib;
using UnityEngine;
using Figurebox.Utils;
using ai;
using System.Collections.Generic;
using UnityEngine.UI;
using ai.behaviours.conditions;
using ai.behaviours;
using System;
using System.Reflection;
using Figurebox;
using System.Linq;
using System.IO;
using Figurebox.core;

namespace Figurebox;
class KingdomPatch
{
  [HarmonyPostfix]
  [HarmonyPatch(typeof(KingdomBehCheckKing), "execute")]
  public static void Checkheir_Postfix(KingdomBehCheckKing __instance, Kingdom pKingdom)
  {
    AW_Kingdom awKingdom = pKingdom as AW_Kingdom;
    if (!awKingdom.hasHeir())
    {
      awKingdom.SetHeir(awKingdom.FindHeir());
    }


  }
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
