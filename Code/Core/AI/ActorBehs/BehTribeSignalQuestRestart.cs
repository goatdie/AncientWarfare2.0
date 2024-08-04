using ai.behaviours;
using AncientWarfare.Const;
using AncientWarfare.Core.Extensions;
using AncientWarfare.Core.Force;

namespace AncientWarfare.Core.AI.ActorBehs;

public class BehTribeSignalQuestRestart : BehTribe
{
    public override BehResult execute(Actor pObject)
    {
        Tribe tribe = pObject.GetTribe();
        pObject.data.get(ActorDataKeys.aw_working_quest_uid_string, out string quest_id);
        tribe.SignalQuestRestart(quest_id);
        return BehResult.Continue;
    }
}