using Figurebox.constants;
using Figurebox.utils.extensions;

namespace Figurebox.ai.conditions;

public class CondHasSlaveCaught : BehaviourActorCondition
{
    public override bool check(Actor pActor)
    {
        var caughtSlaves = pActor.data.ReadObj<string[]>(AWDataS.caught_slaves);
        if (caughtSlaves == null) return false;
        return caughtSlaves.Length > 0;
    }
}