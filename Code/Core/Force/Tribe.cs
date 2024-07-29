using System.Collections.Generic;
using AncientWarfare.Core.Extensions;
using AncientWarfare.Core.Quest;
using AncientWarfare.Core.Quest.QuestSettingParams;
using AncientWarfare.NameGenerators;
using AncientWarfare.Utils;
using Chinese_Name;
using NeoModLoader.api.attributes;

namespace AncientWarfare.Core.Force
{
    public class Tribe : BaseForce<TribeData>, IHasMember
    {
        private ColorAsset      _color;
        public  List<TileZone>  border_zones = new();
        public  List<QuestInst> quests       = new();
        public  List<TileZone>  zones        = new();

        public Tribe(TribeData data) : base(data)
        {
            InitializeQuests();
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

        [Hotfixable]
        public void AddMemberOneside(Actor actor)
        {
            Data.members.Add(actor.data.id);
        }

        [Hotfixable]
        public void RemoveMemberOneside(string actor_id)
        {
            Data.members.Remove(actor_id);
        }

        private void InitializeQuests()
        {
            quests.Expand(
                new QuestInst(QuestLibrary.food_base_collect, this, new Dictionary<string, object>
                {
                    { TypedResourceCollectSettingKeys.resource_type, ResType.Food },
                    { TypedResourceCollectSettingKeys.resource_count, 10 }
                })
            );
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
        }

        public void RemoveZone(TileZone zone)
        {
            if (zones.Remove(zone))
            {
                zone.SetTribe(null);
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

        internal void FreshQuests()
        {
            foreach (QuestInst quest in quests) quest.UpdateProgress();
        }

        public override void Update()
        {
            FreshQuests();
        }
    }
}