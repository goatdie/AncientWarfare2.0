using HarmonyLib;
namespace Figurebox.UI.Patches;

class KingdomWindowPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(KingdomWindow), "OnEnable")]
    public static void KingdomOnEnable_Prefix(KingdomWindow __instance)
    {
        //KingdomHistoryWindow.currentKingdom = __instance.kingdom;
        if (__instance.GetComponent<KingdomWindowAdditionComponent>() == null)
        {
            __instance.gameObject.AddComponent<KingdomWindowAdditionComponent>().Initialize();
        }
    }
}