using ai.behaviours;
using Figurebox.core;
using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using Figurebox.Utils;


namespace Figurebox;

class KingdomHeirPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(KingdomBehCheckKing), "findKing")]
    public static bool Checkheir_Pretfix(KingdomBehCheckKing __instance, Kingdom pKingdom)
    {
        AW_Kingdom awKingdom = pKingdom as AW_Kingdom;
        // 当能够进到这里时，国家已经没有国王了，可以不用检查
        if (awKingdom.hasHeir())
        {
            __instance._units.Clear();
            awKingdom.setKing(awKingdom.heir);
            WorldLog.logNewKing(awKingdom);
            return false;
        }

        return true;
    }
    [HarmonyPostfix]
    [HarmonyPatch(typeof(MapIconLibrary), "drawKings")]
    public static void DrawHeir_Postfix(MapIconLibrary __instance, MapIconAsset pAsset)
    {
        Sprite newIcon = SpriteTextureLoader.getSprite("civ/icons/minimap_heir");
        List<Kingdom> list_civs = World.world.kingdoms.list_civs;
        foreach (Kingdom k in list_civs)
        {
            List<Actor> actorList = k.units.getSimpleList();
            foreach (Actor actor in actorList)
            {
                if (!(actor == null) && actor.isAlive() && !actor.isInMagnet() && (!actor.isKing() && !actor.isCityLeader()) && actor.IsHeir())
                {
                    Vector3 pPos = actor.currentPosition;
                    pPos.y -= 3f;

                    GroupSpriteObject groupSpriteObject = MapIconLibrary.drawMark(pAsset, pPos, null, k, actor.city, null, 0.8f, false, -1f);
                    Sprite spriteIcon = UnitSpriteConstructor.getSpriteIcon(newIcon, k.getColor());
                    groupSpriteObject.setSprite(spriteIcon);
                }
            }
        }
    }
}