using System;
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

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ClanManager), nameof(ClanManager.checkActionKing))]
    private static void AdditionPlotsCheck(ClanManager __instance, Actor pActor)
    {
        if (Math.Abs(__instance._timestamp_last_plot - World.world.getCurWorldTime()) < 0.001) return;

        var flag = content.PlotsLibrary.tryPlotActiveVassal(__instance, pActor);

        if (!flag)
        {
            // 其他plot
        }

        if (flag) __instance._timestamp_last_plot = World.world.getCurWorldTime();
    }
}