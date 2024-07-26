using AncientWarfare.Const;
using AncientWarfare.Core;
using AncientWarfare.Core.Extensions;
using AncientWarfare.Core.Force;
using AncientWarfare.UI.Windows;
using AncientWarfare.Utils;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using CustomMapMode = AncientWarfare.Core.MapModes.CustomMapMode;

namespace AncientWarfare.Patches
{
    internal static class PatchMapBox
    {
        [HarmonyPostfix, HarmonyPatch(typeof(MapBox), nameof(MapBox.setMapSize))]
        private static void Postfix_setMapSize()
        {
            TileZoneAdditionDataManager.Reset();
        }
        [HarmonyPostfix, HarmonyPatch(typeof(MapBox), nameof(MapBox.updateCities))]
        private static void Postfix_updateCities()
        {
            Toolbox.bench("forces", "game_total");
            ForceManager.I.Update();
            Toolbox.benchEnd("forces", "game_total", false, 0);
        }
        [HarmonyTranspiler, HarmonyPatch(typeof(MapBox), nameof(MapBox.checkClickTouchInspect))]
        private static IEnumerable<CodeInstruction> Transpiler_checkClickTouchInspect(IEnumerable<CodeInstruction> insts)
        {
            var codes = new List<CodeInstruction>(insts);

            var insert_idx = HarmonyTools.FindInstructionIdx<MethodInfo>(codes, OpCodes.Call, x => x.Name == nameof(MapBox.isRenderMiniMap));
            var continue_judge = new Label();
            var break_judge = (Label)codes[insert_idx - 1].operand;

            insert_idx = insert_idx + 2;
            codes[insert_idx].WithLabels(continue_judge);
            codes.InsertRange(insert_idx, new List<CodeInstruction>
            {
                new CodeInstruction(OpCodes.Ldloc_0),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PatchMapBox), nameof(PatchMapBox.CheckClickTouchInspectBreak))),
                new CodeInstruction(OpCodes.Brfalse_S, continue_judge),
                new CodeInstruction(OpCodes.Br_S, break_judge)
            });

            return codes;
        }
        private static bool CheckClickTouchInspectBreak(WorldTile tile)
        {
            var map_mode = Core.MapModes.Manager.map_mode;
            switch (map_mode)
            {
                case CustomMapMode.Hidden:
                    return false;
                case CustomMapMode.Tribe:
                    var tribe = tile.zone.GetTribe();
                    if (tribe != null)
                    {
                        AdditionConfig.selectedTribe = tribe;
                        ScrollWindow.showWindow(nameof(TribeInfoWindow));
                    }
                    break;
            }
            return true;
        }
    }
}
