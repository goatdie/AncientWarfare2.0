using ai.behaviours;
using AncientWarfare.Core.AI.Abstract;
using AncientWarfare.Core.Extensions;
using AncientWarfare.Core.Quest;

namespace AncientWarfare.Core.AI.ActorBehs;

public class BehTribeGiveupBuildStorageQuest : BehTribe
{
    public override BehResult execute(Actor pObject)
    {
        pObject.GetTribe().RestartQuest(nameof(QuestLibrary.build_or_upgrade_storage_building));
        return BehResult.Continue;
    }
}