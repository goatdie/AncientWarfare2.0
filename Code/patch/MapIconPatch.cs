using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Figurebox.attributes;
using Figurebox.core;
using Figurebox.core.map_modes;
using Figurebox.utils;
using Figurebox.utils.extensions;
using Figurebox.utils.instpredictors;
using HarmonyLib;
using NeoModLoader.api.attributes;
using UnityEngine;

namespace Figurebox.patch;

public class MapIconPatch : AutoPatch
{
    [Hotfixable]
    [MethodReplace(typeof(MapIconLibrary), nameof(MapIconLibrary.drawArmies))]
    private static void drawGroups(MapIconAsset pAsset)
    {
        if (!PlayerConfig.optionBoolEnabled("marks_armies")) return;

        foreach (UnitGroup group in AW_UnitGroupManager.instance.groups)
        {
            var aw_group = group as AW_UnitGroup;
            if (aw_group == null) continue;

            if (aw_group.groupLeader == null || !aw_group.groupLeader.isAlive() ||
                aw_group.groupLeader.isInMagnet()) continue;

            Kingdom kingdom = aw_group.groupLeader.kingdom;
            if (!kingdom.isCiv()) continue;

            MapMark map_mark = MapIconLibrary.drawMark(pAsset, aw_group.groupLeader.currentPosition, null, kingdom,
                                                       aw_group.groupLeader.city);
            if (DebugConfig.isOn(DebugOption.ShowAmountNearArmy))
            {
                map_mark.text.gameObject.SetActive(true);
                map_mark.text.text = aw_group.countUnits().ToString() ?? "";
                map_mark.text.GetComponent<Renderer>().sortingLayerID = map_mark.spriteRenderer.sortingLayerID;
                map_mark.text.GetComponent<Renderer>().sortingOrder = map_mark.spriteRenderer.sortingOrder;
            }
            else
            {
                map_mark.text.gameObject.SetActive(false);
            }

            Sprite icon = SpriteTextureLoader.getSprite(aw_group.asset.path_icon);
            if (icon == null)
            {
                icon = SpriteTextureLoader.getSprite("civ/icons/minimap_flag");
                Main.LogWarning($"No found sprite for group '{aw_group.asset.id}'s icon '{aw_group.asset.path_icon}'",
                                pLogOnlyOnce: true);
            }

            map_mark.setSprite(UnitSpriteConstructor.getSpriteIcon(icon, kingdom.getColor()));
        }
    }

    [HarmonyTranspiler]
    [HarmonyPatch(typeof(MapIconManager), nameof(MapIconManager.updateScaleEffect))]
    private static IEnumerable<CodeInstruction> UpdateMapIconScaleEffect(IEnumerable<CodeInstruction> codes)
    {
        var list = new List<CodeInstruction>(codes);

        var index = HarmonyTools.FindCodeSnippet(list, out var snippet,
                                                 new MethodInstPredictor(OpCodes.Call, "get_world"),
                                                 new MethodInstPredictor(
                                                     OpCodes.Callvirt, nameof(MapBox.showCityZones)),
                                                 new BaseInstPredictor(OpCodes.Brfalse));

        var show_city_zones_label = new Label();
        snippet[0].labels.Add(show_city_zones_label);

        var flag_true_label = (Label)list[2 + HarmonyTools.FindCodeSnippetIdx(
                                              list, new BaseInstPredictor(OpCodes.Ldc_I4_1),
                                              new LocalVarInstPredictor(OpCodes.Stloc_S, 11),
                                              new BaseInstPredictor(OpCodes.Br))].operand;

        var city2 =
            HarmonyTools.FindInstruction<LocalBuilder>(list, OpCodes.Stloc_S, x => x.LocalIndex == 10)
                        .operand as LocalBuilder;
        var flag =
            HarmonyTools.FindInstruction<LocalBuilder>(list, OpCodes.Stloc_S, x => x.LocalIndex == 11)
                        .operand as LocalBuilder;
        // push city
        list.Insert(index++, new CodeInstruction(OpCodes.Ldloc_1));
        // push city2
        list.Insert(index++, new CodeInstruction(OpCodes.Ldloc_S, city2));
        // call JudgeScaleEffect
        list.Insert(
            index++,
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(MapIconPatch), nameof(JudgeScaleEffect))));
        // if false continue check
        list.Insert(index++, new CodeInstruction(OpCodes.Brfalse, show_city_zones_label));
        // push 1
        list.Insert(index++, new CodeInstruction(OpCodes.Ldc_I4_1));
        // store to flag
        list.Insert(index++, new CodeInstruction(OpCodes.Stloc_S, flag));
        // if flag, goto IL_0147
        list.Insert(index++, new CodeInstruction(OpCodes.Br, flag_true_label));

        return list;
    }

    private static bool JudgeScaleEffect(City pCursorCity, City pCheckCity)
    {
        switch (MapModeManager.map_mode)
        {
            case CustomMapMode.Vassals:
                return pCursorCity?.kingdom?.AW()?.GetRootSuzerain(true) ==
                       pCheckCity.kingdom.AW().GetRootSuzerain(true);
        }

        return false;
    }
}