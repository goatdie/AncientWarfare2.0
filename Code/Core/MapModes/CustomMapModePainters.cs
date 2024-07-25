using System.Collections.Generic;
using System.Linq;
using AncientWarfare.Core.Extensions;
using AncientWarfare.Core.Force;
using NeoModLoader.api.attributes;
using UnityEngine;

namespace AncientWarfare.Core.MapModes;

/**
 * 绘制自定义地图模式
 * @param pPixels 像素数组
 * @return 是否需要更新(即有无像素点更新)
 */
internal static class CustomMapModePainters
{
    private static readonly HashSet<TileZone> _drawn_zones = new();
    private static readonly HashSet<TileZone> _last_drawn_zones = new();

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

    public static bool DrawTribe(Color32[] pPixels)
    {
        foreach(var tribe in ForceManager.I.tribes.All)
        {
            var inside_color = tribe.Color.getColorBorderInsideAlpha();
            var border_color = tribe.Color.getColorMain2();
            foreach (var zone in tribe.zones)
            {
                ColorZone(pPixels, zone, inside_color, border_color, 
                    IsTribeBorder(zone.zone_up, tribe),
                    IsTribeBorder(zone.zone_down, tribe),
                    IsTribeBorder(zone.zone_left, tribe),
                    IsTribeBorder(zone.zone_right, tribe),
                    tribe.GetHashCode()
                    );
            }
        }
        ClearNotDrawnZones(pPixels);

        return _last_drawn_zones.Count > 0;
    }
    private static bool IsTribeBorder(TileZone pZone, Tribe pTribe)
    {
        return pZone?.GetTribe() != pTribe;
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
        if (pDrawBorderUp) drawn_id |= 1 << 0;
        if (pDrawBorderDown) drawn_id |= 1 << 1;
        if (pDrawBorderLeft) drawn_id |= 1 << 2;
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
}