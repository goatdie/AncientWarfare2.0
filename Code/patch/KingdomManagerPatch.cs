using Figurebox.core;
using Figurebox.core.events;
using HarmonyLib;

namespace Figurebox.patch;

internal class KingdomManagerPatch : AutoPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(KingdomManager), nameof(KingdomManager.makeNewCivKingdom))]
    public static void makeNewCivKingdom_Postfix(Kingdom __result)
    {
        EventsManager.Instance.NewKingdom(__result);
    }
}