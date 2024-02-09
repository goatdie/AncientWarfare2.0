using System.Collections.Generic;
using System.Linq;
using Figurebox.utils.extensions;
using NeoModLoader.api.attributes;
using UnityEngine;

namespace Figurebox.core.map_modes;

/**
 * 绘制自定义地图模式
 * @param pPixels 像素数组
 * @return 是否需要更新(即有无像素点更新)
 */
internal static class CustomMapModePainters
{
    private static readonly List<Kingdom>  _tmp_kingdoms = new();
    private static readonly List<City>     _tmp_cities   = new();
    private static readonly List<TileZone> _tmp_zones    = new();

    private static readonly HashSet<TileZone> _drawn_zones      = new();
    private static readonly HashSet<TileZone> _last_drawn_zones = new();

    /// <summary>
    ///     绘制附庸图
    /// </summary>
    /// <param name="pPixels"></param>
    /// <returns></returns>
    [Hotfixable]
    public static bool DrawVassals(Color32[] pPixels)
    {
        _tmp_kingdoms.AddRange(AW_KingdomManager.Instance.list_civs);
        foreach (Kingdom kingdom in _tmp_kingdoms)
        {
            AW_Kingdom root_suzerain = kingdom.AW().GetRootSuzerain(true);
            Color32 inside_color = root_suzerain.kingdomColor.getColorBorderInsideAlpha();
            Color32 border_color = root_suzerain.kingdomColor.getColorMain2();

            _tmp_cities.AddRange(kingdom.cities);
            foreach (City city in _tmp_cities)
            {
                _tmp_zones.AddRange(city.zones);
                foreach (TileZone zone in _tmp_zones)
                    ColorZone(pPixels, zone, inside_color, border_color,
                              IsBorderSuzerain(zone.zone_up,    city, root_suzerain),
                              IsBorderSuzerain(zone.zone_down,  city, root_suzerain),
                              IsBorderSuzerain(zone.zone_left,  city, root_suzerain),
                              IsBorderSuzerain(zone.zone_right, city, root_suzerain),
                              root_suzerain.GetHashCode());
                _tmp_zones.Clear();
            }

            _tmp_cities.Clear();
        }

        _tmp_kingdoms.Clear();
        ClearNotDrawnZones(pPixels);

        return _last_drawn_zones.Count > 0;
    }

    /// <summary>
    ///     清除本次未被绘制的所有Zone
    /// </summary>
    /// <param name="pPixels"></param>
    private static void ClearNotDrawnZones(Color32[] pPixels)
    {
        foreach (TileZone zone in _last_drawn_zones.Where(zone => !_drawn_zones.Contains(zone)))
            ClearZone(pPixels, zone);
        _last_drawn_zones.Clear();
        _last_drawn_zones.UnionWith(_drawn_zones);
        _drawn_zones.Clear();
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
    /// <param name="pDrawnHashCode">绘制hash, 用于检测是否需要重复绘制</param>
    /// <returns>是否需要更新</returns>
    [Hotfixable]
    private static bool ColorZone(Color32[] pPixels, TileZone pZone, Color32 pColor, Color32 pBorderColor,
                                  bool pDrawBorderUp = false, bool pDrawBorderDown = false,
                                  bool pDrawBorderLeft = false, bool pDrawBorderRight = false, int pDrawnHashCode = 0)
    {
        _drawn_zones.Add(pZone);

        uint drawn_id = 0;
        if (pDrawBorderUp) drawn_id |= 1    << 0;
        if (pDrawBorderDown) drawn_id |= 1  << 1;
        if (pDrawBorderLeft) drawn_id |= 1  << 2;
        if (pDrawBorderRight) drawn_id |= 1 << 3;

        if (pDrawnHashCode != 0)
        {
            if (pZone.last_drawn_id == drawn_id && pZone.last_drawn_hashcode == pDrawnHashCode) return false;
            pZone.last_drawn_hashcode = pDrawnHashCode;
        }

        pZone.last_drawn_id = (int)drawn_id;
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
    ///     清空所有颜色
    /// </summary>
    /// <param name="pPixels"></param>
    public static void ClearAll(Color32[] pPixels)
    {
        foreach (TileZone zone in World.world.zoneCalculator.zones) ClearZone(pPixels, zone);
        _last_drawn_zones.Clear();
    }

    /// <summary>
    ///     清空一个Zone的颜色
    /// </summary>
    /// <param name="pPixels"></param>
    /// <param name="pZone"></param>
    /// <returns></returns>
    private static bool ClearZone(Color32[] pPixels, TileZone pZone)
    {
        if (pZone.last_drawn_id == -1) return false;
        pZone.last_drawn_id = -1;
        pZone.last_drawn_hashcode = -1;
        foreach (WorldTile tile in pZone.tiles) pPixels[tile.data.tile_id] = Color.clear;

        return true;
    }

    private static bool IsBorderSuzerain(TileZone pZone, City pCity, AW_Kingdom pSuzerain)
    {
        if (pZone?.city == null) return true;

        if (pZone.city == pCity) return false;

        return pZone.city.kingdom.AW().GetRootSuzerain(true) != pSuzerain;
    }
}