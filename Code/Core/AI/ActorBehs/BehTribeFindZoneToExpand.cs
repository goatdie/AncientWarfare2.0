using ai;
using ai.behaviours;
using AncientWarfare.Core.AI.Abstract;
using AncientWarfare.Core.Extensions;
using AncientWarfare.Core.Force;
using NeoModLoader.api.attributes;

namespace AncientWarfare.Core.AI.ActorBehs;

public class BehTribeFindZoneToExpand : BehTribe
{
    [Hotfixable]
    public override BehResult execute(Actor pObject)
    {
        Tribe tribe = pObject.GetTribe();
        foreach (TileZone zone in tribe.border_zones)
        foreach (TileZone neighour in zone.neighbours)
        {
            if (neighour.GetTribe()            == tribe) continue;
            if (neighour.centerTile.targetedBy != null) continue;
            ExecuteEvent can_goto = pObject.goTo(neighour.centerTile);
            if (can_goto == ExecuteEvent.False) continue;

            pObject.beh_tile_target = neighour.centerTile;
            return BehResult.Continue;
        }

        return BehResult.Stop;
    }
}