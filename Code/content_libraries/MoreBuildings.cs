using System.Collections.Generic;
namespace Figurebox
{
    class MoreBuildings
    {

        private List<BuildingAsset> humanBuildings = new List<BuildingAsset>();
        //private static List<BuildOrder> _possible_buildings;
        //private static List<BuildOrder> _possible_buildings_no_resources = new List<BuildOrder>();
        //private static List<TileZone> _possible_zones = new List<TileZone>();

        public void init()
        {

            //Main.harmony.Patch(AccessTools.Method(typeof(CityBehBuild), "calcPossibleBuildings"), prefix: new HarmonyMethod(AccessTools.Method(typeof(BuildOrderLibraryRB), nameof(calcPossibleBuildings_prefix))));         
            foreach (BuildingAsset humanBuilding in AssetManager.buildings.list)
            {
                if (humanBuilding.race == "human")
                {
                    humanBuildings.Add(humanBuilding);
                }
            }
            tower_init();
            null_init();
            foreach (var race in RacesLibrary.additionalRaces)
            {
                if (race == "Xia")
                {
                    initXia();


                }




                // var buildingOrder = AssetManager.race_build_orders.clone(race, "human");
                //buildingOrder.replace("human", race);
            }


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
        private void null_init()
        {
            BuildingAsset nullbuild = AssetManager.buildings.clone("nullbuild", "!city_colored_building");
            nullbuild.base_stats[S.health] = 1000f;
            nullbuild.id = "nullbuild";
            // nullbuild.base_stats[S.targets] = 1f;
            //nullbuild.baseStats.attackSpeed = 30000f;
            nullbuild.base_stats[S.area_of_effect] = 1f;
            // nullbuild.tower_projectile = "FireArrow";
            //   nullbuild.tower_projectile_offset = 4f;
            //  nullbuild.tower_projectile_amount = 6;
            //  nullbuild.base_stats[S.damage] = 50f;
            // nullbuild.base_stats[S.knockback] = 1.4f;
            //nullbuild.priority = 114514;
            // nullbuild.tech = "building_watch_tower";
            nullbuild.fundament = new BuildingFundament(1, 1, 1, 0);
            nullbuild.cost = new ConstructionCost(0, 0, 0, 0);
            nullbuild.type = "windmill";
            nullbuild.race = "Xia";
            nullbuild.upgradedFrom = "windmill_Xia";
            nullbuild.build_place_borders = true;
            nullbuild.build_place_batch = false;
            nullbuild.build_place_single = true;
            nullbuild.max_zone_range = 1;
            nullbuild.canBePlacedOnLiquid = false;
            nullbuild.ignoreBuildings = false;
            nullbuild.checkForCloseBuilding = false;
            nullbuild.canBeLivingHouse = false;
            nullbuild.burnable = false;
            nullbuild.spawnUnits = false;
            nullbuild.shadow = true;
            nullbuild.canBeUpgraded = false;
            AssetManager.buildings.add(nullbuild);
            AssetManager.buildings.loadSprites(nullbuild);

            //nullbuild.setShadow(0.5f, 0.23f, 0.27f);
            // AssetManager.race_build_orders.get("Tian").addBuilding("Barrack1_Tian", 0, 2, 50, 10, false, false, 0);


        }

        private static void initXia()
        {

            RaceBuildOrderAsset pAsset = new RaceBuildOrderAsset();
            pAsset.id = "Xia";
            AssetManager.race_build_orders.add(pAsset);
            pAsset.addBuilding("bonfire", 1);
            pAsset.addBuilding("tent_Xia", pHouseLimit: true);
            BuildOrderLibrary.b.requirements_orders = List.Of<string>("bonfire");
            pAsset.addUpgrade("tent_Xia");
            BuildOrderLibrary.b.requirements_orders = List.Of<string>("tent_Xia");
            //pAsset.addUpgrade("house_Xia");
            addVariantsUpgrade(pAsset, "house_Xia", List.Of<string>("hall_Xia"));
            //BuildOrderLibrary.b.requirements_orders = List.Of<string>("hall_Xia");
            //pAsset.addUpgrade("1house_human");
            addVariantsUpgrade(pAsset, "1house_Xia", List.Of<string>("1hall_Xia"));
            //BuildOrderLibrary.b.requirements_orders = List.Of<string>("1hall_Xia");
            //pAsset.addUpgrade("2house_human");
            addVariantsUpgrade(pAsset, "2house_Xia", List.Of<string>("1hall_Xia"));
            //BuildOrderLibrary.b.requirements_orders = List.Of<string>("1hall_Xia");
            //pAsset.addUpgrade("3house_human");
            addVariantsUpgrade(pAsset, "3house_Xia", List.Of<string>("2hall_Xia"));
            //BuildOrderLibrary.b.requirements_orders = List.Of<string>("2hall_Xia");
            //pAsset.addUpgrade("4house_human");
            addVariantsUpgrade(pAsset, "4house_Xia", List.Of<string>("2hall_Xia"));
            // addVariantsUpgrade(pAsset, "5house_Xia", List.Of<string>("2hall_Xia"));
            //BuildOrderLibrary.b.requirements_orders = List.Of<string>("2hall_Xia");
            pAsset.addUpgrade("hall_Xia", pPop: 30, pBuildings: 8);
            //BuildOrderLibrary.b.requirements_orders = List.Of<string>("1house_human");
            pAsset.addUpgrade("1hall_Xia", pPop: 100, pBuildings: 20);
            BuildOrderLibrary.b.requirements_orders = List.Of<string>("statue", "mine", "barracks_Xia");
            pAsset.addUpgrade("fishing_docks_Xia");
            BuildOrderLibrary.b.requirements_orders = List.Of<string>("fishing_docks_Xia");
            pAsset.addBuilding("windmill_Xia", 1, pPop: 6, pBuildings: 5);
            BuildOrderLibrary.b.requirements_orders = List.Of<string>("bonfire");
            pAsset.addUpgrade("windmill_Xia", pPop: 40, pBuildings: 10);
            pAsset.addBuilding("nullbuild", 1, pPop: 30, pBuildings: 10);
            pAsset.addUpgrade("nullbuild", pPop: 40, pBuildings: 10);
            pAsset.addBuilding("fishing_docks_Xia", 5, pBuildings: 2);
            BuildOrderLibrary.b.requirements_orders = List.Of<string>("bonfire");
            pAsset.addBuilding("well", 1, pPop: 20, pBuildings: 10);
            BuildOrderLibrary.b.requirements_types = List.Of<string>("hall");
            pAsset.addBuilding("hall_Xia", 1, pPop: 10, pBuildings: 6);
            // pAsset.addBuilding("5house_Xia", 1, pPop: 10, pBuildings: 6);
            BuildOrderLibrary.b.requirements_orders = List.Of<string>("bonfire");
            BuildOrderLibrary.b.requirements_types = List.Of<string>("house");
            pAsset.addBuilding("mine", 1, pPop: 20, pBuildings: 10);
            BuildOrderLibrary.b.requirements_orders = List.Of<string>("bonfire", "hall_Xia");
            pAsset.addBuilding("barracks_Xia", 1, pPop: 50, pBuildings: 16, pMinZones: 20);
            BuildOrderLibrary.b.requirements_orders = List.Of<string>("1hall_Xia");
            pAsset.addBuilding("watch_tower_Xia", 1, pPop: 30, pBuildings: 10);
            //pAsset.addUpgrade("watch_tower_Xia", 0, 0, 0, 0, false, false, 0);
            //BuildOrderLibrary.b.requirements_orders = List.Of<string>("watch_tower_Xia");
            pAsset.addBuilding("XiaTower", 1, pPop: 30, pBuildings: 10);
            pAsset.addUpgrade("XiaTower");
            //addVariantsUpgrade(pAsset, "watch_tower_Xia", List.Of<string>("watch_tower_Xia"));
            pAsset.addUpgrade("watch_tower_Xia", 0, 0, 3, 3);

            BuildOrderLibrary.b.requirements_orders = List.Of<string>("bonfire", "hall_Xia");
            pAsset.addBuilding("temple_Xia", 1, pPop: 90, pBuildings: 20, pMinZones: 20);
            BuildOrderLibrary.b.requirements_orders = List.Of<string>("bonfire", "1hall_Xia", "statue");
            pAsset.addBuilding("statue", 1, pPop: 70, pBuildings: 15);
            BuildOrderLibrary.b.requirements_orders = List.Of<string>("1hall_Xia");


        }


        /* private void loadSprites(BuildingAsset pTemplate)
         {
             string folder = pTemplate.race;
             if (folder == string.Empty)
             {
                 folder = "Others";
             }
             folder = folder + "/" + pTemplate.id;
             Sprite[] array = Utils.ResourcesHelper.loadAllSprite("buildings/" + folder, 0.5f, 0.0f);

             pTemplate.sprites = new BuildingSprites();
             foreach (Sprite sprite in array)
             {
                 string[] array2 = sprite.name.Split(new char[] { '_' });
                 string text = array2[0];
                 int num = int.Parse(array2[1]);

                 if (array2.Length == 3)
                 {
                     int.Parse(array2[2]);
                 }
                 while (pTemplate.sprites.animationData.Count < num + 1)
                 {
                     pTemplate.sprites.animationData.Add(null);
                 }
                 if (pTemplate.sprites.animationData[num] == null)
                 {
                     pTemplate.sprites.animationData[num] = new BuildingAnimationDataNew();
                 }
                 BuildingAnimationDataNew buildingAnimationDataNew = pTemplate.sprites.animationData[num];
                 if (text.Equals("main"))
                 {
                     buildingAnimationDataNew.list_main.Add(sprite);
                     if (buildingAnimationDataNew.list_main.Count > 1)
                     {
                         buildingAnimationDataNew.animated = true;
                     }
                 }
                 else if (text.Equals("ruin"))
                 {
                     buildingAnimationDataNew.list_ruins.Add(sprite);
                 }
                 //else if (text.Equals("shadow"))
                 //{
                 //    buildingAnimationDataNew.list_shadows.Add(sprite);
                 //}
                 else if (text.Equals("special"))
                 {
                     buildingAnimationDataNew.list_special.Add(sprite);
                 }
                 else if (text.Equals("mini"))
                 {
                     pTemplate.sprites.mapIcon = new BuildingMapIcon(sprite);
                 }
             }
             foreach (BuildingAnimationDataNew buildingAnimationDataNew2 in pTemplate.sprites.animationData)
             {
                 buildingAnimationDataNew2.main = buildingAnimationDataNew2.list_main.ToArray();
                 buildingAnimationDataNew2.ruins = buildingAnimationDataNew2.list_ruins.ToArray();
                 //buildingAnimationDataNew2.shadows = buildingAnimationDataNew2.list_shadows.ToArray();
                 buildingAnimationDataNew2.special = buildingAnimationDataNew2.list_special.ToArray();
             }

         }*/

        private static void addVariantsUpgrade(RaceBuildOrderAsset pAsset, string name, List<string> requirementsBuildings)
        {
            foreach (var race in RacesLibrary.defaultRaces)
            {
                pAsset.addUpgrade($"{name}_{race}");
                BuildOrderLibrary.b.requirements_orders = requirementsBuildings;
            }
        }
    }
}