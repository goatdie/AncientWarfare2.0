using Figurebox.Utils;
using Figurebox;
using System;
using System.Linq;
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
namespace Figurebox
{
    internal class MoreActors
    {
       
        
         public void init()
        {
            ColorSetAsset Xia_default = new ColorSetAsset();
            Xia_default.id = "xia_default";
			Xia_default.shades_from = "#FFC984";
			Xia_default.shades_to = "#543E2C";
			Xia_default.is_default = true;
            AssetManager.skin_color_set_library.add(Xia_default);

             var XiaAsset = AssetManager.actor_library.clone("unit_Xia", "unit_human");
            XiaAsset.nameLocale = "Xias";
            XiaAsset.body_separate_part_head = true;
            XiaAsset.heads = 11;
            //XiaAssetexture_heads = "t_Xia_heads";
            XiaAsset.oceanCreature = false;
            //XiaAsset.procreate = true;
            XiaAsset.nameTemplate = "Xia_name";
            XiaAsset.race = "Xia";
            XiaAsset.fmod_spawn = "event:/SFX/UNITS/Human/HumanSpawn";
		    XiaAsset.fmod_attack = "event:/SFX/UNITS/Human/HumanAttack";
	     	XiaAsset.fmod_idle = "event:/SFX/UNITS/Human/HumanIdle";
	    	XiaAsset.fmod_death = "event:/SFX/UNITS/Human/HumanDeath";
            //XiaAssetexture_path = "t_Xia";
            //XiaAssetexture_heads = string.Empty;
            XiaAsset.base_stats[S.max_age] = 90f;
            XiaAsset.base_stats[S.max_children] = 6f;
            XiaAsset.icon = "iconXias";
            XiaAsset.color = Toolbox.makeColor("#33724D");
            XiaAsset.setBaseStats(100, 15, 45, 5, 90, 5);
            XiaAsset.base_stats[S.attack_speed] = 60f;
            XiaAsset.disableJumpAnimation = true;
            AssetManager.actor_library.addColorSet("xia_default");
            AssetManager.actor_library.CallMethod("loadShadow", XiaAsset);
            AssetManager.actor_library.add(XiaAsset);
            Localization.addLocalization(XiaAsset.nameLocale, XiaAsset.nameLocale);
            //XiaAssetraits.Add("zho");

            var babbyXia = AssetManager.actor_library.clone("baby_Xia", "unit_Xia");
            babbyXia.body_separate_part_head = false;
            babbyXia.body_separate_part_hands = false;
            babbyXia.take_items = false;
            babbyXia.base_stats[S.speed] = 10f;
           // babbyXia.timeToGrow = 60;
            babbyXia.baby = true;
            babbyXia.animation_idle = "walk_3";
            babbyXia.growIntoID = "unit_Xia";
            AssetManager.actor_library.CallMethod("addTrait", "peaceful");
            babbyXia.disableJumpAnimation = true;
            babbyXia.color_sets = XiaAsset.color_sets;
            AssetManager.actor_library.CallMethod("loadShadow", babbyXia);

           // loadMultiraceUnitsSprites();
         
        }
       
      /*  private void addColor(ActorAsset pStats, string pID = "default", string from = "#FFC984", string to = "#543E2C")
        {
            pStats.useSkinColors = true;
            if (pStats.color_sets == null)
            {
                pStats.color_sets = new List<ColorSet>();
            }
            ColorSet colorSet = new ColorSet();
            colorSet.id = pID;
            pStats.color_sets.Add(colorSet);
            Color pFrom = Toolbox.makeColor(from);
            Color pTo = Toolbox.makeColor(to);
            int num = 5;
            float num2 = 1f / (float)(num - 1);
            for (int i = 0; i < num; i++)
            {
                float num3 = 1f - (float)i * num2;
                if (num3 > 1f)
                {
                    num3 = 1f;
                }
                Color c = Toolbox.blendColor(pFrom, pTo, num3);
                colorSet.colors.Add(c);
            }
        }*/
        
         private static void loadMultiraceUnitsSprites(){
              var spritesFoldersList = new List<string>(){
                "unit_child", "unit_king", "unit_leader",
                "heads_special"
            };

            var spritesHeadsList = new List<string>(){
                "heads_female", //"heads_female_santa",
                "heads_male", //"heads_male_santa",
            };

            var spritesFoldersSpecialList = new List<string>(){
                "unit_female_","unit_male_", "unit_warrior_"
            };

            foreach(var raceName in RacesLibrary.human){
                var races = raceName.Split('-');

                int i = 1;
                foreach(var race in races){
                    foreach(var spritesFolder in spritesFoldersList){
                        addUnitsSpritesToResources(spritesFolder, raceName, race);
                    }

                    foreach(var spritesFolder in spritesHeadsList){
                        addUnitsSpritesToResources(spritesFolder, raceName, race, true);
                    }

                    foreach(var spritesFolder in spritesFoldersSpecialList){
                        addUnitsSpritesToResources($"{spritesFolder}{i}", raceName, race);
                    }

                    i++;
                }


            }
        }

           private static void addUnitsSpritesToResources(string spritesFolder, string raceName, string race, bool isHeads = false, bool isBodies = false){
            var unitsSprites = Resources.LoadAll<Sprite>($"actors/races/{race}/{spritesFolder}");
            var addedSrites = Resources.LoadAll<Sprite>($"actors/races/{raceName}/{spritesFolder}");
            
            var addedCount = addedSrites is null ? 0 : addedSrites.Length;

            //headIndex = headIndex == -1 ? -1 : headIndex * unitsSprites.Length;
            var headIndexOffset = !isHeads ? -1 : addedCount;
            //headIndex = !headIndex ? -1 : addedCount;

            var pivot = new Vector2(0.5f, 0);
            foreach(var sprite in unitsSprites){
                var spriteName = !isHeads ? sprite.name : sprite.name.Split('_')[0];

                if(headIndexOffset != -1){


                    //pivot = sprite.pivot;
                    pivot = new Vector2(0, 0);
                    var newHeadName = sprite.name.Split('_')[0];
                    var newHeadIndex = Int32.Parse(sprite.name.Split('_')[1]);

                    spriteName = $"{newHeadName}_{newHeadIndex + headIndexOffset}";

                    // if(raceName == "orc-dwarf"){
                    //     Debug.LogWarning($"path: actors/races/{race}/{spritesFolder}/{spriteName}, rect: {sprite.rect}, pivot: {sprite.pivot}");
                    // }
                }

                var texture = Resources.Load<Texture2D>($"actors/races/{race}/");
                var newSprite = Sprite.Create(texture, sprite.rect, pivot, sprite.pixelsPerUnit);
                //var newSprite = Sprite.Create(texture, sprite.rect, sprite.pivot, sprite.pixelsPerUnit);
                //newSprite.name = spriteName;

                var newPath = $"actors/races/{raceName}/{spritesFolder}/{spriteName}";

                
               
            }
        }


        
        
        

       
    }
    
}
