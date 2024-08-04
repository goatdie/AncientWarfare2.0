using ai.behaviours;
using AncientWarfare.Core.Extensions;
using AncientWarfare.Core.Force;

namespace AncientWarfare.Core.AI.ActorBehs;

public class BehTribeExpandZone : BehTribe
{
    public override BehResult execute(Actor pObject)
    {
        TileZone zone = pObject.currentTile.zone;
        Tribe zone_tribe = zone.GetTribe();
        Tribe tribe = pObject.GetTribe();
        if (zone_tribe != null && zone_tribe != tribe)
            // TODO: 抢夺
            return BehResult.Continue;
        if (zone.neighbours.Exists(z => z.GetTribe() == tribe))
            tribe.AddZone(zone);

        return BehResult.Continue;
    }
}