using AncientWarfare.Core.Extensions;

namespace AncientWarfare.Core.AI.Abstract;

public abstract class BehTribeProfWrapped : BehActionActorProfWrapped
{
    public override bool errorsFound(Actor pObject)
    {
        return base.errorsFound(pObject) || pObject.GetTribe() == null;
    }
}