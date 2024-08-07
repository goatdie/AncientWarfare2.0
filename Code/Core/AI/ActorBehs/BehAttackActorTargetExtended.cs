using ai.behaviours;
using AncientWarfare.Core.Extensions;
using AncientWarfare.Core.Profession;

namespace AncientWarfare.Core.AI.ActorBehs;

public class BehAttackActorTargetExtended : BehaviourActionActor
{
    public override void create()
    {
        base.create();
        null_check_actor_target = true;
    }

    public override BehResult execute(Actor pActor)
    {
        if (pActor.isInAttackRange(pActor.beh_actor_target)) pActor.tryToAttack(pActor.beh_actor_target);

        if (pActor.beh_actor_target.isAlive()) return BehResult.RestartTask;

        pActor.IncreaseProfessionExp(nameof(NewProfessionLibrary.battle));
        return BehResult.Continue;
    }
}