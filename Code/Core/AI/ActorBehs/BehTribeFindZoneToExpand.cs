using ai.behaviours;
using AncientWarfare.Core.Extensions;
using AncientWarfare.Core.Force;

namespace AncientWarfare.Core.AI.ActorBehs;

public class BehTribeFindZoneToExpand : BehTribe
{
    public override BehResult execute(Actor pObject)
    {
        Tribe tribe = pObject.GetTribe();
        foreach (TileZone zone in tribe.border_zones)
        foreach (TileZone neighour in zone.neighbours)
        {
            if (neighour.GetTribe() == tribe) continue;
            if (!neighour.centerTile.region.Equals(pObject.currentTile.region)) continue;
            pObject.beh_tile_target = neighour.centerTile;
            return BehResult.Continue;
        }

        return BehResult.Stop;
    }
}