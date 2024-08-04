using ai.behaviours;
using AncientWarfare.Core.AI.Abstract;
using AncientWarfare.Core.Extensions;
using AncientWarfare.Core.Force;

namespace AncientWarfare.Core.AI.ActorBehs;

public class BehTribeFindStorage : BehTribe
{
    public override BehResult execute(Actor pObject)
    {
        Tribe tribe = pObject.GetTribe();
        var list = tribe.buildings.getSimpleList();

        Building nearest_b = pObject.GetNearestBuildingIn(list, b => b.asset.storage && !b.isUnderConstruction());

        if (nearest_b == null) return BehResult.Stop;

        pObject.beh_building_target = nearest_b;
        return BehResult.Continue;
    }
}