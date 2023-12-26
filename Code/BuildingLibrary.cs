using System;
using System.Collections.Generic;
using NCMS.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using ReflectionUtility;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using HarmonyLib;
//using sfx;

namespace Figurebox
{
    class BuildingLibrary
    {
        public void init()
        {
            loadRaceBuildings();
        }
        private BuildingAsset get(string pID)
        {
            return AssetManager.buildings.get(pID);
        }


        private void loadRaceBuildings()
        {
            /*BuildingAsset temple_X = AssetManager.buildings.get("temple_Xia");
            temple_X.fundament = new BuildingFundament(5, 5, 8, 0);*/

            var buildingTypesSimple = new string[]{
                "watch_tower",
                "docks",
                "barracks",
                "temple",
                "windmill"
            };

            var buildingTypesExtended = new List<(string, string, BuildingFundament, ConstructionCost)>{
                ("tent", "house",   new BuildingFundament(1, 1, 2, 0), new ConstructionCost()),
                ( "house", "1house",  new BuildingFundament(2, 2, 3, 0), new ConstructionCost(5)),
                ( "1house", "2house", new BuildingFundament(2, 2, 2, 0), new ConstructionCost(4)),
                ( "2house", "3house", new BuildingFundament(2, 2, 2, 0), new ConstructionCost(pStone: 5)),
                ( "3house", "4house", new BuildingFundament(2, 3, 2, 0), new ConstructionCost(pStone: 10)),
                ( "4house", "5house", new BuildingFundament(3, 3, 2, 0), new ConstructionCost(pStone: 15)),
                ( "5house", null, new BuildingFundament(3, 3, 2, 0), new ConstructionCost(pStone: 20, pCommonMetals: 2, pGold: 10)),
                ( "hall", "1hall",   new BuildingFundament(6, 6, 4, 0), new ConstructionCost(10, pGold: 10)),
                ( "1hall", "2hall",  new BuildingFundament(6, 6, 4, 0), new ConstructionCost(pStone: 10, pCommonMetals: 1, pGold: 20)),
                ( "2hall", null, new BuildingFundament(6, 6, 4, 0), new ConstructionCost(pStone: 15, pCommonMetals: 1, pGold: 100)),
                ( "fishing_docks", "docks", new BuildingFundament(2, 2, 4, 0), new ConstructionCost(10)),
            };

            foreach (var race in RacesLibrary.additionalRaces)
            {
                foreach (var buildingType in buildingTypesSimple)
                {

                    BuildingAsset building_base;
                    string Base_Building = SB.watch_tower_human;
                    if (buildingType == "docks") { Base_Building = SB.docks_human; }
                    if (buildingType == "barracks") { Base_Building = SB.barracks_human; }
                    if (buildingType == "temple") { Base_Building = SB.temple_human; }
                    if (buildingType == "windmill")
                    {
                        Base_Building = SB.windmill_human_1;
                        building_base = AssetManager.buildings.get(SB.windmill_human_1);
                    }
                    else { building_base = AssetManager.buildings.get($"{buildingType}_human"); }
                    var building = AssetManager.buildings.clone($"{buildingType}_{race}", Base_Building);
                    building.race = race;
                    building.canBeUpgraded = false;

                    loadSprites(building);
                }

                foreach (var buildingType in buildingTypesExtended)
                {
                    //int SD = buildingType.Item5;
                    string Base_Building = SB.tent_human;
                    if (buildingType.Item1 == "house") { Base_Building = SB.house_human_0; }
                    if (buildingType.Item1 == "house1") { Base_Building = SB.house_human_1; }
                    if (buildingType.Item1 == "house2") { Base_Building = SB.house_human_2; }
                    if (buildingType.Item1 == "house3") { Base_Building = SB.house_human_3; }
                    if (buildingType.Item1 == "house4") { Base_Building = SB.house_human_4; }
                    if (buildingType.Item1 == "house5") { Base_Building = SB.house_human_5; }
                    if (buildingType.Item1 == "hall") { Base_Building = SB.hall_human_0; }
                    if (buildingType.Item1 == "1hall") { Base_Building = SB.hall_human_1; }
                    if (buildingType.Item1 == "2hall") { Base_Building = SB.hall_human_2; }
                    if (buildingType.Item1 == "fishing_docks") { Base_Building = SB.fishing_docks_human; }
                    var buildingFundament = buildingType.Item3;
                    var building_base = AssetManager.buildings.get(Base_Building);
                    if (building_base != null)
                    {
                        var building = AssetManager.buildings.clone($"{buildingType.Item1}_{race}", Base_Building);
                        building.race = race;
                        building.fundament = buildingFundament;
                        building.cost = buildingType.Item4;

                        if (!String.IsNullOrEmpty(buildingType.Item2))
                        {

                            building.canBeUpgraded = true;
                            building.upgradeTo = $"{buildingType.Item2}_{race}";

                        }
                        else
                        {
                            building.canBeUpgraded = false;
                            //  building.upgradeTo =null;
                        }

                        AssetManager.buildings.loadSprites(building);
                        // loadSprites(building);


                        AssetManager.race_build_orders.get("Xia").addUpgrade("watch_tower_Xia", 0, 0, 2, 2, false, false, 0);
                        // AssetManager.race_build_orders.get("Xia").addUpgrade("Tower" , 0, 0, 30, 8, false, false, 0);
                        BuildingAsset t = AssetManager.buildings.get("watch_tower_Xia");

                        t.canBeUpgraded = true;
                        t.tower_projectile = "FireArrow";
                        t.upgradeTo = "XiaTower";
                        BuildingAsset temple_xia = AssetManager.buildings.get("temple_Xia");
                        temple_xia.fundament = new BuildingFundament(4, 3, 9, 0);
                        BuildingAsset windmill = AssetManager.buildings.get("windmill_Xia");
                        windmill.upgradeTo = "nullbuild";
                    }
                }
            }


        }



