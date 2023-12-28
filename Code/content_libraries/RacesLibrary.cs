using System.Collections.Generic;
using HarmonyLib;
using ReflectionUtility;
using UnityEngine;
using UnityEngine.UI;
//using sfx;


namespace Figurebox
{

    class RacesLibrary
    {
        internal static List<string> defaultRaces = new List<string>()
        {
            "human",
            "elf",
            "orc",
            "dwarf"
        };
        internal static List<string> human = new List<string>()
        {
            "human"
        };
        internal static List<string> additionalRaces = new List<string>()
        {
            "Xia"
        };
        private static Race tRace;
        internal void init()
        {


            Race Xia = AssetManager.raceLibrary.clone("Xia", "human");

            tRace = Xia;
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

            // Xia.build_order_id = "Xia";
            //  Reflection.CallStaticMethod(typeof(BannerGenerator), "loadTexturesFromResources", "xia");

            //clans/


            Xia.name_template_city = "Xia_city";
            Xia.name_template_kingdom = "Xia_kingdom";
            Xia.name_template_culture = "human_culture";
            Xia.name_template_clan = "human_clan";
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
                "unit_male_1", "unit_male_2", "unit_male_3", "unit_male_4", "unit_male_5", "unit_male_6", "unit_male_7", "unit_male_8", "unit_male_9", "unit_male_10"
            }); //"unit_male_0",
            Xia.skin_citizen_female = List.Of<string>(new string[]
            {
                "unit_female_1", "unit_female_2", "unit_female_3", "unit_female_4", "unit_female_5", "unit_female_6", "unit_female_7", "unit_female_8", "unit_female_9", "unit_female_10"
            });
            Xia.skin_warrior = List.Of<string>(new string[]
            {
                "unit_warrior_1", "unit_warrior_2", "unit_warrior_3", "unit_warrior_4", "unit_warrior_5", "unit_warrior_6", "unit_warrior_7", "unit_warrior_8", "unit_warrior_9", "unit_warrior_10"
            });
            Xia.nomad_kingdom_id = $"nomads_{Xia.id}";
            AssetManager.raceLibrary.CallMethod("setPreferredStatPool", "diplomacy#1,warfare#1,stewardship#1,intelligence#1");
            AssetManager.raceLibrary.CallMethod("setPreferredFoodPool", "bread#1,fish#1,tea#1");
            AssetManager.raceLibrary.CallMethod("addPreferredWeapon", "sword", 10);
            AssetManager.raceLibrary.CallMethod("addPreferredWeapon", "ge", 10);
            AssetManager.raceLibrary.CallMethod("addPreferredWeapon", "bow", 5);
            AssetManager.raceLibrary.addBuildingOrderKey(SB.order_tent, "tent_Xia");
            AssetManager.raceLibrary.addBuildingOrderKey(SB.order_house_0, "house_Xia");
            AssetManager.raceLibrary.addBuildingOrderKey(SB.order_house_1, "1house_Xia");
            AssetManager.raceLibrary.addBuildingOrderKey(SB.order_house_2, "2house_Xia");
            AssetManager.raceLibrary.addBuildingOrderKey(SB.order_house_3, "3house_Xia");
            AssetManager.raceLibrary.addBuildingOrderKey(SB.order_house_4, "4house_Xia");
            AssetManager.raceLibrary.addBuildingOrderKey(SB.order_house_5, "5house_Xia");
            AssetManager.raceLibrary.addBuildingOrderKey(SB.order_hall_0, "hall_Xia");
            AssetManager.raceLibrary.addBuildingOrderKey(SB.order_hall_1, "1hall_Xia");
            AssetManager.raceLibrary.addBuildingOrderKey(SB.order_hall_2, "2hall_Xia");
            AssetManager.raceLibrary.addBuildingOrderKey(SB.order_windmill_0, "windmill_Xia");
            AssetManager.raceLibrary.addBuildingOrderKey(SB.order_windmill_1, "windmill_Xia");
            AssetManager.raceLibrary.addBuildingOrderKey(SB.order_docks_0, "fishing_docks_Xia");
            AssetManager.raceLibrary.addBuildingOrderKey(SB.order_docks_1, "docks_Xia");
            AssetManager.raceLibrary.addBuildingOrderKey(SB.order_watch_tower, "watch_tower_Xia");
            AssetManager.raceLibrary.addBuildingOrderKey(SB.order_barracks, "barracks_Xia");
            AssetManager.raceLibrary.addBuildingOrderKey(SB.order_temple, "temple_Xia");
            AssetManager.raceLibrary.addBuildingOrderKey(SB.order_statue, SB.statue);
            AssetManager.raceLibrary.addBuildingOrderKey(SB.order_well, SB.well);
            AssetManager.raceLibrary.addBuildingOrderKey(SB.order_bonfire, SB.bonfire);
            AssetManager.raceLibrary.addBuildingOrderKey(SB.order_mine, SB.mine);
            var monk = AssetManager.actor_library.get("monkey");
            var monkname = AssetManager.nameGenerator.get("monkey_name");
            monkname.vowels = new string[]
            {
                "bibi", "longtail", "pigtail", "monyet", "monpai", "monk", "woooo", "oo", "aa"
            };
            // Race monkey = AssetManager.raceLibrary.clone("monkeyrace", "human");
            // monkey.path_icon  = "ui/Icons/iconMonkey";
            // monk.race ="monkeyrace";
            // AssetManager.raceLibrary.CallMethod("setPreferredFoodPool", "bananas#1,fish#1");

