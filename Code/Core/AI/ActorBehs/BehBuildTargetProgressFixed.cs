using System.Collections.Generic;
using ai.behaviours;
using AncientWarfare.Core.AI.Abstract;
using AncientWarfare.Core.Profession;

namespace AncientWarfare.Core.AI.ActorBehs;

public class BehBuildTargetProgressFixed : BehActionActorExtended
{
    public BehBuildTargetProgressFixed() : base(BehResult.Continue)
    {
    }

    public override Dictionary<string, int> ExpGiven { get; } = new()
    {
        { nameof(NewProfessionLibrary.build), 1 }
    };

    public override List<string> TechRequired { get; }

    public override void create()
    {
        base.create();
        check_building_target_non_usable = true;
        null_check_tile_target = true;
    }

    public override (BehResult, bool) OnExecute(Actor actor)
    {
        if (!actor.beh_building_target.isUnderConstruction())
            return (BehResult.Stop, false);
        actor.beh_building_target.updateBuild();
        return (BehResult.Continue, true);
    }
}