        private static Dictionary<string, Sprite[]> cached_sprite_list;
        internal static void loadSprites(BuildingAsset pTemplate)
        {
            if (cached_sprite_list is null)
            {
                cached_sprite_list = Reflection.GetField(typeof(SpriteTextureLoader), null, "cached_sprite_list") as Dictionary<string, Sprite[]>;
            }

            string pPath = pTemplate.sprite_path;
            if (String.IsNullOrEmpty(pPath))
            {
                pPath = $"buildings/{pTemplate.id}";
            }

            if (cached_sprite_list.ContainsKey(pPath))
            {
                cached_sprite_list.Remove(pPath);
            }

            AssetManager.buildings.loadSprites(pTemplate);
        }
        // private static void loadSprites(BuildingAsset pTemplate)
        // {   
        //     AssetManager.buildings.CallMethod("loadSprites", pTemplate);


        //     return;

        //     // for(int i = 0; i < pTemplate.sprites.animationData.Count; i++){
        //     //     var sprites = pTemplate.sprites.animationData[i];
        //     //     for(int j = 0; j < sprites.list_main.Count; j++){
        //     //         var sprite = sprites.list_main[j];

        //     //         sprites.list_main[j] = Sprite.Create(sprite.texture, new Rect(0.0f, 0.0f, (float)sprite.texture.width, (float)sprite.texture.height), new Vector2(0.5f, 0f), 1f);
        //     //     }


        //     //     for(int j = 0; j < sprites.list_shadows.Count; j++){
        //     //         var sprite = sprites.list_shadows[j];

        //     //         sprites.list_shadows[j] = Sprite.Create(sprite.texture, new Rect(0.0f, 0.0f, (float)sprite.texture.width, (float)sprite.texture.height), new Vector2(0.5f, 0f), 1f);
        //     //     }


        //     //     for(int j = 0; j < sprites.list_ruins.Count; j++){
        //     //         var sprite = sprites.list_ruins[j];

        //     //         sprites.list_ruins[j] = Sprite.Create(sprite.texture, new Rect(0.0f, 0.0f, (float)sprite.texture.width, (float)sprite.texture.height), new Vector2(0.5f, 0f), 1f);
        //     //     }


        //     //     for(int j = 0; j < sprites.list_special.Count; j++){
        //     //         var sprite = sprites.list_special[j];

        //     //         sprites.list_special[j] = Sprite.Create(sprite.texture, new Rect(0.0f, 0.0f, (float)sprite.texture.width, (float)sprite.texture.height), new Vector2(0.5f, 0f), 1f);
        //     //     }


        //     // }

        //     // foreach (BuildingAnimationDataNew animationDataNew in pTemplate.sprites.animationData)
        //     // {
        //     //     animationDataNew.main = animationDataNew.list_main.ToArray();
        //     //     animationDataNew.ruins = animationDataNew.list_ruins.ToArray();
        //     //     animationDataNew.shadows = animationDataNew.list_shadows.ToArray();
        //     //     animationDataNew.special = animationDataNew.list_special.ToArray();
        //     // }
        // }
    }
}