using System.Collections.Generic;
using ai.behaviours;
using AncientWarfare.Core.AI.Abstract;
using AncientWarfare.Core.Profession;

namespace AncientWarfare.Core.AI.ActorBehs;

public class BehBuildTargetProgressFixed : BehActionActorProfWrapped
{
    public override HashSet<string> ProfessionRequired { get; } = null;

    public override Dictionary<string, int> ProfessionExpGivenPerExecute { get; } = new()
    {
        { nameof(NewProfessionLibrary.build), 1 }
    };

    public override void create()
    {
        base.create();
        check_building_target_non_usable = true;
        null_check_tile_target = true;
    }

    public override BehResult execute(Actor pObject)
    {
        if (!pObject.beh_building_target.isUnderConstruction())
            return BehResult.Stop;
        pObject.beh_building_target.updateBuild();
        return BehResult.Continue;
    }
}