using ai.behaviours;
using AncientWarfare.Core.Extensions;
using AncientWarfare.Utils;

namespace AncientWarfare.Core.AI.ActorBehs
{
    public class BehFindTileForTribe : BehaviourActionActor
    {
        public override BehResult execute(Actor pObject)
        {
            if (!TribePlaceFinder.HasPossibleZones()) return BehResult.Stop;

            TileZone target_tile_zone = null;
            float min_dist = float.MaxValue;
            TribePlaceFinder.I.zones.ShuffleOne();
            foreach (TileZone zone in TribePlaceFinder.I.zones)
            {
                var zone_data = zone.GetAdditionData();
                if (!zone_data.good_for_new_tribe) continue;
                if (!zone.tiles[0].isSameIsland(pObject.currentTile)) continue;
                float dist = Toolbox.DistTile(zone.centerTile, pObject.currentTile);
                if (dist < min_dist || target_tile_zone == null)
                {
                    min_dist = dist;
                    target_tile_zone = zone;
                }
            }

            if (target_tile_zone == null)
            {
                var tile_island = World.world.islandsCalculator.getRandomIslandGround(true);
                if (tile_island == null) return BehResult.Stop;
                pObject.beh_tile_target = tile_island.getRandomTile();
            }
            else
            {
                pObject.beh_tile_target = target_tile_zone.tiles.GetRandom();
            }

            return BehResult.Continue;
        }
    }
}