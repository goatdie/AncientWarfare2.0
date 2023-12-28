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
private static List<BuildingAsset> humanBuildings = new List<BuildingAsset>();

    public static void init()
    {
      var buildingTypesSimple = new string[] { "watch_tower", "docks", "barracks", "temple" };
      var buildingTypesExtended = new List<(string, string, BuildingFundament, ConstructionCost, string, string, int)>
            {
              ("tent", "house",   new BuildingFundament(1, 1, 2, 0), new ConstructionCost(), "" , "_0", -1),
              ( "house", "house",  new BuildingFundament(2, 2, 3, 0), new ConstructionCost(5), "_0", "_1", 0),
              ( "house", "house", new BuildingFundament(2, 2, 2, 0), new ConstructionCost(4), "_1", "_2", 1),
              ( "house", "house", new BuildingFundament(2, 2, 2, 0), new ConstructionCost(pStone: 5), "_2", "_3", 2),
              ( "house", "house", new BuildingFundament(2, 3, 2, 0), new ConstructionCost(pStone: 10), "_3", "_4", 3),
              ( "house", "house", new BuildingFundament(3, 3, 2, 0), new ConstructionCost(pStone: 15), "_4", "_5", 4),
              ( "house", null, new BuildingFundament(3, 3, 2, 0), new ConstructionCost(pStone: 20, pCommonMetals: 2, pGold: 10), "_5", "", 5),
              ( "hall", "hall",   new BuildingFundament(6, 6, 4, 0), new ConstructionCost(10, pGold: 10), "_0", "_1", 0),
              ( "hall", "hall",  new BuildingFundament(6, 6, 4, 0), new ConstructionCost(pStone: 10, pCommonMetals: 1, pGold: 20), "_1", "_2", 1),
              ( "hall", null, new BuildingFundament(6, 6, 4, 0), new ConstructionCost(pStone: 15, pCommonMetals: 1, pGold: 100), "_2", "", 2),
              ( "fishing_docks", "docks", new BuildingFundament(2, 2, 4, 0), new ConstructionCost(10), "", "", -1),
              ( "windmill", "windmill", new BuildingFundament(2, 2, 4, 0), new ConstructionCost(10), "_0", "_1", -1),
              ( "windmill", null, new BuildingFundament(2, 2, 4, 0), new ConstructionCost(10), "_1", "", -1),
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
          if (buildingType == "docks") { building.upgradedFrom = $"fishing_docks_{race}"; }
        }
        foreach (var buildingType in buildingTypesExtended)
        {
          string Base_Building = SB.tent_human;
          string IDI = buildingType.Item1 + buildingType.Item5;
          if (IDI == "house_0") { Base_Building = SB.house_human_0; }
          if (IDI == "house_1") { Base_Building = SB.house_human_1; }
          if (IDI == "house_2") { Base_Building = SB.house_human_2; }
          if (IDI == "house_3") { Base_Building = SB.house_human_3; }
          if (IDI == "house_4") { Base_Building = SB.house_human_4; }
          if (IDI == "house_5") { Base_Building = SB.house_human_5; }
          if (IDI == "hall_0") { Base_Building = SB.hall_human_0; }
          if (IDI == "hall_1") { Base_Building = SB.hall_human_1; }
          if (IDI == "hall_2") { Base_Building = SB.hall_human_2; }
          if (IDI == "fishing_docks") { Base_Building = SB.fishing_docks_human; }
          if (IDI == "windmill_0") { Base_Building = SB.windmill_human_0; }
          if (IDI == "windmill_1") { Base_Building = SB.windmill_human_1; }
          var building_base = AssetManager.buildings.get(Base_Building);
          if (buildingType.Item1 != null)
          {
            var building = AssetManager.buildings.clone($"{buildingType.Item1}_{race}{buildingType.Item5}", Base_Building);
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
            else { building.canBeUpgraded = false; }
            loadSprites(building);
            if (race == "Xia")
            {
              string SD = $"{buildingType.Item7}"; if (buildingType.Item7 < 1) { SD = ""; }
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
              else { build.canBeUpgraded = false; }
              loadSprites(build);
            }
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