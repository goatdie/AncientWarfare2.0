using Figurebox.core;
using Figurebox.utils.extensions;
using HarmonyLib;

namespace Figurebox.patch;

internal class PlotPatch : AutoPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Plot), nameof(Plot.isSameType))]
    private static void IsSameType(Plot __instance, ref bool __result, PlotAsset pAsset)
    {
        if (__result) return;
        AW_PlotAsset instance_asset = __instance.getAsset().AW();
        if (instance_asset == null) return;
        AW_PlotAsset plot_asset = pAsset.AW();
        if (plot_asset == null) return;
        __result = instance_asset.plot_type == plot_asset.plot_type;
    }
}