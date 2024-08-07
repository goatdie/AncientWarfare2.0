using System;
using System.Collections.Generic;
using System.Linq;
using AncientWarfare.Const;
using AncientWarfare.Core.Force;
using AncientWarfare.Utils;
using NeoModLoader.api.attributes;

namespace AncientWarfare.Core.Extensions
{
    public static partial class ActorExtension
    {
        private static readonly List<Building> temp_buildings_for_FindAvailableResourceBuildingInTribe = new();
        private static readonly List<Building> temp_buildings_for_FindBuildingTarget               = new();
        private static readonly List<TileZone> temp_zones_for_FindAvailableResourceBuildingInTribe = new();

        public static bool IsValid(this Actor actor)
        {
            return actor != null && actor.isAlive();
        }

        public static ActorAdditionData GetAdditionData(this Actor actor, bool null_if_not_exists = false)
        {
            return null_if_not_exists
                ? ActorAdditionDataManager.TryGet(actor.data.id)
                : ActorAdditionDataManager.Get(actor.data.id);
        }

        [Hotfixable]
        public static void GiveChildBirth(this Actor actor)
        {
            actor.data.get(ActorDataKeys.aw_pregnant_child_data_string, out string raw_child_data);
            if (string.IsNullOrEmpty(raw_child_data)) return;

            actor.data.makeChild(World.world.getCurWorldTime());
            actor.data.removeString(ActorDataKeys.aw_pregnant_child_data_string);

            var child_data = GeneralHelper.FromJSON<ActorData>(raw_child_data);
            // 补足孩子的数据
            var names = actor.data.name.Split(ComnConst.COMN_SEPERATOR);

            child_data.name = "";
            child_data.id = World.world.mapStats.getNextId("unit");
            child_data.x = actor.currentTile.pos.x;
            child_data.y = actor.currentTile.pos.y;
            child_data.created_time = World.world.getCreationTime();
            ActorAsset asset = AssetManager.actor_library.get(child_data.asset_id);
            ActorBase.generateCivUnit(asset, child_data, AssetManager.raceLibrary.get(asset.race));
            child_data.health = (int)asset.base_stats[S.health];
            child_data.hunger = asset.maxHunger / 2;

            var family_name = names[0];
            var clan_name = names.Length == 2 ? names[1] : string.Empty;
            ActorAdditionData addition_data = ActorAdditionDataManager.Get(child_data.id);
            addition_data.family_name = family_name;
            addition_data.clan_name = clan_name;


            var baby_asset_id = asset.id.Replace("unit_", "baby_");
            child_data.profession = UnitProfession.Baby;
            if (!AssetManager.actor_library.Contains(baby_asset_id))
            {
                baby_asset_id = "baby_" + asset.id;
                if (!AssetManager.actor_library.Contains(baby_asset_id))
                {
                    baby_asset_id = asset.id;
                    child_data.profession = UnitProfession.Unit;
                }
            }

            ActorAsset baby_asset = AssetManager.actor_library.get(baby_asset_id);
            if (child_data.profession == UnitProfession.Baby)
                foreach (var it in baby_asset.traits.Where(it => !child_data.traits.Contains(it)))
                    child_data.traits.Add(it);

            Tribe tribe = actor.GetTribe();

            Actor child = World.world.units.loadObject(child_data);

            if (child == null || tribe == null) return;
            ForceManager.MakeJoinToForce(child, tribe);
        }

        public static void SubmitInventoryResourcesToTribe(this Actor actor)
        {
            if (!actor.inventory.hasResources()) return;
            var tribe = actor.GetTribe();
            if (tribe == null) return;
            if (tribe.Data.storage.IsFull())
            {
                tribe.NewExpandStorageQuest();
            }

            foreach (var it in actor.inventory.getResources())
            {
                tribe.Data.storage.Store(it.Value.id, it.Value.amount);
            }

            actor.inventory.empty();
        }

        public static Building FindBuildingTarget(this Actor actor, string type)
        {
            Tribe tribe = actor.GetTribe();
            if (tribe == null) return null;

            var possible_targets = temp_buildings_for_FindBuildingTarget;
            possible_targets.Clear();
            if (type == BuildTargetType.new_building)
            {
                var list = tribe.buildings.getSimpleList();
                possible_targets.AddRange(list.FindAll(x => x.isUnderConstruction() &&
                                                            (x.getConstructionTile()?.isSameIsland(actor.currentTile) ??
                                                             false)));
            }
            else
            {
                return actor.FindAvailableResourceBuildingInTribe(type);
            }

            if (possible_targets.Count == 0) return null;
            Building res = possible_targets.GetRandom();
            possible_targets.Clear();
            return res;
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
            foreach (TileZone zone in search_zones)
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
                    foreach (Building it in search_set)
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
            if (food.id == actor.data.favoriteFood)
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

        public static Building GetNearestBuildingIn(this Actor           actor, IEnumerable<Building> buildings,
                                                    Func<Building, bool> check = null)
        {
            Building nearest_b = null;
            var min_dist = float.MaxValue;
            foreach (Building b in buildings)
            {
                if (!check?.Invoke(b) ?? false) continue;

                var dist = Toolbox.DistVec2(b.currentTile.pos, actor.currentTile.pos);
                if (dist < min_dist)
                {
                    min_dist = dist;
                    nearest_b = b;
                }
            }

            return nearest_b;
        }

        public static float GetPossibilityToFindJobItSelf(this Actor actor)
        {
            throw new NotImplementedException();
        }

        public static string FindJobItSelf(this Actor actor)
        {
            throw new NotImplementedException();
        }
    }
}