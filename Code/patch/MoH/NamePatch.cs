using System.Collections.Generic;
using Figurebox.Utils;
using HarmonyLib;
using UnityEngine;
using Figurebox.core;

#if 一米_中文名
using Chinese_Name;
#endif
namespace Figurebox
{
    public class NamePatch
    {
#if 一米_中文名
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Clan), nameof(Clan.createClan))]
        public static void ModifyClanCreation(Clan __instance, Actor pFounder)
        {
            if (pFounder == null)
            {
                Debug.Log("NOT FOUND FOUNDER");
                return;
            }
            if (pFounder.city == null)
            {
                Debug.Log("NOT FOUND FOUNDER City");
                return;
            }
            // 如果创建者是Xia种族
            if (pFounder.asset.nameTemplate.Contains("Xia"))
            {
                // 为创始人设置姓和氏
                SetNameForActor(pFounder, __instance);
                string existingChineseFamilyName;
                pFounder.data.get("chinese_family_name", out existingChineseFamilyName, "");


                // 设置Clan名称
                string familyName;
                string clanName;

                pFounder.data.get("family_name", out familyName, "");
                pFounder.data.get("clan_name", out clanName, "");
                if (string.IsNullOrEmpty(existingChineseFamilyName))
                {
                    __instance.data.name = pFounder.city.data.name + familyName + "姓" + clanName + "氏";
                }
                else
                {
                    __instance.data.name = pFounder.city.data.name + familyName + "氏";
                    __instance.data.set("clan_chinese_family_name", existingChineseFamilyName);
                }
                // 存储姓氏和氏到Clan中
                __instance.data.set("clan_family_name", familyName);
                __instance.data.set("clan_clan_name", clanName);
            }



        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Clan), "addUnit")]
        public static void ModifyClanAddition(Clan __instance, Actor pActor)
        {
            // 如果新成员是Xia种族
            if (pActor.asset.nameTemplate.Contains("Xia"))
            {
                // 为新成员设置姓和氏
                SetNameForActor(pActor, __instance);
            }
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Clan), "addUnit")]
        public static bool PreventHisFigure(Clan __instance, Actor pActor)
        {
            // 不让历史人物变成贵族
            if (pActor.hasTrait("figure") && pActor.getAge() <= 18)
            {
                World.world.clans.removeUnit(pActor.data);
                return false;
            }
            return true;
        }

        public static void SetNameForActor(Actor actor, Clan pClan)
        {
            bool nameSet;
            actor.data.get("name_set", out nameSet, false);
            if (nameSet)
            {
                return; // 名字已经被设置，无需再次设置
            }

            string familyName, clanName;
            string existingFamilyName, existingClanName;
            string existingChineseFamilyName;
            string ClanChineseFamilyName;
            actor.data.get("chinese_family_name", out existingChineseFamilyName, "");
            actor.data.get("family_name", out existingFamilyName, "");
            actor.data.get("clan_name", out existingClanName, "");

            // 尝试从Clan的data中获取姓氏和氏
            pClan.data.get("clan_family_name", out familyName, "");
            pClan.data.get("clan_clan_name", out clanName, "");
            pClan.data.get("clan_chinese_family_name", out ClanChineseFamilyName, "");

            // 如果Clan和actor中没有姓氏或氏，为角色生成新的姓氏或氏
            if (string.IsNullOrEmpty(familyName) && string.IsNullOrEmpty(existingFamilyName))
            {
                familyName = WordLibraryManager.GetRandomWord("姓");
            }
            if (string.IsNullOrEmpty(clanName) && string.IsNullOrEmpty(existingClanName))
            {
                // 使用UnityEngine的随机数生成器决定选择哪种方式
                clanName = Random.Range(0, 2) == 0 ? actor.city.data.name[0].ToString() : WordLibraryManager.GetRandomWord("氏");
            }

            // 设置姓氏和氏到actor.data中
            if (string.IsNullOrEmpty(existingChineseFamilyName))
            {
                actor.data.set("family_name", familyName);
                actor.data.set("clan_name", clanName);
            }
            if (!string.IsNullOrEmpty(ClanChineseFamilyName))
            {
                actor.data.set("family_name", ClanChineseFamilyName);
                actor.data.set("clan_name", ClanChineseFamilyName);
                actor.data.set("chinese_family_name", ClanChineseFamilyName);
            }

            if (!string.IsNullOrEmpty(existingChineseFamilyName))
            {
                return; // 如果已经有中文姓氏，不再进行命名
            }
            AW_Kingdom awKingdom = actor.kingdom as AW_Kingdom;
            if (awKingdom.NameIntegration)
            {
                return;
            }
            // 根据性别设置名字
            string givenName = actor.getName();
            string finalName = actor.data.gender == ActorGender.Male ? clanName + givenName : givenName + familyName;

            actor.data.set("name_set", true);
            // 检查是否已经设置了中文姓氏



            actor.data.setName(finalName);
        }




        [HarmonyPostfix]
        [HarmonyPatch(typeof(WindowCreatureInfo), "OnEnable")]
        public static void PostfixOnEnable(WindowCreatureInfo __instance)
        {
            Actor pActor = __instance.actor;

            // 检查角色是否为Xia种族
            if (pActor.asset.nameTemplate.Contains("Xia"))
            {
                // 获取姓
                string existingFamilyName;
                pActor.data.get("family_name", out existingFamilyName, "");

                // 获取氏
                string clanName;
                pActor.data.get("clan_name", out clanName, "");

                // 如果姓或氏为null或空字符串，则将其设置为"无"
                if (string.IsNullOrEmpty(existingFamilyName))
                {
                    existingFamilyName = "无";
                }

                if (string.IsNullOrEmpty(clanName))
                {
                    clanName = "无";
                }

                // 显示姓和氏
                __instance.showStat("familyname", existingFamilyName);
                __instance.showStat("clanname", clanName);
            }
        }
#endif
    }
}