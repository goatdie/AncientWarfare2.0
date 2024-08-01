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
        public readonly List<TileZone>    border_zones       = new();
        public readonly BuildingContainer buildings          = new();
        public readonly List<QuestInst>   quests             = new();
        public readonly List<TileZone>    zones              = new();
        private         bool              _buildings_updated = true;
        private Race _cache_race;
        private         ColorAsset        _color;
        private         bool              _zones_updated = true;

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
            _buildings_updated = true;
        }

        public void AddBuildingOneside(Building building)
        {
            buildings.Add(building);
            _buildings_updated = true;
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
            EnqueueQuests(
                new QuestInst(QuestLibrary.food_base_collect, this, new Dictionary<string, object>
                {
                    { TypedResourceCollectSettingKeys.resource_type_int, ResType.Food },
                    { TypedResourceCollectSettingKeys.resource_count_int, 10 }
                })
            );
        }

        public void EnqueueQuest(QuestInst quest)
        {
            if (quest.asset.merge_action_when_repeat == null)
            {
                quests.Add(quest);
                return;
            }

            QuestInst repeat_q = quests.FirstOrDefault(x => x.asset == quest.asset);
            if (repeat_q != null)
                repeat_q.asset.merge_action_when_repeat(repeat_q, quest);
            else
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

        private void FreshQuests()
        {
            foreach (QuestInst quest in quests) quest.UpdateProgress();

            quests.RemoveAll(quest => quest.Disposable && !quest.Active);
        }

        public override void Update()
        {
            buildings.checkAddRemove();

            CheckZones();
            CheckStorages();

            FreshQuests();

            ClearDirty();
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

        public void NewExpandStorageQuest()
        {
        }
    }
}