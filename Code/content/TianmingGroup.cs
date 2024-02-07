using System.Collections.Generic;
using System.Linq;
using ai.behaviours;
using HarmonyLib;
using ReflectionUtility;
using UnityEngine;

namespace Figurebox.content
{
  class TianmingGroup
  {
    public static void init()
    {

      
      //guardJob.addCondition(new CondIsGroupLeader(), false);
      // var cityJobsUnit = (List<string>)Reflection.GetField(typeof(CityTaskList), null, "jobsUnit");



      ProjectileAsset FireArrow = new ProjectileAsset();
      FireArrow.id = "FireArrow";
      FireArrow.speed = 15f;
      FireArrow.texture = "qing";
      FireArrow.parabolic = true;
      FireArrow.texture_shadow = "shadow_arrow";
      FireArrow.terraformOption = "demon_fireball";
      FireArrow.startScale = 0.05f;
      FireArrow.targetScale = 0.19f;
      FireArrow.sound_impact = "event:/SFX/HIT/HitGeneric";
      FireArrow.terraformRange = 0;
      AssetManager.projectiles.add(FireArrow);
    }
    //private bool tryToMakeWarrior(Actor pActor)
    [HarmonyPostfix]
    [HarmonyPatch(typeof(UnitGroup), "update")]
    public static void CheckGroup_Postfix(UnitGroup __instance)
    {
      /*if (__instance.groupLeader != null && __instance.groupLeader.data.profession == UnitProfession.King)
      {
          List<Actor> simpleList = __instance.units.getSimpleList();
          foreach (Actor actor in simpleList)
          {
              if ( actor != null&&actor.city!=null&&actor != __instance.groupLeader)
              {
              }
          }
      }*/
      if (__instance.groupLeader != null && __instance.groupLeader.hasTrait("禁卫军"))
      {
        List<Actor> simpleList = __instance.units.getSimpleList();
        foreach (Actor actor in simpleList)
        {
          if (actor != null)
          {
            actor.removeTrait("禁卫军");
          }
        }
        __instance.disband();
      }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Kingdom), "update")]
    public static void CheckKingGroup_Postfix(Kingdom __instance)
    {
      if (__instance.king == null) { return; }
      var actorGroup = (UnitGroup)Reflection.GetField(typeof(Actor), __instance.king, "unit_group");
      if (actorGroup != null)
      {
        int maxcount = 0;
        int count = actorGroup.countUnits();
        if (__instance.capital != null) { maxcount = __instance.capital.status.warrior_slots / 3; }
        List<Actor> simpleList = __instance.units.getSimpleList();
        foreach (Actor a in simpleList)
        {
          if (a.unit_group != actorGroup && a.kingdom.king != null && a.data.profession == UnitProfession.Warrior && a.city == __instance.king.city && count <= maxcount && !a.hasTrait("禁卫军"))
          {
            actorGroup.addUnit(a);
            a.addTrait("禁卫军");
            a.data.health += 300;
            CitizenJobs jobs = a.city.jobs;
            jobs.addToJob(AssetManager.citizen_job_library.get("guard"), 1);
            jobs.takeJob(AssetManager.citizen_job_library.get("guard"));
            var actorAi = (AiSystemActor)Reflection.GetField(a.GetType(), a, "ai");
            actorAi.setJob("guard");
            ItemAsset wa = AssetManager.items.get("ji");
            ItemAsset aa = AssetManager.items.get("armor");
            string wm = "bronze";
            string am = "bronze";
            ItemData item = ItemGenerator.generateItem(wa, wm, World.world.mapStats.getCurrentYear(), __instance, __instance.king.data.name, 1, a);
            item.modifiers.Clear();
            if (a.equipment.getSlot(wa.equipmentType).data == null || a.equipment.getSlot(aa.equipmentType).data == null)
            {
              a.equipment.getSlot(wa.equipmentType).CallMethod("setItem", item);
              item = ItemGenerator.generateItem(aa, am, World.world.mapStats.getCurrentYear(), __instance, __instance.king.data.name, 1, a);
              item.modifiers.Clear();
              a.equipment.getSlot(aa.equipmentType).CallMethod("setItem", item);
            }
          }
        }
      }
      if (__instance.king != null && !__instance.king.data.alive)
      {
        var deadKingGroup = (UnitGroup)Reflection.GetField(typeof(Actor), __instance.king, "unit_group");
        if (deadKingGroup != null)
        {
          deadKingGroup.disband();
        }
      }
    }
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Actor), "setProfession")]
    public static void SetKingGroup_Postfix(Actor __instance, UnitProfession pType)
    {
      if (pType == UnitProfession.King)
      {
        if (__instance.kingdom != null && __instance.city != null)
        {
          var actorGroup = (UnitGroup)Reflection.GetField(typeof(Actor), __instance, "unit_group");
          if (actorGroup == null)
          {
            actorGroup = MapBox.instance.unitGroupManager.createNewGroup(__instance.city);
            actorGroup.setGroupLeader(__instance);
            actorGroup.addUnit(__instance);
            //Reflection.SetField(__instance.GetType(), __instance, "unitGroup", actorGroup);
          }
        }
      }
    }
    [HarmonyPrefix]
    [HarmonyPatch(typeof(KingdomBehCheckKing), "findKing")]
    public static bool CheckRoyal_Prefix(KingdomBehCheckKing __instance, Kingdom pKingdom)
    {
      bool tianmingBoolValue;
      pKingdom.data.get("tianmingbool", out tianmingBoolValue);
      if (tianmingBoolValue)
      {
        if (pKingdom.data.royal_clan_id == null)
        {

          return true;
        }

        Clan clan = BehaviourActionBase<Kingdom>.world.clans.get(pKingdom.data.royal_clan_id);
        __instance._units.Clear();
        Actor actor = null;
        if (clan != null)
        {
          foreach (Actor a in clan.units.Values)
          {
            if (a != null && a.isAlive())
            {
              //__instance._units.Add(actor);
              actor = a;
            }
          }
          //actor = __instance.getKingFromRoyalClan(pKingdom);
        }
        else
        {
          return true;
        }
        if (actor == null)
        {
          return false;
        }
        if (actor.city != null && actor.city.leader == actor)
        {
          actor.city.removeLeader();
        }
        if (pKingdom.capital != null && actor.city != pKingdom.capital)
        {
          if (actor.city != null)
          {
            actor.city.removeCitizen(actor, false, AttackType.Other);
          }
          pKingdom.capital.addNewUnit(actor, true);
        }
        pKingdom.setKing(actor);
        WorldLog.logNewKing(pKingdom);

        return false;


      }


      return true;
    }
    [HarmonyPrefix]
    [HarmonyPatch(typeof(City), "tryToMakeWarrior")]
    public static bool Checkveteran_Prefix(City __instance, Actor pActor)
    {
      if (pActor.hasTrait("veteran"))
      {
        return false;
      }

      return true;
    }
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Actor), "stopBeingWarrior")]
    public static void SetTianmingicon_Postfix(Actor __instance)
    {
      __instance.addTrait("veteran");
      if (__instance.hasTrait("禁卫军"))
      {
        __instance.removeTrait("禁卫军");
      }
    }
    [HarmonyPostfix]
    [HarmonyPatch(typeof(City), "updateAge")]
    public static void CheckKVeteran_Postfix(City __instance)
    {
      List<Actor> simpleList1 = __instance.units.getSimpleList();
      foreach (Actor actor in simpleList1)
      {
        if (actor != null && actor.data.profession == UnitProfession.Warrior)
        {
          if (actor.data.getAge() >= ((int)(actor.stats[S.max_age] * 0.65)))
          {
            actor.stopBeingWarrior();
          }


        }
      }

    }
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Kingdom), "update")]
    public static void Kinggoing_Postfix(float pElapsed, Kingdom __instance)
    {
      List<War> activeWars = __instance.getWars().Where(war => war.isActive()).ToList();
      //List<War> activeWars = __instance.getWars();
      if (activeWars.Count > 0)
      {
        activeWars.Sort((war1, war2) => war2.data.dead.CompareTo(war1.data.dead));
        War mostDeadlyWar = activeWars[0];

        City targetCity = null;
        if (mostDeadlyWar._asset == AssetManager.war_types_library.get("spite"))
        {
          return;
        }
        if (mostDeadlyWar._asset == AssetManager.war_types_library.get("reclaim"))
        {
          if (FunctionHelper.storedDefenderCities.ContainsKey(mostDeadlyWar))
          {
            foreach (var city in FunctionHelper.storedDefenderCities[mostDeadlyWar])
            {
              if (__instance.king != null && (__instance.king.currentTile.zone == null || (__instance.king.currentTile.zone.city != city)))
              {
                targetCity = city;
                break;
              }
            }
          }
        }

        foreach (Kingdom attacker in mostDeadlyWar.getAttackers())
        {
          if (attacker.king != null && mostDeadlyWar.main_defender.king != null && attacker.king.getAge() >= 18)
          { //Debug.Log("这里确实执行了"+mostDeadlyWar.data.name);
            var actorGroup = (UnitGroup)Reflection.GetField(typeof(Actor), attacker.king, "unit_group");
            var defenderGroup = (UnitGroup)Reflection.GetField(typeof(Actor), mostDeadlyWar.main_defender.king, "unit_group");
            if (actorGroup != null && defenderGroup != null && actorGroup.countUnits() >= 5 && actorGroup.countUnits() >= (defenderGroup.countUnits() - 5))
            { //Debug.Log("进攻首都战争"+mostDeadlyWar.data.name);
              if (mostDeadlyWar.main_defender.capital != null && attacker.king.currentTile.zone != null && (attacker.king.currentTile.zone.city != (targetCity != null ? targetCity : mostDeadlyWar.main_defender.capital)))
              {
                City chosenCity = targetCity != null ? targetCity : mostDeadlyWar.main_defender.capital;

                if (chosenCity == null)
                {
                  Debug.LogError("chosenCity is null");
                }
                else if (chosenCity.getTile() == null)
                {
                

                  //Debug.LogWarning(chosenCity.data.name+chosenCity.kingdom.name+"chosenCity.getTile() is null");
                  attacker.king.goTo(attacker.capital.getTile());
                }
                else if (attacker.king == null)
                {
                  Debug.LogError("attacker.king is null");
                }
                else
                {
                  attacker.king.goTo(chosenCity.getTile());
                }

              }
            }
            else if (actorGroup != null && defenderGroup != null && actorGroup.countUnits() < (defenderGroup.countUnits() - 5))
            {
              if (attacker.capital != null && attacker.king.currentTile != null && attacker.king.currentTile.zone != null && attacker.king.currentTile.zone.city != attacker.capital)
              { //Debug.Log("回去"+mostDeadlyWar.data.name);
                attacker.king.goTo(attacker.capital.getTile());
              }
            }
          }
        }
      }
    }
  }
}