using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Figurebox
{
    class MoreBuildings
    {
        public void init()
        {
            tower_init();
            add_Xia();
        }

        private static void add_Xia()
        {
            clone_human_buildings("Xia");
            AssetManager.buildings.get("tent_Xia").fundament = new BuildingFundament(1, 1, 1, 0);
            AssetManager.buildings.get("house_Xia_0").fundament = new BuildingFundament(3, 3, 4, 0);
            AssetManager.buildings.get("house_Xia_1").fundament = new BuildingFundament(3, 3, 4, 0);
            AssetManager.buildings.get("house_Xia_2").fundament = new BuildingFundament(3, 3, 4, 0);
            AssetManager.buildings.get("house_Xia_3").fundament = new BuildingFundament(4, 4, 6, 0);
            AssetManager.buildings.get("house_Xia_4").fundament = new BuildingFundament(5, 5, 9, 0);
            AssetManager.buildings.get("house_Xia_5").fundament = new BuildingFundament(5, 5, 9, 0);
            AssetManager.buildings.get("hall_Xia_0").fundament = new BuildingFundament(4, 4, 7, 0);
            AssetManager.buildings.get("hall_Xia_1").fundament = new BuildingFundament(5, 5, 9, 0);
            AssetManager.buildings.get("hall_Xia_2").fundament = new BuildingFundament(8, 8, 14, 0);
            AssetManager.buildings.get("temple_Xia").fundament = new BuildingFundament(3, 3, 5, 0);
            AssetManager.buildings.get("barracks_Xia").fundament = new BuildingFundament(3, 3, 7, 0);
            AssetManager.buildings.get("windmill_Xia_0").fundament = new BuildingFundament(2, 1, 2, 0);
            AssetManager.buildings.get("windmill_Xia_1").fundament = new BuildingFundament(2, 2, 2, 0);
            AssetManager.buildings.get("watch_tower_Xia").fundament = new BuildingFundament(2, 2, 3, 0);
        }

        private void tower_init()
        {
            BuildingAsset XiaTower = AssetManager.buildings.clone("XiaTower", "!city_colored_building");
            XiaTower.base_stats[S.health] = 1000f;
            XiaTower.id = "XiaTower";
            XiaTower.base_stats[S.targets] = 1f;
            //XiaTower.base_stats[S.attackSpeed] = 30f;
            XiaTower.base_stats[S.area_of_effect] = 1f;
            XiaTower.tower_projectile = "FireArrow";
            XiaTower.tower_projectile_offset = 4f;
            XiaTower.tower_projectile_amount = 6;
            XiaTower.base_stats[S.damage] = 50f;
            XiaTower.base_stats[S.knockback] = 1.4f;
            XiaTower.priority = 114514;
            XiaTower.tech = "building_watch_tower";
            XiaTower.fundament = new BuildingFundament(1, 1, 1, 0);
            XiaTower.cost = new ConstructionCost(5, 5, 0, 0);
            XiaTower.type = "watch_tower";
            XiaTower.race = "Xia";
            XiaTower.build_place_borders = true;
            XiaTower.build_place_batch = false;
            XiaTower.build_place_single = true;
            XiaTower.upgradedFrom = "watch_tower_Xia";
            XiaTower.max_zone_range = 2;
            XiaTower.canBePlacedOnLiquid = false;
            XiaTower.ignoreBuildings = false;
            XiaTower.checkForCloseBuilding = false;
            XiaTower.canBeLivingHouse = false;
            XiaTower.burnable = false;
            XiaTower.spawnUnits = false;
            XiaTower.shadow = true;
            AssetManager.buildings.add(XiaTower);
            AssetManager.buildings.loadSprites(XiaTower);

            //XiaTower.setShadow(0.5f, 0.23f, 0.27f);
            // AssetManager.race_build_orders.get("Tian").addBuilding("Barrack1_Tian", 0, 2, 50, 10, false, false, 0);
        }

        private static void clone_human_buildings(string race)
        {
            List<BuildingAsset> human_buildings = AssetManager.buildings.list
                .Where(building => building.race == "human").ToList();

            foreach (BuildingAsset building in human_buildings)
            {
                var test_path = building.sprite_path;
                if (string.IsNullOrEmpty(test_path))
                    test_path = "buildings/" + building.id.Replace(SK.human, race);
                if (Resources.LoadAll<Sprite>(test_path) is not { Length: > 0 }) continue;

                BuildingAsset new_building =
                    AssetManager.buildings.clone(building.id.Replace(SK.human, race),
                        building.id);

                new_building.race = race;

                if (building.canBeUpgraded)
                    new_building.upgradeTo = new_building.upgradeTo.Replace(SK.human, race);

                if (!string.IsNullOrEmpty(new_building.upgradedFrom))
                    new_building.upgradedFrom =
                        new_building.upgradedFrom.Replace(SK.human, race);

                AssetManager.buildings.loadSprites(new_building);
            }
        }
    }
}