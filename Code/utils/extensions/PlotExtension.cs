using Figurebox.core;

namespace Figurebox.utils.extensions;

public static class PlotExtension
{
    public static AW_PlotAsset AW(this PlotAsset pPlot)
    {
        return pPlot as AW_PlotAsset;
    }
}