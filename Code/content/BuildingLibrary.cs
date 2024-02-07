using System;
using System.Collections.Generic;
using ReflectionUtility;
using UnityEngine;

//using sfx;

namespace Figurebox.content
{
    class BuildingLibrary
    {
        public static void init()
        {
            var buildingTypesSimple = new string[] { "watch_tower", "docks", "barracks", "temple" };
            var buildingTypesExtended =
                new List<(string, string, BuildingFundament, ConstructionCost, string, string, int)>
                {
                    ("tent", "house", new BuildingFundament(1, 1, 2, 0), new ConstructionCost(), "", "_0", -1),
                    ("house", "house", new BuildingFundament(2, 2, 3, 0), new ConstructionCost(5), "_0", "_1", 0),
                    ("house", "house", new BuildingFundament(2, 2, 2, 0), new ConstructionCost(4), "_1", "_2", 1),
                    ("house", "house", new BuildingFundament(2, 2, 2, 0), new ConstructionCost(pStone: 5), "_2", "_3",
                        2),
                    ("house", "house", new BuildingFundament(2, 3, 2, 0), new ConstructionCost(pStone: 10), "_3", "_4",
                        3),
                    ("house", "house", new BuildingFundament(3, 3, 2, 0), new ConstructionCost(pStone: 15), "_4", "_5",
                        4),
                    ("house", null, new BuildingFundament(3, 3, 2, 0),
                        new ConstructionCost(pStone: 20, pCommonMetals: 2, pGold: 10), "_5", "", 5),
                    ("hall", "hall", new BuildingFundament(6, 6, 4, 0), new ConstructionCost(10, pGold: 10), "_0", "_1",
                        0),
                    ("hall", "hall", new BuildingFundament(6, 6, 4, 0),
                        new ConstructionCost(pStone: 10, pCommonMetals: 1, pGold: 20), "_1", "_2", 1),
                    ("hall", null, new BuildingFundament(6, 6, 4, 0),
                        new ConstructionCost(pStone: 15, pCommonMetals: 1, pGold: 100), "_2", "", 2),
                    ("fishing_docks", "docks", new BuildingFundament(2, 2, 4, 0), new ConstructionCost(10), "", "", -1),
                    ("windmill", "windmill", new BuildingFundament(2, 2, 4, 0), new ConstructionCost(10), "_0", "_1",
                        -1),
                    ("windmill", null, new BuildingFundament(2, 2, 4, 0), new ConstructionCost(10), "_1", "", -1),
                };
            foreach (var race in RacesLibrary.additionalRaces)
            {
                foreach (var buildingType in buildingTypesSimple)
                {
                    BuildingAsset building_base;
                    string Base_Building = SB.watch_tower_human;
                    if (buildingType == "docks")
                    {
                        Base_Building = SB.docks_human;
                    }

                    if (buildingType == "barracks")
                    {
                        Base_Building = SB.barracks_human;
                    }

                    if (buildingType == "temple")
                    {
                        Base_Building = SB.temple_human;
                    }

                    building_base = AssetManager.buildings.get($"{buildingType}_human");
                    var building = AssetManager.buildings.clone($"{buildingType}_{race}", Base_Building);
                    if (race == "Xia")
                    {
                        building.base_stats[S.health] = building_base.base_stats[S.health] * 3;
                        building.burnable = false;
                        building.affectedByAcid = false;
                        building.canBeDamagedByTornado = false;
                    }

                    building.race = race;
                    building.hasKingdomColor = false;
                    building.setShadow(0f, 0.3f, 0.3f);
                    building.canBeUpgraded = false;
                    loadSprites(building);
                    if (buildingType == "docks")
                    {
                        building.upgradedFrom = $"fishing_docks_{race}";
                    }
                }

                foreach (var buildingType in buildingTypesExtended)
                {
                    string Base_Building = SB.tent_human;
                    string IDI = buildingType.Item1 + buildingType.Item5;
                    Base_Building = IDI switch
                    {
                        "house_0" => SB.house_human_0,
                        "house_1" => SB.house_human_1,
                        "house_2" => SB.house_human_2,
                        "house_3" => SB.house_human_3,
                        "house_4" => SB.house_human_4,
                        "house_5" => SB.house_human_5,
                        "hall_0" => SB.hall_human_0,
                        "hall_1" => SB.hall_human_1,
                        "hall_2" => SB.hall_human_2,
                        "fishing_docks" => SB.fishing_docks_human,
                        "windmill_0" => SB.windmill_human_0,
                        "windmill_1" => SB.windmill_human_1,
                        _ => Base_Building
                    };

                    if (buildingType.Item1 == null) continue;
                    var building = AssetManager.buildings.clone($"{buildingType.Item1}_{race}{buildingType.Item5}",
                        Base_Building);
                    building.race = race;
                    building.setShadow(0f, 0.3f, 0.3f);
                    building.hasKingdomColor = false;
                    building.fundament = buildingType.Item3;
                    building.cost = buildingType.Item4;
                    if (buildingType.Item2 != null)
                    {
                        building.canBeUpgraded = true;
                        building.upgradeTo = $"{buildingType.Item2}_{race}{buildingType.Item6}";
                    }
                    else
                    {
                        building.canBeUpgraded = false;
                    }

                    loadSprites(building);
                    if (race != "Xia") continue;
                    string SD = $"{buildingType.Item7}";
                    if (buildingType.Item7 < 1)
                    {
                        SD = "";
                    }

                    var build = AssetManager.buildings.clone($"{SD}{buildingType.Item1}_{race}", Base_Building);
                    build.race = race;
                    build.setShadow(0f, 0.3f, 0.3f);
                    build.fundament = buildingType.Item3;
                    build.cost = buildingType.Item4;
                    if (buildingType.Item2 != null && IDI != "windmill_0")
                    {
                        build.canBeUpgraded = true;
                        build.upgradeTo = $"{buildingType.Item2}_{race}{buildingType.Item6}";
                    }
                    else
                    {
                        build.canBeUpgraded = false;
                    }

                    loadSprites(build);
                }
            }
        }


        private static Dictionary<string, Sprite[]> cached_sprite_list;

        private static void loadSprites(BuildingAsset pTemplate)
        {
            if (cached_sprite_list is null)
            {
                cached_sprite_list =
                    SpriteTextureLoader.cached_sprite_list;
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
    }
}