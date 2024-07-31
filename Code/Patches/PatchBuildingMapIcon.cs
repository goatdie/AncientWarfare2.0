using AncientWarfare.Attributes;
using AncientWarfare.Core.Extensions;
using AncientWarfare.Core.Force;
using UnityEngine;

namespace AncientWarfare.Patches;

internal static class PatchBuildingMapIcon
{
    [MethodReplace(typeof(BuildingMapIcon), nameof(BuildingMapIcon.getColor))]
    private static Color32 Replace_getColor(BuildingMapIcon pThis, int pX, int pY,
                                            Building        pBuilding)
    {
        if (pX >= pThis.width || pY >= pThis.height) return Toolbox.clear;

        Color32 raw_color = pThis.tex[pY][pX].color;
        if (raw_color.a == 0) return Toolbox.clear;

        ColorAsset color_asset = null;
        if (pBuilding.kingdom != null)
        {
            color_asset = pBuilding.kingdom.kingdomColor;
        }
        else
        {
            Tribe tribe = pBuilding.GetTribe();
            if (tribe != null) color_asset = tribe.Color;
        }

        if (color_asset != null)
        {
            if (Toolbox.areColorsEqual(raw_color, Toolbox.color_magenta_0))
                return color_asset.k_color_0;
            if (Toolbox.areColorsEqual(raw_color, Toolbox.color_magenta_1))
                return color_asset.k_color_1;
            if (Toolbox.areColorsEqual(raw_color, Toolbox.color_magenta_2))
                return color_asset.k_color_2;
            if (Toolbox.areColorsEqual(raw_color, Toolbox.color_magenta_3))
                return color_asset.k_color_3;
            if (Toolbox.areColorsEqual(raw_color, Toolbox.color_magenta_4))
                return color_asset.k_color_4;
        }

        if (pBuilding.isAbandoned())
            return pThis.tex[pY][pX].color_abandoned;
        if (pBuilding.isRuin())
            return pThis.tex[pY][pX].color_ruin;
        return raw_color;
    }
}