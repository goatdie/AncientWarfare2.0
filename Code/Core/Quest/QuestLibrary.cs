using System.Collections.Generic;
using AncientWarfare.Abstracts;
using AncientWarfare.Attributes;
using AncientWarfare.Core.AI;
using AncientWarfare.Core.Quest.QuestSettingParams;
using AncientWarfare.Utils;

namespace AncientWarfare.Core.Quest;

[ManagerInitializeAfter(typeof(QuestTypeLibrary))]
public class QuestLibrary : AW_AssetLibrary<QuestAsset, QuestLibrary>, IManager
{
    public static readonly QuestAsset food_base_collect;
    public static readonly QuestAsset chop_wood;
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
        t.given_setting = new Dictionary<string, object>
            { { TypedResourceCollectSettingKeys.resource_type_int, ResType.Food } };
        t.allow_jobs.Expand(nameof(ActorJobExtendLibrary.gatherer_bushes), nameof(ActorJobExtendLibrary.hunter));

        add(new QuestAsset { id = nameof(chop_wood) });
        t.type = QuestTypeLibrary.resource_collect;
        t.disposable = true;
        t.merge_action_when_repeat = QuestTypeDelegates.merge__resource_collect;
        t.given_setting = new Dictionary<string, object> { { ResourceCollectSettingKeys.resource_id_string, SR.wood } };
        t.allow_jobs.Expand(nameof(ActorJobExtendLibrary.woodcutter));

        add(new QuestAsset { id = nameof(expand_tribe_for_resource) });
        t.type = QuestTypeLibrary.tribe_expand_for_resource;
        t.disposable = true;
        t.merge_action_when_repeat = QuestTypeDelegates.empty_merge;
        t.allow_jobs.Expand(nameof(ActorJobExtendLibrary.expand_tribe));

        add(new QuestAsset { id = nameof(build_or_upgrade_storage_building) });
        t.type = QuestTypeLibrary.construct_building;
        t.disposable = true;
        t.multitable = false;
        t.restart_timeout = 120;
        t.merge_action_when_repeat = QuestTypeDelegates.empty_merge;
        t.allow_jobs.Expand(nameof(ActorJobExtendLibrary.build_or_upgrade_storage));
    }
}