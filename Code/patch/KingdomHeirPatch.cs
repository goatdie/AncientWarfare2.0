using ai.behaviours;
using Figurebox.core;
using HarmonyLib;

namespace Figurebox;

class KingdomHeirPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(KingdomBehCheckKing), "findKing")]
    public static bool Checkheir_Pretfix(KingdomBehCheckKing __instance, Kingdom pKingdom)
    {
        AW_Kingdom awKingdom = pKingdom as AW_Kingdom;
        // 当能够进到这里时，国家已经没有国王了，可以不用检查
        if (awKingdom.hasHeir())
        {
            __instance._units.Clear();
            awKingdom.setKing(awKingdom.heir);
            WorldLog.logNewKing(awKingdom);
            return false;
        }

        return true;
    }
}