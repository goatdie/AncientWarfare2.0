using Figurebox.constants;
using Figurebox.Utils.extensions;

namespace Figurebox.ai.conditions;

public class CondIsKing : BehaviourActorCondition
{
    public override bool check(Actor pActor)
    {
        return pActor.isKing();
    }
}