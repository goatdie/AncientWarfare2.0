using System.Collections.Generic;
using ai.behaviours;
using AncientWarfare.Core.AI.Abstract;
using AncientWarfare.Core.Profession;

namespace AncientWarfare.Core.AI.ActorBehs;

public class BehAttackActorTargetExtended : BehActionActorExtended
{
    public BehAttackActorTargetExtended() : base(BehResult.Continue)
    {
    }

    public override Dictionary<string, int> ExpGiven { get; } = new()
    {
        { nameof(NewProfessionLibrary.battle), 1 }
    };

    public override List<string> TechRequired { get; }

    public override void create()
    {
        base.create();
        null_check_actor_target = true;
    }

    public override (BehResult, bool) OnExecute(Actor actor)
    {
        var attack_or_not = false;
        if (actor.isInAttackRange(actor.beh_actor_target))
        {
            actor.tryToAttack(actor.beh_actor_target);
            attack_or_not = true;
        }

        return (actor.beh_actor_target.isAlive() ? BehResult.RestartTask : BehResult.Continue, attack_or_not);
    }
}