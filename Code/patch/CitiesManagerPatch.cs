using Figurebox.core;
using Figurebox.core.events;
using HarmonyLib;

namespace Figurebox.patch;

internal class CitiesManagerPatch : AutoPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(CitiesManager), nameof(CitiesManager.buildNewCity))]
    private static void buildNewCity_Postfix(City __result)
    {
        EventsManager.Instance.NewCity(__result);
    }
}