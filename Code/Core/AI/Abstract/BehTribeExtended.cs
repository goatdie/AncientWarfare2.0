using ai.behaviours;
using AncientWarfare.Core.Extensions;

namespace AncientWarfare.Core.AI.Abstract;

public abstract class BehTribeExtended : BehActionActorExtended
{
    protected BehTribeExtended(BehResult res_on_tech_not_satisfied) : base(res_on_tech_not_satisfied)
    {
    }

    public override bool errorsFound(Actor pObject)
    {
        return base.errorsFound(pObject) || pObject.GetTribe() == null;
    }
}