using HarmonyLib;
namespace Figurebox.UI.Patches;

class KingdomWindowPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(KingdomWindow), "OnEnable")]
    public static bool KingdomOnEnable_Prefix(KingdomWindow __instance)
    {
        //KingdomHistoryWindow.currentKingdom = __instance.kingdom;
        if (__instance.GetComponent<KingdomWindowAdditionComponent>() == null)
        {
            __instance.gameObject.AddComponent<KingdomWindowAdditionComponent>().Initialize();
        }
        return true;
    }
}