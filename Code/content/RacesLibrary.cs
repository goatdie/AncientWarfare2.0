using System.Collections.Generic;
using Figurebox.abstracts;
using HarmonyLib;
using ReflectionUtility;
using UnityEngine;
using UnityEngine.UI;

//using sfx;


namespace Figurebox.content
{
    class RacesLibrary : ExtendedLibrary<Race>
    {
        internal static readonly List<string> default_races = new List<string>()
        {
            "human",
            "elf",
            "orc",
            "dwarf"
        };

        internal static List<string> additionalRaces = new List<string>()
        {
            "Xia"
        };


        public static void AssignNameTemplateKingdom(string kingdomName)
        {
            foreach (var raceName in default_races)
            {
                Race race = AssetManager.raceLibrary.get(raceName);
                if (race != null)
                {
                    race.name_template_kingdom = kingdomName;
                }
            }
        }

        protected override void init()
        {
            AssignNameTemplateKingdom("Xia_kingdom");
            Race Xia = AssetManager.raceLibrary.clone("Xia", "human");

            Xia.civ_baseCities = 1;
            Xia.civ_base_army_mod = 0.5f;
            Xia.civ_base_zone_range = 15;
            //Xia.build_order_id = "Xia";
            // Xia.color = Toolbox.makeColor("#548CFE");
            Xia.build_order_id = "kingdom_base";
            Xia.path_icon = "ui/Icons/iconXias";
            Xia.nameLocale = "Xia";
            Xia.banner_id = "Xia";
            Xia.main_texture_path = "races/Xia/";


#if 一米_中文名
            Xia.name_template_city = "Xia_city";
            Xia.name_template_kingdom = "Xia_kingdom";
            Xia.name_template_culture = "Xia_culture";
            Xia.name_template_clan = "human_clan";
#endif
            Xia.clan_backgrounds = new List<string>
            {
                "actors/races/Xia/clans/backgrounds/clan_background_0",
                "actors/races/Xia/clans/backgrounds/clan_background_1",
                "actors/races/Xia/clans/backgrounds/clan_background_2",
                "actors/races/Xia/clans/backgrounds/clan_background_3",
                "actors/races/Xia/clans/backgrounds/clan_background_4",
                "actors/races/Xia/clans/backgrounds/clan_background_5",
                "actors/races/Xia/clans/backgrounds/clan_background_6",
                "actors/races/Xia/clans/backgrounds/clan_background_7",
                "actors/races/Xia/clans/backgrounds/clan_background_8",
                "actors/races/Xia/clans/backgrounds/clan_background_9",
                "actors/races/Xia/clans/backgrounds/clan_background_10",
                "actors/races/Xia/clans/backgrounds/clan_background_11",
                "actors/races/Xia/clans/backgrounds/clan_background_12",
                "actors/races/Xia/clans/backgrounds/clan_background_13",
                "actors/races/Xia/clans/backgrounds/clan_background_14",
                "actors/races/Xia/clans/backgrounds/clan_background_15",
                "actors/races/Xia/clans/backgrounds/clan_background_16"
            };
            Xia.clan_icons = new List<string>
            {
                "actors/races/Xia/clans/icons/clan_icon_0",
                "actors/races/Xia/clans/icons/clan_icon_1",
                "actors/races/Xia/clans/icons/clan_icon_2",
                "actors/races/Xia/clans/icons/clan_icon_3",
                "actors/races/Xia/clans/icons/clan_icon_4",
                "actors/races/Xia/clans/icons/clan_icon_5",
                "actors/races/Xia/clans/icons/clan_icon_6",
                "actors/races/Xia/clans/icons/clan_icon_7",
                "actors/races/Xia/clans/icons/clan_icon_8",
                "actors/races/Xia/clans/icons/clan_icon_9",
                "actors/races/Xia/clans/icons/clan_icon_10",
                "actors/races/Xia/clans/icons/clan_icon_11",
                "actors/races/Xia/clans/icons/clan_icon_12",
                "actors/races/Xia/clans/icons/clan_icon_13",
                "actors/races/Xia/clans/icons/clan_icon_14",
                "actors/races/Xia/clans/icons/clan_icon_15",
                "actors/races/Xia/clans/icons/clan_icon_16",
                "actors/races/Xia/clans/icons/clan_icon_17",
                "actors/races/Xia/clans/icons/clan_icon_18",
                "actors/races/Xia/clans/icons/clan_icon_19",
                "actors/races/Xia/clans/icons/clan_icon_20",
                "actors/races/Xia/clans/icons/clan_icon_21"
            };
            Xia.hateRaces = List.Of<string>(new string[]
            {
                SK.orc
            });
            Xia.production = new string[]
            {
                "bread", "pie", "tea"
            };
            Xia.skin_citizen_male = List.Of<string>(new string[]
            {
                "unit_male_1", "unit_male_2", "unit_male_3", "unit_male_4", "unit_male_5", "unit_male_6", "unit_male_7",
                "unit_male_8", "unit_male_9", "unit_male_10"
            }); //"unit_male_0",
            Xia.skin_citizen_female = List.Of<string>(new string[]
            {
                "unit_female_1", "unit_female_2", "unit_female_3", "unit_female_4", "unit_female_5", "unit_female_6",
                "unit_female_7", "unit_female_8", "unit_female_9", "unit_female_10"
            });
            Xia.skin_warrior = List.Of<string>(new string[]
            {
                "unit_warrior_1", "unit_warrior_2", "unit_warrior_3", "unit_warrior_4", "unit_warrior_5",
                "unit_warrior_6", "unit_warrior_7", "unit_warrior_8", "unit_warrior_9", "unit_warrior_10"
            });
            Xia.nomad_kingdom_id = $"nomads_{Xia.id}";
            AssetManager.raceLibrary.setPreferredStatPool(
                                                "diplomacy#1,warfare#1,stewardship#1,intelligence#1");
            AssetManager.raceLibrary.setPreferredFoodPool("bread#1,fish#1,tea#1");
            AssetManager.raceLibrary.addPreferredWeapon( "sword", 10);
            AssetManager.raceLibrary.addPreferredWeapon("ge",    10);
            AssetManager.raceLibrary.addPreferredWeapon("bow",   5);
            AssetManager.raceLibrary.cloneBuildingKeys(SK.human, Xia.id);
            AssetManager.nameGenerator.clone("Xia_name", "human_name");

            var monk = AssetManager.actor_library.get("monkey");
            var monkname = AssetManager.nameGenerator.get("monkey_name");
            monkname.vowels = new string[]
            {
                " bibi", " beruk", " longtail", " pigtail", " monyet", " monpai", " monk", " woooo", " oo", " aa"
            };


            //BannerGenerator.loadTexturesFromResources("xia");
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ActorAnimationLoader), "loadAnimationBoat")]
        public static bool loadAnimationBoat_Prefix(ref string pTexturePath, ActorAnimationLoader __instance)
        {
            if (pTexturePath.EndsWith("_"))
            {
                pTexturePath = pTexturePath + "human";
                return true;
            }

            foreach (string race in additionalRaces)
            {
                if (pTexturePath.Contains("Xia"))
                {
                    pTexturePath = pTexturePath.Replace(race, "Xia");
                    return true;
                }

                if (pTexturePath.Contains(race))
                {
                    pTexturePath = pTexturePath.Replace(race, "human");
                    return true;
                }
            }

            return true;
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(BannerLoaderClans), "create")]
        public static bool Prefix(BannerLoaderClans __instance)
        {
            if (__instance._created)
            {
                return true;
            }

            __instance.transform.Find("Frame").gameObject.GetComponent<Image>().sprite = SpriteTextureLoader.getSprite("ui/Icons/clan_frame");
            return true;
        }
    }
}