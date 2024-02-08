using Figurebox.utils.extensions;
using UnityEngine;

namespace Figurebox.core.map_modes;

/**
 * 绘制自定义地图模式
 * @param pPixels 像素数组
 * @return 是否需要更新(即有无像素点更新)
 */
internal static class CustomMapModePainters
{
    /// <summary>
    ///     绘制附庸图
    /// </summary>
    /// <param name="pPixels"></param>
    /// <returns></returns>
    public static bool DrawVassals(Color32[] pPixels)
    {
        var need_update = false;
        foreach (Kingdom kingdom in AW_KingdomManager.Instance.list_civs)
        {
            AW_Kingdom root_suzerain = kingdom.AW().GetRootSuzerain(true);
            Color32 inside_color = root_suzerain.kingdomColor.getColorBorderInsideAlpha();
            Color32 border_color = root_suzerain.kingdomColor.getColorMain2();
            foreach (City city in kingdom.cities)
            foreach (TileZone zone in city.zones)
                need_update |= ColorZone(pPixels, zone, inside_color, border_color,
                                         IsBorderSuzerain(zone.zone_up,    city, root_suzerain),
                                         IsBorderSuzerain(zone.zone_down,  city, root_suzerain),
                                         IsBorderSuzerain(zone.zone_left,  city, root_suzerain),
                                         IsBorderSuzerain(zone.zone_right, city, root_suzerain));
        }

        return need_update;
    }

    public static bool DrawIdealogy(Color32[] pPixels)
    {
        return false;
    }

    /// <summary>
    ///     绘制一个Zone的颜色, 纯色
    /// </summary>
    /// <param name="pPixels"></param>
    /// <param name="pZone"></param>
    /// <param name="pColor">非边界颜色</param>
    /// <param name="pBorderColor">需要绘制的边界颜色</param>
    /// <param name="pDrawBorderUp"></param>
    /// <param name="pDrawBorderDown"></param>
    /// <param name="pDrawBorderLeft"></param>
    /// <param name="pDrawBorderRight"></param>
    /// <returns>是否需要更新</returns>
    private static bool ColorZone(Color32[] pPixels, TileZone pZone, Color32 pColor, Color32 pBorderColor,
                                  bool      pDrawBorderUp   = false, bool pDrawBorderDown = false,
                                  bool      pDrawBorderLeft = false, bool pDrawBorderRight = false)
    {
        uint drawn_id = 0;
        if (pDrawBorderUp) drawn_id |= 1    << 0;
        if (pDrawBorderDown) drawn_id |= 1  << 1;
        if (pDrawBorderLeft) drawn_id |= 1  << 2;
        if (pDrawBorderRight) drawn_id |= 1 << 3;

        if (pZone.last_drawn_id == drawn_id) return false;

        var drawn_hashcode = (pColor.a << 24) | (pColor.r << 16) | (pColor.g << 8) | pColor.b;
        drawn_hashcode ^= (pBorderColor.a << 24) | (pBorderColor.r << 16) | (pBorderColor.g << 8) | pBorderColor.b;
        if (pZone.last_drawn_hashcode == drawn_hashcode) return false;

        pZone.last_drawn_id = (int)drawn_id;
        pZone.last_drawn_hashcode = drawn_hashcode;
        foreach (WorldTile tile in pZone.tiles)
            if (tile.worldTileZoneBorder.borderUp && pDrawBorderUp)
                pPixels[tile.data.tile_id] = pBorderColor;
            else if (tile.worldTileZoneBorder.borderDown && pDrawBorderDown)
                pPixels[tile.data.tile_id] = pBorderColor;
            else if (tile.worldTileZoneBorder.borderLeft && pDrawBorderLeft)
                pPixels[tile.data.tile_id] = pBorderColor;
            else if (tile.worldTileZoneBorder.borderRight && pDrawBorderRight)
                pPixels[tile.data.tile_id] = pBorderColor;
            else
                pPixels[tile.data.tile_id] = pColor;

        return true;
    }

    /// <summary>
    ///     清空一个Zone的颜色
    /// </summary>
    /// <param name="pPixels"></param>
    /// <param name="pZone"></param>
    /// <returns></returns>
    private static bool ClearZone(Color32[] pPixels, TileZone pZone)
    {
        if (pZone.last_drawn_id == 0) return false;
        pZone.last_drawn_id = 0;
        foreach (WorldTile tile in pZone.tiles) pPixels[tile.data.tile_id] = Color.clear;

        return true;
    }

    private static bool IsBorderSuzerain(TileZone pZone, City pCity, AW_Kingdom pSuzerain)
    {
        if (pZone?.city                                      == null) return true;
        if (pZone.city                                       == pCity) return false;
        return pZone.city.kingdom.AW().GetRootSuzerain(true) != pSuzerain;
    }
}