using AncientWarfare.Core;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AncientWarfare.Patches
{
    internal static class PatchMapBox
    {
        [HarmonyPostfix, HarmonyPatch(typeof(MapBox), nameof(MapBox.setMapSize))]
        private static void Postfix_setMapSize()
        {
            TileZoneAdditionDataManager.Reset();
        }
    }
}
