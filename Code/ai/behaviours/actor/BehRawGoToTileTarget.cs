using ai.behaviours;

namespace Figurebox.ai.behaviours.actor;

public class BehRawGoToTileTarget : BehaviourActionActor
{
    public bool walkOnBlocks;
    public bool walkOnWater;

    public override void create()
    {
        base.create();
        null_check_tile_target = true;
        walkOnWater = true;
    }

    public override BehResult execute(Actor pActor)
    {
        pActor.goTo(pActor.beh_tile_target, walkOnWater, walkOnBlocks);
        return BehResult.Continue;
    }
}