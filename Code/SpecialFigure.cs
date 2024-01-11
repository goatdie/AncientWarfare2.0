using System.Collections.Generic;
using Figurebox.Utils;
using HarmonyLib;
using NCMS.Utils;
using UnityEngine;
using Figurebox.Utils.MoH;
namespace Figurebox
{

  internal class SpecialFigure
  {
    public static bool togglefigurevalue = true;
    public static Dictionary<string, bool> SpawnedNames = new Dictionary<string, bool>();
    public static string savedFamilyName = "";
    public int setCount = 0;
    public int nameCount
    {
      get
      {
        return SpawnedNames.Count;
      }
    }
    public static void togglefigure()
    {
      if (PowerButtons.GetToggleValue("historical_figure"))
      {
        togglefigurevalue = false;
        return;
      }
      togglefigurevalue = true;
    }

    /* [HarmonyPrefix]
     [HarmonyPatch(typeof(Clan), "addUnit")]
     public static bool generateName_Prefix(Actor pActor)
     {
       if (pActor.hasTrait("figure") && pActor.data.profession == UnitProfession.Baby)
       {
         string temp;
         pActor.data.get("chinese_family_name", out temp, "");
         Debug.Log("子答辩姓氏" + temp);
         if (!string.IsNullOrEmpty(temp))
         {
           savedFamilyName = temp;
           pActor.data.set("chinese_family_name", savedFamilyName);
         }

         // savedFamilyName = ""; // 重置 savedFamilyName 为 空字符串
         return false;
       }
       if (pActor.hasTrait("figure") && pActor.data.profession != UnitProfession.Baby)
       {
         string temp;
         Debug.Log("姓氏" + savedFamilyName);

         pActor.data.get("chinese_family_name", out temp, "");
         Debug.Log("人答辩姓氏" + temp);
         if (!string.IsNullOrEmpty(temp))
         {
           savedFamilyName = temp;
           pActor.data.set("chinese_family_name", savedFamilyName);
         }
         savedFamilyName = ""; // 重置 savedFamilyName 为 空字符串
         return true;
       }

       return true;
     }

     [HarmonyPrefix]
     [HarmonyPatch(typeof(Clan), "createClan")]
     public static bool createclan_Prefix(Actor pFounder)
     {
       if (pFounder.hasTrait("figure") && pFounder.data.profession != UnitProfession.Baby)
       {
         string temp;
         Debug.Log("家族姓氏" + savedFamilyName);
         pFounder.data.get("chinese_family_name", out temp, "");
         Debug.Log("创建clan答辩姓氏" + temp);
         if (!string.IsNullOrEmpty(temp) && string.IsNullOrEmpty(savedFamilyName))
         {
           savedFamilyName = temp;
           pFounder.data.set("chinese_family_name", savedFamilyName);
         }
         else
         {
           pFounder.data.set("chinese_family_name", savedFamilyName);
         }
         //savedFamilyName = ""; // 重置 savedFamilyName 为 空字符串
         return true;
       }
       return true;
     }*/
    [HarmonyPostfix]
    [HarmonyPatch(typeof(MapIconLibrary), "drawKings")]
    public static void drawfigure_Postfix(MapIconLibrary __instance, MapIconAsset pAsset)
    {
      Sprite newIcon = SpriteTextureLoader.getSprite("civ/icons/minimap_figure");
      List<Kingdom> list_civs = World.world.kingdoms.list_civs;
      foreach (Kingdom k in list_civs)
      {
        List<Actor> actorList = k.units.getSimpleList();
        foreach (Actor actor in actorList)
        {
          if (!(actor == null) && actor.isAlive() && !actor.isInMagnet() && (!actor.isKing()&&!actor.isCityLeader()) && actor.hasTrait("first"))
          {
            Vector3 pPos = actor.currentPosition;
            pPos.y -= 3f;

            GroupSpriteObject groupSpriteObject = MapIconLibrary.drawMark(pAsset, pPos, null, k, actor.city, null, 0.7f, false, -1f);
            Sprite spriteIcon = UnitSpriteConstructor.getSpriteIcon(newIcon, k.getColor());
            groupSpriteObject.setSprite(spriteIcon);
          }
        }
      }
    }