            //monkname.templates.Add("monpai");



            //BannerGenerator.loadTexturesFromResources("xia");

        }

        /* public static void kingdomColorsDataInit()
        {
            KingdomColorsData kingdomColorsData = JsonUtility.FromJson<KingdomColorsData>(ResourcesHelper.LoadTextAsset("colors/kingdom_colors.json"));
            Dictionary<string, KingdomColorContainer> dict = (Dictionary<string, KingdomColorContainer>)typeof(KingdomColors).GetField("dict", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
            foreach (KingdomColorContainer kingdomColorContainer in kingdomColorsData.colors)
            {
                foreach (KingdomColor kingdomColor in kingdomColorContainer.list)
                {
                    kingdomColor.initColor();
                }
                dict.Add(kingdomColorContainer.race, kingdomColorContainer);
            }
        }*/
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ActorAnimationLoader), "loadAnimationBoat")]
        public static bool loadAnimationBoat_Prefix(ref string pTexturePath, ActorAnimationLoader __instance)
        {
            if (pTexturePath.EndsWith("_"))
            {
                pTexturePath = pTexturePath + "human";
                return true;
            }
            foreach (string race in Main.instance.addRaces)
            {
                if (pTexturePath.Contains("Xia"))
                {
                    pTexturePath = pTexturePath.Replace(race, "Xia");
                    return true;

                }
                else if (pTexturePath.Contains(race))
                {
                    pTexturePath = pTexturePath.Replace(race, "human");
                    return true;
                }
            }
            return true;
        }


        /*[HarmonyPrefix]
        [HarmonyPatch(typeof(CityBehBuild), "upgradeBuilding")]
        public static bool generateFrameData_post(Building pBuilding, City pCity)
        {
            if (pBuilding == null || pBuilding.asset == null || pBuilding.asset.upgradeTo == null)
            {
                Debug.LogError("One of the building parameters is null: pBuilding: " + (pBuilding == null ? "null" : "not null") + ", pBuilding.asset: " + (pBuilding.asset == null ? "null" : "not null") + ", pBuilding.asset.upgradeTo: " + (pBuilding.asset.upgradeTo == null ? "null" : "not null"));
                return false;
            }

            Debug.Log("Building asset name: " + pBuilding.asset.id + ", upgradeTo: " + pBuilding.asset.upgradeTo);

            BuildingAsset buildingAsset = AssetManager.buildings.get(pBuilding.asset.upgradeTo);
            if (buildingAsset == null)
            {
                Debug.LogError("Asset not found in lib buildings: " + pBuilding.asset.upgradeTo);
                return false;
            }
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ActorBase), "checkAnimationContainer")]
        public static bool PrefixGetSpriteToRender(ActorBase __instance)
        { Debug.Log("Actor skin id: " + __instance.getUnitTexturePath());
           /* if (__instance.animationContainer != null && __instance.animationContainer.walking != null)
            {
                ActorAnimation actorAnimation = __instance.animationContainer.walking;
                if (actorAnimation.frames == null || actorAnimation.frames.Length == 0)
                {
                    Debug.Log("ActorAnimation walking frames is null or empty(cancer cell)");
                    Debug.Log("Actor name: " + __instance.data.name);
                    Debug.Log("Actor gender: " + __instance.data.gender);
                    Debug.Log("Actor profession: " + __instance.data.profession);
                    Debug.Log("Actor : " + __instance);
                    Debug.Log("ActorRace : " + __instance.race.nameLocale);

                    Debug.Log("Actor head: " + __instance.id_sprite_head);
                    if( __instance.city!=null){
                        Debug.Log("Actor city: " + __instance.city.data.name);
                    Debug.Log("Actor city Race : " + __instance.city.race.nameLocale);

                    }
                    __instance.data.favorite=true;

                    __instance.killHimself(true, AttackType.Other, true, true, true);

                    return false;
                }
            }

            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Culture), "createSkins")]
        public static void PreCreateSkins(Culture __instance)
        {




                Debug.Log("Race of Actor: " + __instance.data.race);

            // Add more Debug.Log statements as needed
        }*/


        [HarmonyPrefix]
        [HarmonyPatch(typeof(BannerLoaderClans), "create")]
        public static bool Prefix(BannerLoaderClans __instance)
        {

            if (__instance._created)
            {
                return true;
            }
            else
            {
                Debug.Log("改变clan框架");
                GameObject frame = __instance.transform.Find("Frame").gameObject;


                Image frameImage = frame.GetComponent<Image>();


                Sprite replacementSprite = Resources.Load<Sprite>("ui/Icons/clan_frame");
                frameImage.sprite = replacementSprite;


                return true;
            }
        }
    }
}