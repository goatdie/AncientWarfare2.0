using HarmonyLib;
namespace Figurebox.UI.Patches;

public class CityWindowPatch
{

    [HarmonyPrefix]
    [HarmonyPatch(typeof(CityWindow), "OnEnable")]
    public static bool cityOnEnable_Prefix(CityWindow __instance)
    {
        CityHistoryWindow.currentCity = __instance.city;
        return true;
    }
}