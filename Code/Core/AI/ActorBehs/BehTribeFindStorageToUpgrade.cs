using ai.behaviours;
using AncientWarfare.Core.Extensions;
using AncientWarfare.Core.Force;

namespace AncientWarfare.Core.AI.ActorBehs;

public class BehTribeFindStorageToUpgrade : BehTribe
{
    public override BehResult execute(Actor pObject)
    {
        Tribe tribe = pObject.GetTribe();
        foreach (Building building in tribe.buildings)
        {
            if (!building.asset.storage) continue;
            if (!building.asset.canBeUpgraded) continue;
            if (!tribe.Data.storage.HasResourceForUpgrading(building.asset)) continue;

            if (!building.CanUpgradeForSurrounding()) continue;

            pObject.beh_building_target = building;
            return BehResult.Stop;
        }

        return BehResult.Continue;
    }
}