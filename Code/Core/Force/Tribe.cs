using System;
using System.Collections.Generic;
using System.Linq;
using AncientWarfare.Core.Extensions;
using AncientWarfare.Core.Quest;
using AncientWarfare.Core.Quest.QuestSettingParams;
using AncientWarfare.NameGenerators;
using Chinese_Name;
using NeoModLoader.api.attributes;
using Manager = AncientWarfare.Core.MapModes.Manager;

namespace AncientWarfare.Core.Force
{
    public class Tribe : BaseForce<TribeData>, IHasMember, IHasBuilding
    {
        public readonly  List<TileZone>                border_zones       = new();
        public readonly  BuildingContainer             buildings          = new();
        public readonly  List<QuestInst>               quests             = new();
        private readonly Dictionary<string, QuestInst> quests_dict        = new();
        public readonly  List<TileZone>                zones              = new();
        private          bool                          _buildings_updated = true;
        private          Race                          _cache_race;
        private          ColorAsset                    _color;
        private          QuestInst                     _finish_constructing_building_quest;
        private          QuestInst                     _food_base_quest;
        private          bool                          _zones_updated = true;

        public Tribe(TribeData data) : base(data)
        {
            InitializeQuests();
            data.storage.SetSize(0);
        }

        public Race Race
        {
            get
            {
                if (_cache_race == null)
                    if (!string.IsNullOrEmpty(Data.race_id))
                        _cache_race = AssetManager.raceLibrary.get(Data.race_id);

                return _cache_race;
            }
        }

        public ColorAsset Color
        {
            get
            {
                if (_color == null)
                {
                    if (Data.color_id == -1) Data.color_id = AssetManager.kingdom_colors_library.getNextColorIndex();
                    _color = AssetManager.kingdom_colors_library.getColorByIndex(Data.color_id);
                }

                return _color;
            }
        }

        public WorldTile CenterTile => zones[0].centerTile;

        public void RemoveBuildingOneside(Building building)
        {
            buildings.Remove(building);
            SetBuildingUpdated();
        }

        public void AddBuildingOneside(Building building)
        {
            buildings.Add(building);
            SetBuildingUpdated();
        }

        [Hotfixable]
        public void AddMemberOneside(Actor actor)
        {
            if (string.IsNullOrEmpty(Data.race_id)) Data.race_id = actor.race.id;

            Data.members.Add(actor.data.id);
        }

        [Hotfixable]
        public void RemoveMemberOneside(string actor_id)
        {
            Data.members.Remove(actor_id);
        }

        private void InitializeQuests()
        {
            _food_base_quest = new QuestInst(QuestLibrary.food_base_collect, this, new Dictionary<string, object>
            {
                { TypedResourceCollectSettingKeys.resource_count_int, 10 }
            });
            _finish_constructing_building_quest =
                new QuestInst(QuestLibrary.finish_constructing_building, this, new Dictionary<string, object>());
            EnqueueQuests(
                _food_base_quest,
                _finish_constructing_building_quest
            );
        }

        public void SignalQuestFinish(string quest_uid)
        {
            if (quests_dict.TryGetValue(quest_uid, out QuestInst q)) q.MarkFinished();
        }

        public void SignalQuestRestart(string quest_uid)
        {
            if (quests_dict.TryGetValue(quest_uid, out QuestInst q)) q.MarkRestart();
        }

        public void EnqueueQuest(QuestInst quest)
        {
            if (quest.asset.merge_action_when_repeat != null)
                foreach (QuestInst repeat_q in quests)
                    if (repeat_q.asset == quest.asset && repeat_q.asset.merge_action_when_repeat(repeat_q, quest))
                        return;

            quests_dict[quest.UID] = quest;
            quests.Add(quest);
        }

        public void EnqueueQuests(params QuestInst[] list)
        {
            foreach (QuestInst q in list) EnqueueQuest(q);
        }

        public void AddZone(TileZone zone)
        {
            if (zones.Contains(zone)) return;

            var zone_tribe = zone.GetTribe();
            if (zone_tribe != null)
            {
                zone_tribe.RemoveZone(zone);
            }

            zones.Add(zone);
            zone.SetTribe(this);

            _zones_updated = true;
            Manager.SetAllDirty();
        }

        public void RemoveZone(TileZone zone)
        {
            if (zones.Remove(zone))
            {
                zone.SetTribe(null);
                _zones_updated = true;
                Manager.SetAllDirty();
            }
        }

        public bool IsFull()
        {
            return Data.members.Count >= 10;
        }

        public override string NewName()
        {
            var name_generator = CN_NameGeneratorLibrary.Get(TribeNameGenerator.ID);
            var param = new Dictionary<string, string>();
            ParameterGetters.GetCustomParameterGetter<TribeNameGenerator.TribeParameterGetter>(
                name_generator.parameter_getter)(this, param);
            return name_generator.GenerateName(param);
        }

