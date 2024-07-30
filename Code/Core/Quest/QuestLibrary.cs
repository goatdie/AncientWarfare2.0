using AncientWarfare.Abstracts;
using AncientWarfare.Attributes;
using AncientWarfare.Core.AI;
using AncientWarfare.Utils;

namespace AncientWarfare.Core.Quest;

[ManagerInitializeAfter(typeof(QuestTypeLibrary))]
public class QuestLibrary : AW_AssetLibrary<QuestAsset, QuestLibrary>, IManager
{
    public static readonly QuestAsset food_base_collect;
    public static readonly QuestAsset expand_tribe_for_resource;

    public void Initialize()
    {
        init();
        id = "aw_quests";
    }

    public override void init()
    {
        add(new QuestAsset { id = nameof(food_base_collect) });
        t.type = QuestTypeLibrary.typed_resource_collect;
        t.allow_jobs.Expand(nameof(ActorJobExtendLibrary.gatherer_bushes));

        add(new QuestAsset { id = nameof(expand_tribe_for_resource) });
        t.type = QuestTypeLibrary.tribe_expand_for_resource;
        t.disposable = true;
        t.merge_action_when_repeat = (_, _) => { };
        t.allow_jobs.Expand(nameof(ActorJobExtendLibrary.gatherer_bushes));
    }
}