using Figurebox.attributes;
using Figurebox.core;
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
}