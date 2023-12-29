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
    {
        AW_Kingdom awKingdom = pKingdom as AW_Kingdom;

        if (MoHTools.IsMoHKingdom(awKingdom))
        {

            __instance.base_icon.sprite = SpriteTextureLoader.getSprite("moh_nameplate");
        }
    }
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Kingdom), "updateAge")]

    public static void MOHUpdateAge_Postfix(Kingdom __instance)
    {
        AW_Kingdom awKingdom = __instance as AW_Kingdom;

        if (MoHTools.IsMoHKingdom(awKingdom))
        {
            int index = 0;
            if (awKingdom.getEnemiesKingdoms().Count == 0)
            {
                index++;
            }

            if (awKingdom.king != null)
            {

                if (awKingdom.king.hasTrait("first"))
                {
                    index += 3;
                }
                if (awKingdom.king.getAge() <= 24)
                {
                    index--;
                }

            }
            Clan kclan = BehaviourActionBase<Kingdom>.world.clans.get(awKingdom.data.royal_clan_id);
            if (kclan != null && kclan.units.Count <= 2)
            {
                index--;

            }
            MoHTools.ChangeMOH_Value(index);
        }
    }
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Kingdom), "updateAge")]

    public static void MOH_condition_Postfix(Kingdom __instance)
    {
        if (MoHTools.MOH_Value >= MoHTools.MOH_UnderLimit)
        {
            MoHTools.SetMOH_Value(MoHTools.MOH_UnderLimit);
        }
        AW_Kingdom awKingdom = __instance as AW_Kingdom;
        if (MoHTools.IsMoHKingdom(awKingdom))
        {
            if (MoHTools.MOH_Value >= 40)
            {
                //天命值在什么条件发生的事件
            }
            if (MoHTools.MOH_Value <= MoHTools.MOH_UnderLimit)
            {
                MoHTools.Clear_MoHKingdom();                //天命去除
            }
        }


    }
}