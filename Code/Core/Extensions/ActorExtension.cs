using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AncientWarfare.Core.Extensions
{
    public static partial class ActorExtension
    {
        private static readonly List<Building> temp_buildings_for_FindAvailableResourceBuildingInTribe = new();
        private static readonly List<TileZone> temp_zones_for_FindAvailableResourceBuildingInTribe = new();
        public static bool IsValid(this Actor actor)
        {
            return actor != null && actor.isAlive();
        }
        public static ActorAdditionData GetAdditionData(this Actor actor)
        {
            return ActorAdditionDataManager.Get(actor.data.id);
        }
        public static void SubmitInventoryResourcesToTribe(this Actor actor)
        {
            if (!actor.inventory.hasResources()) return;
            var tribe = actor.GetTribe();
            if (tribe == null) return;
            foreach(var it in actor.inventory.getResources())
            {
                tribe.Data.storage.Store(it.Value.id, it.Value.amount);
            }
            actor.inventory.empty();
        }
        public static Building FindAvailableResourceBuildingInTribe(this Actor actor, string type)
        {
            if (string.IsNullOrEmpty(type)) return null;

            var possible_targets = temp_buildings_for_FindAvailableResourceBuildingInTribe;
            var search_zones = temp_zones_for_FindAvailableResourceBuildingInTribe;
            // 没必要但是为了保险
            possible_targets.Clear();
            search_zones.Clear();

            var tribe = actor.GetTribe();
            if (tribe == null)
            {
                return null;
            }
            search_zones.AddRange(tribe.zones);
            search_zones.Add(actor.currentTile.zone);
            search_zones.AddRange(actor.currentTile.zone.neighboursAll);
            search_zones.ShuffleOne();
            foreach(var zone in search_zones)
            {
                HashSet<Building> search_set = null;
                if (type == SB.type_flower || type == SB.type_vegetation)
                {
                    search_set = zone.plants;
                }
                else if (type == SB.type_tree)
                {
                    search_set = zone.trees;
                }
                else if (type == SB.type_mineral)
                {
                    search_set = zone.minerals;
                }
                else if (type == SB.type_fruits)
                {
                    search_set = zone.food;
                }
                if (search_set?.Count > 0)
                {
                    foreach(var it in search_set)
                    {
                        if (it.currentTile.targetedBy != null) continue;
                        if (!it.currentTile.isSameIsland(actor.currentTile)) continue;
                        if (!it.hasResources) continue;

                        possible_targets.Add(it);
                    }
                    if (possible_targets.Count > 0)
                    {
                        break;
                    }
                }
            }


            var res = possible_targets.Count > 0 ? possible_targets.GetRandom() : null;
            possible_targets.Clear();
            search_zones.Clear();
            return res;
        }
        public static void ConsumeFood(this Actor actor, ResourceAsset food)
        {
            if (!actor.IsValid() || food == null)
            {
                return;
            }
            int hunger_restore = food.restoreHunger;
            float health_restore = food.restoreHealth;
            if(food.id == actor.data.favoriteFood)
            {
                hunger_restore += hunger_restore / 2;
                health_restore *= 1.5f;
            }
            actor.restoreStatsFromEating(hunger_restore, health_restore);
            if (Toolbox.randomChance(food.give_chance))
            {
                if (food.give_trait != null && food.give_trait.Count > 0 && Toolbox.randomBool())
                {
                    string trait = food.give_trait.GetRandom();
                    if (trait != null)
                    {
                        actor.addTrait(trait, false);
                    }
                }
                if (food.give_status != null && food.give_status.Count > 0 && Toolbox.randomBool())
                {
                    string status = food.give_status.GetRandom();
                    if (status != null)
                    {
                        actor.addStatusEffect(status, -1f);
                    }
                }
                if (food.give_action != null && Toolbox.randomBool())
                {
                    food.give_action(food);
                }
            }



            actor.timer_action = 1.5f;
            actor.ate_last_item_id = food.id;
            actor.ate_last_time = World.world.getCurSessionTime();


            if (string.IsNullOrEmpty(actor.data.favoriteFood)) return;

            if (food.id == actor.data.favoriteFood && actor.data.mood != "happy")
            {
                actor.changeMood("happy");
                return;
            }
            if (actor.data.mood == "happy")
            {
                actor.changeMood("normal");
                return;
            }
            if (actor.data.mood == "normal")
            {
                if (Toolbox.randomChance(0.2f))
                {
                    actor.changeMood("sad");
                    return;
                }
            }
            else if (actor.data.mood == "sad" && Toolbox.randomChance(0.05f))
            {
                actor.changeMood("angry");
            }
        }
    }
}
