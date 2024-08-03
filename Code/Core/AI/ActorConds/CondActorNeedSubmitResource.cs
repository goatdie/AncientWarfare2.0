namespace AncientWarfare.Core.AI.ActorConds;

public class CondActorNeedSubmitResource : BehaviourActorCondition
{
    public override bool check(Actor pActor)
    {
        return pActor.inventory.hasResources();
    }
}