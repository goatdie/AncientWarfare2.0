using ai.behaviours;
using AncientWarfare.Core.Extensions;

namespace AncientWarfare.Core.AI.Abstract
{
    public class BehTribe : BehaviourActionActor
    {
        public override bool errorsFound(Actor pObject)
        {
            return base.errorsFound(pObject) || pObject.GetTribe() == null;
        }
    }
}