    [HarmonyPostfix]
    [HarmonyPatch(typeof(ActorManager), "spawnPopPoint")]
    public static void Historical_Postfix(ActorManager __instance, ActorData pData, Actor __result)
    {
      if (togglefigurevalue == false) { return; }
      ActorData actorStatus = __result.data;
      Race race = __result.race;
      BaseStats actorStats = __result.stats; //狗马把一堆名字换了
      List<Actor> actorList = new List<Actor>();
      ActorContainer actorContainer = race.units;
      actorList.AddRange(actorContainer.getSimpleList());
      bool hasFirstTrait = false;
      Clan clan = BehaviourActionBase<Kingdom>.world.clans.get(pData.clan);
      foreach (Actor actor in actorList)
      {
        if (actor.hasTrait("figure") || actor.hasTrait("first") || MoHTools.MoHKingdom!=null)
        { //Debug.Log("有人来"+actor.getName());
          hasFirstTrait = true;
          break;
        }
      }

      if (race.civilization && !hasFirstTrait)
      {

        string text0 = "姬发"; // this for the name(The Ricardo is a template for how to make your own figure)
        if ((Toolbox.randomChance(0.8f)) && !SpawnedNames.ContainsKey(text0)) // random stuff
        {
          if (__result == null)
          {
            return;
          }
          //actorStatus.getName();
          __result.data.setName(text0);
          actorStatus.set("kingdom_name", "周");
          actorStatus.set("family_name", "姬");
          actorStatus.set("clan_name", "姬");
          actorStatus.set("name_set", true);
          actorStatus.health = 1500;
          actorStatus.favorite = true;
          SpawnedNames.Add(text0, true);
          __result.addTrait("figure");
          __result.addTrait("first"); // here give the fugure traits
          CityTools.logFigure(__result); // world message stuff(don't forget to add this if you wanna see your figure message)					
          //actorStatus.set("chinese_family_name", "姬");

          return;

        }
        string text2 = "嬴政";
        if ((Toolbox.randomChance(0.005f)) && !SpawnedNames.ContainsKey(text2))
        {
          if (__result == null)
          {
            return;
          }
          actorStatus.setName(text2);
          actorStatus.set("kingdom_name", "秦");
          actorStatus.set("family_name", "嬴");
          actorStatus.set("clan_name", "趙");
          actorStatus.set("name_set", true);
          actorStatus.favorite = true;
          actorStatus.health = 1500;
          SpawnedNames.Add(text2, true);
          __result.addTrait("figure");
          __result.addTrait("first");
          CityTools.logFigure(__result);
          return;
        }

        string text3 = "刘邦";
        if ((Toolbox.randomChance(0.005f)) && !SpawnedNames.ContainsKey(text3))
        {
          if (__result == null)
          {
            return;
          }
          actorStatus.name = text3;
          actorStatus.set("kingdom_name", "漢");
          actorStatus.set("chinese_family_name", "刘");
          actorStatus.set("family_name", "刘");
          actorStatus.set("clan_name", "刘");
          actorStatus.set("name_set", true);
          SpawnedNames.Add(text3, true);
          __result.addTrait("figure");
          actorStatus.favorite = true;
          actorStatus.health = 1500;
          __result.addTrait("first");
          CityTools.logFigure(__result);
          return;
        }

        string text5 = "曹丕";
        if ((Toolbox.randomChance(0.005f)) && !SpawnedNames.ContainsKey(text5))
        {
          if (__result == null)
          {
            return;
          }
          actorStatus.name = text5;
          SpawnedNames.Add(text5, true);
          actorStatus.favorite = true;
          actorStatus.health = 1500;
          actorStatus.set("kingdom_name", "魏");
          actorStatus.set("chinese_family_name", "曹");
          actorStatus.set("family_name", "曹");
          actorStatus.set("clan_name", "曹");
          actorStatus.set("name_set", true);
          __result.addTrait("figure");
          __result.addTrait("first");

          CityTools.logFigure(__result);



          return;
        }
        text5 = "司马炎";
        if ((Toolbox.randomChance(0.005f)) && !SpawnedNames.ContainsKey(text5))
        {
          if (__result == null)
          {
            return;
          }
          actorStatus.name = text5;
          SpawnedNames.Add(text5, true);
          actorStatus.favorite = true;
          actorStatus.health = 1500;
          actorStatus.set("kingdom_name", "晋");
          actorStatus.set("chinese_family_name", "司马");
          actorStatus.set("family_name", "司马");
          actorStatus.set("clan_name", "司马");
          actorStatus.set("name_set", true);
          __result.addTrait("figure");
          __result.addTrait("first");

          CityTools.logFigure(__result);


          return;
        } /*
         text5 = "Don Jiang";
            if ((Toolbox.randomChance(0.008f)) && !SpecialFigure.SpawnedNames.ContainsKey(text5))
        {
             if (__result == null)
            {
                    return;
                }
            actorStatus.name = text5;
            SpecialFigure.SpawnedNames.Add(text5, true);
            __result.addTrait("figure");
            __result.addTrait("rat");
            __result.addTrait("lust");
            __result.addTrait("strong_minded");
            if(__result.hasTrait("figure"))
            {
                CityTools.logFigure(__result);
                }

            return;
        }
        text5 = "Zhejiang Rabbit";
            if ((Toolbox.randomChance(0.008f)) && !SpecialFigure.SpawnedNames.ContainsKey(text5))
        {
             if (__result == null)
            {
                    return;
                }
            actorStatus.name = text5;
            SpecialFigure.SpawnedNames.Add(text5, true);
            __result.addTrait("figure");
            __result.addTrait("ratking");
            __result.addTrait("madness");
             CityTools.logFigure(__result);


            return;
        }
        text5 = "CaoWei";
            if ((Toolbox.randomChance(0.008f)) && !SpecialFigure.SpawnedNames.ContainsKey(text5))
        {
             if (__result == null)
            {
                    return;
                }
            actorStatus.name = text5;
            SpecialFigure.SpawnedNames.Add(text5, true);
            __result.addTrait("figure");
            __result.addTrait("stupid");
            __result.addTrait("greedy");
            __result.addTrait("lust");
             CityTools.logFigure(__result);


            return;
        }
        text5 = "EnderBlack";
            if ((Toolbox.randomChance(0.008f)) && !SpecialFigure.SpawnedNames.ContainsKey(text5))
        {
             if (__result == null)
            {
                    return;
                }
            actorStatus.name = text5;
            SpecialFigure.SpawnedNames.Add(text5, true);
            __result.addTrait("figure");
            __result.addTrait("greedy");
             CityTools.logFigure(__result);


            return;
        }
        text5 = "SuXi";
            if ((Toolbox.randomChance(0.008f)) && !SpecialFigure.SpawnedNames.ContainsKey(text5))
        {
             if (__result == null)
            {
                    return;
                }
            actorStatus.name = text5;
            SpecialFigure.SpawnedNames.Add(text5, true);
            __result.addTrait("figure");
            __result.addTrait("ambitious");
            __result.addTrait("unlucky");
            __result.addTrait("lust");
            __result.addTrait("attractive");
             CityTools.logFigure(__result);


            return;
        }
        text5 = "Rat";
            if ((Toolbox.randomChance(0.008f)) && !SpecialFigure.SpawnedNames.ContainsKey(text5))
        {
             if (__result == null)
            {
                    return;
                }
            actorStatus.name = text5;
            SpecialFigure.SpawnedNames.Add(text5, true);
            __result.addTrait("figure");
            __result.addTrait("rat");
            __result.addTrait("unlucky");
            __result.addTrait("attractive");
             CityTools.logFigure(__result);


            return;
        }
    */
      }

    }
  }

}