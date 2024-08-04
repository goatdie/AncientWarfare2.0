using ai.behaviours;
using AncientWarfare.Const;
using AncientWarfare.Core.AI.Abstract;
using AncientWarfare.Core.Extensions;

namespace AncientWarfare.Core.AI.ActorBehs;

public class BehTribeSubmitConstructBuildingQuest : BehTribe
{
    public override BehResult execute(Actor pObject)
    {
        Building building_to_submit = pObject.beh_building_target;
        pObject.GetTribe()
               .SignalBuildingQuestFinished(pObject.data.GetString(ActorDataKeys.aw_working_quest_uid_string),
                                            building_to_submit);
        return BehResult.Continue;
    }
}