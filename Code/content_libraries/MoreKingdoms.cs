using HarmonyLib;
using ReflectionUtility;
using System;
using NCMS.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Reflection.Emit;
using Figurebox.Utils;
namespace Figurebox
{
    internal class MoreKingdoms
    {

        internal void init()
        {
            

         
                var kingdomAsset = AssetManager.kingdoms.get("human");
                kingdomAsset.addFriendlyTag("Xia");
               

                var kingdomNomadsAsset = AssetManager.kingdoms.get("nomads_human");
                kingdomNomadsAsset.addFriendlyTag("Xia");
               
            
         
            AssetManager.kingdoms.add(new KingdomAsset
            {
                id = "empty",
                civ = true
            });
            AssetManager.kingdoms.add(new KingdomAsset
            {
                id = "nomads_empty",
                nomads = true,
                mobs = true
            });
            #region Xia
            //主要国家
            KingdomAsset addKingdom7 = AssetManager.kingdoms.clone("Xia", "empty");
            addKingdom7.addTag("civ");
            addKingdom7.addTag("Xia");
            addKingdom7.addFriendlyTag("human");
            addKingdom7.addFriendlyTag("Xia");
            addKingdom7.addFriendlyTag("neutral");
            addKingdom7.addFriendlyTag("good");
            addKingdom7.addEnemyTag("bandits");
            newHiddenKingdom(addKingdom7);
            //临时用的国家
            KingdomAsset addKingdom8 = AssetManager.kingdoms.clone("nomads_Xia", "nomads_empty");
            addKingdom8.addTag("civ");
            addKingdom8.addTag("Xia");
            addKingdom8.addFriendlyTag("Xia");
            addKingdom8.addFriendlyTag("human");
            addKingdom8.addFriendlyTag("neutral");
            addKingdom8.addFriendlyTag("good");
            addKingdom8.addEnemyTag("bandits");
            newHiddenKingdom(addKingdom8);
            #endregion
            

           //initColors();
            //BannerGenerator.loadBanners($"{Main.mainPath}/EmbededResources/banners");
        }
        private Kingdom newHiddenKingdom(KingdomAsset pAsset)
        {
            Kingdom kingdom = World.world.kingdoms.newObject(pAsset.id);
            kingdom.asset = pAsset;
            kingdom.createHidden();
           // kingdom.id = pAsset.id;
            kingdom.data.name = pAsset.id;
            KingdomManager kingdomManager = MapBox.instance.kingdoms;
            kingdomManager.setupKingdom(kingdom, false);
            return kingdom;
        }
         
          /* private static void initColors(){
            var XiaKingdomColors = new KingdomColorContainer(){
                race = "Xia",
                curColor = 0,
                list = new List<KingdomColor>()
                {
                   new KingdomColor("#54A63D", "#6ED452", "#F0E32B"){id = 0, name = "Xia_color_0"},
                    new KingdomColor("#D8DA25", "#7EDA25", "#DA8125"){id = 1, name = "Xia_color_1"},
                    new KingdomColor("#CE4F31", "#CE9E31", "#CE3161"){id = 2, name = "Xia_color_2"},
                    new KingdomColor("#D006F9", "#F906A9", "#5606F9"){id = 3, name = "Xia_color_3"},
                    new KingdomColor("#1965E6", "#3319E6", "#19CCE6"){id = 4, name = "Xia_color_4"},
                }
            };
            addColor(XiaKingdomColors);

        }
        private static void addColor(KingdomColorContainer kColors){
            var dict = Reflection.GetField(typeof(KingdomColors), null, "dict") as Dictionary<string, KingdomColorContainer>;
            foreach (KingdomColor kingdomColor in kColors.list){
                kingdomColor.initColor();
            }
            
            dict.Add(kColors.race, kColors);
        }*/
       
        
     
       
    }

}
