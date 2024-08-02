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
    public static readonly QuestAsset build_or_upgrade_storage_building;

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
        t.merge_action_when_repeat = QuestTypeDelegates.empty_merge;
        t.allow_jobs.Expand(nameof(ActorJobExtendLibrary.expand_tribe));

        add(new QuestAsset { id = nameof(build_or_upgrade_storage_building) });
        t.type = QuestTypeLibrary.construct_building;
        t.disposable = true;
        t.multitable = false;
        t.merge_action_when_repeat = QuestTypeDelegates.empty_merge;
        t.allow_jobs.Expand(nameof(ActorJobExtendLibrary.build_or_upgrade_storage));
    }
}