using ai.behaviours;

namespace Figurebox.ai.behaviours.actor;

public class BehVerifierCanCatchSlave : BehaviourActionActor
{
    public override BehResult execute(Actor pObject)
    {
        if (pObject.currentTile.zone.city?.kingdom?.isEnemy(pObject.kingdom) ?? false) return BehResult.Continue;

        return BehResult.Continue;
    }
}