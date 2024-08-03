using System.Collections.Generic;
using AncientWarfare.Attributes;
using AncientWarfare.Core;
using AncientWarfare.Core.Extensions;
using AncientWarfare.Core.Force;
using HarmonyLib;
using UnityEngine;

namespace AncientWarfare.Patches;

internal static class PatchBuilding
{
    [MethodReplace(typeof(Building), nameof(Building.canBeUpgraded))]
    private static bool Replace_canBeUpgraded(Building building)
    {
        if (building.isUnderConstruction()) return false;
        if (!building.asset.canBeUpgraded) return false;
        // TODO: 检查科技要求
        return true;
    }

    [MethodReplace(typeof(Building), nameof(Building.checkSpriteToRender))]
    private static void Replace_checkSpriteToRender(Building pThis)
    {
        var flag = true;
        Sprite[] array;
        var flag2 = pThis.isRuin();
        if (flag2) flag = false;

        if (pThis.isUnderConstruction())
        {
            pThis.last_main_sprite = pThis.asset.sprites.construction;
            pThis.spriteRenderer.sprite = pThis.last_main_sprite;
            return;
        }

        if (pThis.asset.has_special_animation_state)
        {
            array = !pThis.hasResources ? pThis.animData.special : pThis.animData.main;
        }
        else if (flag2 && pThis.asset.has_ruins_graphics)
        {
            array = pThis.animData.ruins;
        }
        else if (pThis.asset.spawn_drops && pThis.data.hasFlag(S.stop_spawn_drops))
        {
            array = pThis.animData.main_disabled;
        }
        else if (pThis.data.state == BuildingState.CivAbandoned)
        {
            array = pThis.animData.main_disabled.Length == 0 ? pThis.animData.main : pThis.animData.main_disabled;
            flag = false;
        }
        else
        {
            array = pThis.animData.main;
            var array2 = pThis.asset.get_override_sprite_main?.Invoke(pThis);
            if (array2 != null) array = array2;
        }

        Sprite sprite = pThis.check_spawn_animation
            ? pThis.getSpawnFrameSprite()
            : flag && array.Length != 1
                ? AnimationHelper.getSpriteFromList(pThis.GetHashCode(), array, pThis.asset.animation_speed)
                : array[0];

        ColorAsset current_color_asset = pThis.kingdom?.kingdomColor;
        if (current_color_asset == null)
        {
            Tribe tribe = pThis.GetTribe();
            if (tribe != null) current_color_asset = tribe.Color;
        }

        if (pThis.last_main_sprite != sprite || pThis._last_color_asset != current_color_asset)
        {
            pThis._last_colored_sprite = UnitSpriteConstructor.getRecoloredSpriteBuilding(sprite, current_color_asset);
            pThis.spriteRenderer.sprite = pThis._last_colored_sprite;
            pThis.last_main_sprite = sprite;
            pThis._last_color_asset = current_color_asset;
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Building), nameof(Building.removeBuildingFinal))]
    private static void Postfix_removeBuildingFinal(Building __instance)
    {
        BuildingAdditionData data = __instance.GetAdditionData();
        if (data == null) return;
        var forces = new HashSet<string>(data.Forces);
        foreach (var force_id in forces) ForceManager.MakeLeaveForce(__instance, force_id);
    }
}