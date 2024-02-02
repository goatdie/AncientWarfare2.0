using Figurebox.core;
using Figurebox.Utils.MoH;
using HarmonyLib;

namespace Figurebox.patch.MoH;

internal static class MoHCorePatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(MapText), "showTextKingdom")]
    public static void SetTianmingicon_Postfix(MapText __instance, Kingdom pKingdom)
    {
        AW_Kingdom awKingdom = pKingdom as AW_Kingdom;

        if (MoHTools.IsMoHKingdom(awKingdom))
        {
            __instance.base_icon.sprite = SpriteTextureLoader.getSprite("moh_nameplate");
        }
    }

    internal static void check_and_add_moh_trait(AW_Kingdom pKingdom, Actor pKing)
    {
        if (MoHTools.IsMoHKingdom(pKingdom)) pKing.addTrait("天命");
    }

    


}