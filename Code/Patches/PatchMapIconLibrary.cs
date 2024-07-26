using AncientWarfare.Core;
using AncientWarfare.Core.Extensions;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

using CustomMapMode = AncientWarfare.Core.MapModes.CustomMapMode;
namespace AncientWarfare.Patches
{
    internal static class PatchMapIconLibrary
    {
        [HarmonyPostfix, HarmonyPatch(typeof(MapIconLibrary), nameof(MapIconLibrary.drawCursorZones))]
        private static void Postfix_drawCursorZones(MapIconAsset pAsset)
        {
            if (World.world.isBusyWithUI() || !Input.mousePresent) return;

            var tile = World.world.getMouseTilePos();
            if (tile == null) return;

            var map_mode = Core.MapModes.Manager.map_mode;
            if (map_mode == CustomMapMode.Tribe)
            {
                var tribe = tile.zone.GetTribe();
                if (tribe == null) return;
                MapIconLibrary.colorZones(pAsset, tribe.zones, pAsset.color);
            }
        }
    }
}
