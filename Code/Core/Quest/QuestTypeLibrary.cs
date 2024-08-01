using AncientWarfare.Abstracts;

namespace AncientWarfare.Core.Quest;

public class QuestTypeLibrary : AW_AssetLibrary<QuestTypeAsset, QuestTypeLibrary>, IManager
{
    public static readonly QuestTypeAsset typed_resource_collect;
    public static readonly QuestTypeAsset resource_collect;
    public static readonly QuestTypeAsset construct_building;
    public static readonly QuestTypeAsset tribe_expand_for_resource;

    public void Initialize()
    {
        init();
        id = "aw_quest_types";
    }

    public override void init()
    {
        add(new QuestTypeAsset { id = nameof(typed_resource_collect) });
        t.InitDelegate = QuestTypeDelegates.init__typed_resource_collect;
        t.UpdateDelegate = QuestTypeDelegates.update__typed_resource_collect;

        add(new QuestTypeAsset { id = nameof(resource_collect) });

        add(new QuestTypeAsset { id = nameof(tribe_expand_for_resource) });
        t.InitDelegate = QuestTypeDelegates.init__tribe_expand_for_resource;
        t.UpdateDelegate = QuestTypeDelegates.update__tribe_expand_for_resource;

        add(new QuestTypeAsset { id = nameof(construct_building) });
        t.InitDelegate = QuestTypeDelegates.init__construct_building;
        t.UpdateDelegate = QuestTypeDelegates.update__construct_building;
    }
}