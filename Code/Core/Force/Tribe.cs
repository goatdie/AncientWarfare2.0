using AncientWarfare.Core.Extensions;
using AncientWarfare.NameGenerators;
using Chinese_Name;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AncientWarfare.Core.Force
{
    public class Tribe : BaseForce<TribeData>
    {
        public List<TileZone> zones = new();
        private ColorAsset _color;
        public ColorAsset Color { get
            {
                if (_color == null)
                {
                    if (Data.color_id == -1) Data.color_id = AssetManager.kingdom_colors_library.getNextColorIndex();
                    _color = AssetManager.kingdom_colors_library.getColorByIndex(Data.color_id);
                }
                return _color;
            } }
        public Tribe(TribeData data) : base(data)
        {

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
        public WorldTile CenterTile => zones[0].centerTile;
        public bool IsFull()
        {
            return Data.members.Count >= 10;
        }

        public override string NewName()
        {
            var name_generator = CN_NameGeneratorLibrary.Get(TribeNameGenerator.ID);
            var param = new Dictionary<string, string>();
            ParameterGetters.GetCustomParameterGetter<TribeNameGenerator.TribeParameterGetter>(name_generator.parameter_getter)(this, param);
            return name_generator.GenerateName(param);
        }
        public void AddNewActor(Actor actor)
        {
            Data.members.Add(actor.data.id);
        }
    }
}