        private void FreshQuests(float elapsed)
        {
            _food_base_quest.Data.set(TypedResourceCollectSettingKeys.resource_count_int,
                                      Math.Max(10, Data.members.Count));

            foreach (QuestInst quest in quests) quest.UpdateProgress(elapsed);

            var finished_quests = quests.FindAll(q => q.Finished);
            foreach (QuestInst q in finished_quests) quests_dict.Remove(q.UID);

            quests.RemoveAll(q => q.Finished);
        }

        public override void Update(float elapsed)
        {
            buildings.checkAddRemove();

            CheckZones();
            CheckStorages();

            FreshQuests(elapsed);

            ClearDirty();
        }

        public void SetBuildingUpdated()
        {
            _buildings_updated = true;
        }

        private void ClearDirty()
        {
            _buildings_updated = false;
            _zones_updated = false;
        }

        private void CheckStorages()
        {
            if (!_buildings_updated) return;
            var building_list = buildings.getSimpleList();
            var storage_size = 0;
            foreach (Building b in building_list)
            {
                if (!b.asset.storage) continue;

                storage_size += b.asset.GetAdditionAsset().storage_size;
            }

            Data.storage.SetSize(storage_size);
        }

        public void NewExpandQuest(string target_resource_type)
        {
            var quest = new QuestInst(QuestLibrary.expand_tribe_for_resource, this, new Dictionary<string, object>
            {
                { TribeExpandForResourceSettingKeys.resource_type_string, target_resource_type }
            });
            EnqueueQuest(quest);
            // throw new NotImplementedException();
        }

        private void CheckZones()
        {
            if (!_zones_updated) return;
            border_zones.Clear();
            border_zones.AddRange(zones.Where(zone => zone.neighboursAll.Exists(x => x.GetTribe() != this)));
        }

        public void NewResourceQuest(string resource_id, int target_count)
        {
            QuestAsset asset = null;
            if (resource_id == SR.wood) asset = QuestLibrary.chop_wood;

            if (asset == null)
            {
                Main.LogDebug($"Not found quest asset for {resource_id}");
                return;
            }

            QuestInst quest = new(asset, this,
                                  new Dictionary<string, object>
                                  {
                                      { ResourceCollectSettingKeys.resource_target_count_int, target_count }
                                  });
            EnqueueQuest(quest);
        }

        public void NewResourceQuestsFromCost(ConstructionCost cost)
        {
            if (cost.gold > Data.storage.GetCount(SR.gold)) NewResourceQuest(SR.gold, cost.gold);

            if (cost.wood > Data.storage.GetCount(SR.wood)) NewResourceQuest(SR.wood, cost.wood);

            if (cost.stone > Data.storage.GetCount(SR.stone)) NewResourceQuest(SR.stone, cost.stone);

            if (cost.common_metals > Data.storage.GetCount(SR.common_metals))
                NewResourceQuest(SR.common_metals, cost.common_metals);
        }

        public void NewExpandStorageQuest()
        {
            var quest = new QuestInst(QuestLibrary.build_or_upgrade_storage_building, this,
                                      new Dictionary<string, object>
                                      {
                                          { ConstructBuildingSettingKeys.building_key_string, SB.order_hall_0 },
                                          { ConstructBuildingSettingKeys.upgrade_building_if_possible_bool, true }
                                      });
            EnqueueQuest(quest);
        }

        public void RestartQuest(string quest_asset_id)
        {
            foreach (QuestInst q in quests)
                if (q.asset.id == quest_asset_id)
                {
                    q.MarkRestart();
                    return;
                }
        }

        public void SignalBuildingQuestFinished(string quest_id, Building building)
        {
            SignalQuestFinish(quest_id);
            // 之后可以根据building发奖励
        }

        public void NewExpandHousingQuest()
        {
            var quest = new QuestInst(QuestLibrary.build_or_upgrade_housing_building, this,
                                      new Dictionary<string, object>
                                      {
                                          { ConstructBuildingSettingKeys.building_key_string, SB.order_house_0 },
                                          { ConstructBuildingSettingKeys.upgrade_building_if_possible_bool, true }
                                      });
            EnqueueQuest(quest);
        }

        /// <summary>
        ///     是否允许<paramref name="actor" />脱产进行科技解锁
        /// </summary>
        /// <param name="actor"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool AllowUnlockTechWithoutProduction(Actor actor)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     是否拥有科技<paramref name="tech" />
        /// </summary>
        /// <param name="tech"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool HasTech(string tech)
        {
            throw new NotImplementedException();
        }

        public bool AllowFindJobItSelf(Actor actor)
        {
            throw new NotImplementedException();
        }
    }
}