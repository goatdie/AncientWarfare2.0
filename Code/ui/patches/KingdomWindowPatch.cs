using HarmonyLib;
namespace Figurebox.UI.Patches;

class KingdomWindowPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(KingdomWindow), "showInfo")]
    public static void Vassals_Post(KingdomWindow __instance)
    {
        var suzerain = KingdomVassals.GetKingdom(__instance.kingdom);
        // 显示宗主国信息
        __instance.showStat("Suzerain", suzerain == __instance.kingdom ? "-" : suzerain.name);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(KingdomWindow), "OnEnable")]
    public static bool KingdomOnEnable_Prefix(KingdomWindow __instance)
    {
        KingdomHistoryWindow.currentKingdom = __instance.kingdom;
        return true;
    }
}