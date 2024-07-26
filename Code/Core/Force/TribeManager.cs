using AncientWarfare.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AncientWarfare.Core.Force
{
    public class TribeManager : SubForceManager<Tribe, TribeData>
    {
        public readonly List<Tribe> All = new();

        public TribeManager(Dictionary<string, LowBaseForce> total_container) : base(total_container)
        {
        }

        public Tribe CreateNewTribe(TileZone zone)
        {
            TribeData data = new TribeData();
            data.id = ForceManager.I.GetNewId();
            //data.name = "Tribe " + All.Count;

            Tribe tribe = new Tribe(data);
            tribe.AddZone(zone);
            foreach(var neighbor in zone.neighboursAll)
            {
                var neighbor_tribe = neighbor.GetTribe();
                if (neighbor_tribe != null) continue;
                tribe.AddZone(neighbor);
            }

            All.Add(tribe);
            total_container.Add(data.id, tribe);
            return tribe;
        }
    }
}
