using ai.behaviours;
using AncientWarfare.Core.AI.Abstract;
using AncientWarfare.Core.Extensions;

namespace AncientWarfare.Core.AI.ActorBehs
{
    public class BehTribeSubmitResources : BehTribe
    {
        public override BehResult execute(Actor pObject)
        {
            pObject.SubmitInventoryResourcesToTribe();
            return BehResult.Continue;
        }
    }
}