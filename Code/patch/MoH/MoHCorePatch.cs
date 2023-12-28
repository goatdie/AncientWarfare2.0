namespace Figurebox.patch.MoH;
using HarmonyLib;
using UnityEngine;
using Figurebox.Utils.MoH;
using Figurebox.core;

internal static class MoHCorePatch 
{
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MapText), "showTextKingdom")]
        public static void SetTianmingicon_Postfix(MapText __instance, Kingdom pKingdom)
        { AW_Kingdom awKingdom = pKingdom as AW_Kingdom;

            if (MoHTools.IsMoHKingdom(awKingdom))
            {

                __instance.base_icon.sprite = SpriteTextureLoader.getSprite("moh_nameplate");
            }
        }
}