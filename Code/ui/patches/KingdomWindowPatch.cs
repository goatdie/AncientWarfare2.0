namespace Figurebox.UI.Patches;
class KingdomWindowPatch{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(KingdomWindow), "showInfo")]
    public static void Vassals_Post(KingdomWindow __instance)
    {
        // 显示宗主国信息
        __instance.showStat("Suzerain", GetKingdom(__instance.kingdom) == __instance.kingdom ? "-" : suzerain.name);
    }
}