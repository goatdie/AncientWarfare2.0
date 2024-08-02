using ai.behaviours;
using AncientWarfare.Core.Extensions;

namespace AncientWarfare.Core.AI.ActorBehs;

public class BehTribeSubmitConstructBuildingQuest : BehTribe
{
    public override BehResult execute(Actor pObject)
    {
        Building building_to_submit = pObject.beh_building_target;
        pObject.GetTribe().SignalBuildingQuestFinished(building_to_submit);
        return BehResult.Continue;
    }
}