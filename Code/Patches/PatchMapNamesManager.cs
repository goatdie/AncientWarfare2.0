using AncientWarfare.Core;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AncientWarfare.Patches
{
    internal static class PatchMapNamesManager
    {
        [HarmonyPostfix, HarmonyPatch(typeof(MapNamesManager), nameof(MapNamesManager.getCurrentMode))]
        private static void Postfix_getCurrentMode(ref MapMode __result)
        {
            if (__result == MapMode.None) return;
            var force_mode = World.world.getForcedMapMode(MapMode.None);
            if (force_mode != MapMode.None) return;

            var mode = Core.MapModes.Manager.map_mode;
            if (mode == Core.MapModes.CustomMapMode.Hidden) return;
            __result = (MapMode)mode;
        }
    }
}
