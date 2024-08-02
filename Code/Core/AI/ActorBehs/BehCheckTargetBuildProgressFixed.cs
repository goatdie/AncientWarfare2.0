using ai.behaviours;

namespace AncientWarfare.Core.AI.ActorBehs;

public class BehCheckTargetBuildProgressFixed : BehaviourActionActor
{
    public override void create()
    {
        base.create();
        check_building_target_non_usable = true;
        null_check_tile_target = true;
    }

    public override BehResult execute(Actor pActor)
    {
        return pActor.beh_building_target.isUnderConstruction() ? BehResult.RestartTask : BehResult.Continue;
    }
}