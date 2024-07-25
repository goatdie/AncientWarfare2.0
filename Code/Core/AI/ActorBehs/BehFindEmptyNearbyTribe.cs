using ai.behaviours;
using AncientWarfare.Core.Extensions;
using AncientWarfare.Core.Force;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AncientWarfare.Core.AI.ActorBehs
{
    public class BehFindEmptyNearbyTribe : BehaviourActionActor
    {
        private static readonly List<Tribe> temp_tribes = new();
        public override BehResult execute(Actor pObject)
        {
            var tribe = GetEmptyTribe(pObject.currentTile);
            if (tribe != null)
            {
                pObject.JoinTribe(tribe);
                return BehResult.Continue;
            }
            return BehResult.Stop;
        }
        private static Tribe GetEmptyTribe(WorldTile center)
        {
            foreach (var tribe in ForceManager.I.tribes.All)
            {
                if (tribe.IsFull()) continue;
                if (Toolbox.DistTile(center, tribe.CenterTile) > 200) continue;
                if (!tribe.CenterTile.isSameIsland(center)) continue;
                temp_tribes.Add(tribe);
            }


            if (temp_tribes.Count == 0) return null;

            var res = temp_tribes.GetRandom();
            temp_tribes.Clear();
            return res;
        }
    }
}
