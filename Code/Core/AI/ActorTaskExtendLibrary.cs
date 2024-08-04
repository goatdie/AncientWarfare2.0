using ai.behaviours;
using AncientWarfare.Abstracts;
using AncientWarfare.Const;
using AncientWarfare.Core.AI.ActorBehs;

namespace AncientWarfare.Core.AI
{
    public class ActorTaskExtendLibrary : ExtendedLibrary<BehaviourTaskActor>
    {
        /// <summary>
        ///     尝试原地加入部落
        /// </summary>
        public static readonly BehaviourTaskActor try_join_tribe_here;

        /// <summary>
        ///     尝试原地创建部落
        /// </summary>
        public static readonly BehaviourTaskActor create_tribe_here;

        /// <summary>
        ///     在周围寻找人数较少的部落并直接加入
        /// </summary>
        public static readonly BehaviourTaskActor check_join_empty_nearby_tribe;

        /// <summary>
        ///     找到合适的地点创建部落
        /// </summary>
        public static readonly BehaviourTaskActor nomad_try_create_tribe;

        /// <summary>
        ///     收集水果/蔬菜
        /// </summary>
        public static readonly BehaviourTaskActor collect_fruits;

        /// <summary>
        ///     寻找动物并狩猎
        /// </summary>
        public static readonly BehaviourTaskActor look_for_animals;

        /// <summary>
        ///     猎人检查任务是否结束(血量不足/饥饿/达到狩猎次数)
        /// </summary>
        public static readonly BehaviourTaskActor hunter_check_end_job;

        /// <summary>
        ///     砍树
        /// </summary>
        public static readonly BehaviourTaskActor chop_trees;

        /// <summary>
        ///     寻找配偶并生育
        /// </summary>
        public static readonly BehaviourTaskActor find_couple_and_make_pregnant;

        /// <summary>
        ///     寻找地块扩张部落
        /// </summary>
        public static readonly BehaviourTaskActor expand_tribe;

        /// <summary>
        ///     扩张部落
        /// </summary>
        public static readonly BehaviourTaskActor expand_tribe_raw;

        public static readonly BehaviourTaskActor check_if_stuck_on_small_land;

        /// <summary>
        ///     寻找可以升级的仓库
        /// </summary>
        public static readonly BehaviourTaskActor find_storage_to_upgrade;

        /// <summary>
        ///     寻找可以升级的房屋
        /// </summary>
        public static readonly BehaviourTaskActor find_housing_to_upgrade;

        /// <summary>
        ///     开始建造新的房屋
        /// </summary>
        public static readonly BehaviourTaskActor start_build_new_housing;

        /// <summary>
        ///     开始建造新的仓库
        /// </summary>
        public static readonly BehaviourTaskActor start_build_new_storage;

        /// <summary>
        ///     建造新建筑
        /// </summary>
        public static readonly BehaviourTaskActor build_new_building;

        /// <summary>
        ///     完成建筑任务
        /// </summary>
        public static readonly BehaviourTaskActor construct_building;

        /// <summary>
        ///     提交建筑任务
        /// </summary>
        public static readonly BehaviourTaskActor submit_building;

        /// <summary>
        ///     提交资源
        /// </summary>
        public static readonly BehaviourTaskActor submit_resource;

        /// <summary>
        ///     放弃任务
        /// </summary>
        public static readonly BehaviourTaskActor giveup_quest;

        public static readonly BehaviourTaskActor random_move;
        public static readonly BehaviourTaskActor end_job;

        protected override void init()
        {
            init_fields();

            init_jobs_for_unit();
            modify_build_new_building();
            modify_look_for_animals();
            modify_collect_fruits();
            modify_chop_trees();
        }

        private void modify_build_new_building()
        {
            t = build_new_building;
            t.list[0] = new BehTribeFindBuilding(BuildTargetType.new_building);
            t.list[t.list.Count - 1] = new BehCheckTargetBuildProgressFixed();
        }

        private void modify_look_for_animals()
        {
            t = look_for_animals;
            t.list[0] = new BehFindTargetForHunterFixed();
        }

        private void modify_chop_trees()
        {
            t = chop_trees;
            t.list.Clear();

            t.addBeh(new BehTribeFindBuilding(SB.type_tree));
            t.addBeh(new BehFindRandomFrontBuildingTile());
            t.addBeh(new BehGoToTileTarget());
            t.addBeh(new BehLookAtTarget("building_target"));
            t.addBeh(new BehResourceGatheringAnimation(1f, "event:/SFX/CIVILIZATIONS/ChopTree"));
            t.addBeh(new BehResourceGatheringAnimation(1f, "event:/SFX/CIVILIZATIONS/ChopTree"));
            t.addBeh(new BehResourceGatheringAnimation(1f, "event:/SFX/CIVILIZATIONS/ChopTree"));
            t.addBeh(new BehResourceGatheringAnimation(1f, "event:/SFX/CIVILIZATIONS/ChopTree"));
            t.addBeh(new BehResourceGatheringAnimation(1f, "event:/SFX/CIVILIZATIONS/ChopTree"));
            t.addBeh(new BehResourceGatheringAnimation(1f, "event:/SFX/CIVILIZATIONS/ChopTree"));
            t.addBeh(new BehResourceGatheringAnimation(1f, "event:/SFX/CIVILIZATIONS/ChopTree"));
            t.addBeh(new BehResourceGatheringAnimation(0f, "event:/SFX/CIVILIZATIONS/ChopTree"));
            t.addBeh(new BehExtractResourcesFromBuilding());
        }

        private void modify_collect_fruits()
        {
            t = collect_fruits;
            t.list.Clear();

            t.addBeh(new BehFindRandomTile());
            t.addBeh(new BehGoToTileTarget());
            t.addBeh(new BehTribeExpandZone());
            t.addBeh(new BehRandomWait(0, 3));
            t.addBeh(new BehTribeFindBuilding(SB.type_fruits, SB.type_vegetation));
            t.addBeh(new BehFindRandomFrontBuildingTile());
            t.addBeh(new BehGoToTileTarget());
            t.addBeh(new BehLookAtTarget("building_target"));
            t.addBeh(new BehResourceGatheringAnimation(1f, "event:/SFX/CIVILIZATIONS/CollectFruits"));
            t.addBeh(new BehResourceGatheringAnimation(1f, "event:/SFX/CIVILIZATIONS/CollectFruits"));
            t.addBeh(new BehResourceGatheringAnimation(1f, "event:/SFX/CIVILIZATIONS/CollectFruits"));
            t.addBeh(new BehResourceGatheringAnimation(1f, "event:/SFX/CIVILIZATIONS/CollectFruits"));
            t.addBeh(new BehResourceGatheringAnimation(0f, "event:/SFX/CIVILIZATIONS/CollectFruits"));
            t.addBeh(new BehExtractResourcesFromBuilding());
            t.addBeh(new BehTribeFindStorage());
            t.addBeh(new BehFindRandomFrontBuildingTile());
            t.addBeh(new BehGoToTileTarget());
            t.addBeh(new BehTribeSubmitResources());
            t.addBeh(new BehRandomWait(1, 2));
        }

        private void init_jobs_for_unit()
        {
            add(new BehaviourTaskActor() { id = nameof(try_join_tribe_here) });
            t.addBeh(new BehJoinTribe());
            t.addBeh(new BehEndJob());

            add(new BehaviourTaskActor() { id = nameof(create_tribe_here) });
            t.addBeh(new BehCheckCreateTribe());
            t.addBeh(new BehEndJob());

            add(new BehaviourTaskActor() { id = nameof(check_join_empty_nearby_tribe) });
            t.addBeh(new BehFindEmptyNearbyTribe());
            t.addBeh(new BehEndJob());

            add(new BehaviourTaskActor() { id = nameof(nomad_try_create_tribe) });
            t.addBeh(new BehRandomWait(1, 5));
            t.addBeh(new BehFindTileForTribe());
            t.addBeh(new BehGoToTileTarget());
            t.addBeh(new BehCheckCreateTribe());
            t.addBeh(new BehEndJob());

            add(new BehaviourTaskActor { id = nameof(find_couple_and_make_pregnant) });
            t.addBeh(new BehTribeFindCouple());
            t.addBeh(new BehGoToActorTarget());
            t.addBeh(new BehMakePregnant());

            add(new BehaviourTaskActor { id = nameof(expand_tribe) });
            t.addBeh(new BehTribeFindZoneToExpand());
            t.addBeh(new BehRandomWait(1, 5));
            t.addBeh(new BehTribeExpandZone());

            add(new BehaviourTaskActor { id = nameof(expand_tribe_raw) });
            t.addBeh(new BehTribeExpandZone());

            add(new BehaviourTaskActor { id = nameof(find_storage_to_upgrade) });
            t.addBeh(new BehTribeFindBuildingToUpgrade(b => b.asset.storage));
            t.addBeh(new BehStoreBuildingTarget());

            add(new BehaviourTaskActor { id = nameof(start_build_new_storage) });
            t.addBeh(new BehTribeStartBuilding(SB.order_hall_0));
            t.addBeh(new BehStoreBuildingTarget());

            add(new BehaviourTaskActor { id = nameof(find_housing_to_upgrade) });
            t.addBeh(new BehTribeFindBuildingToUpgrade(b => b.asset.type == SB.type_house));
            t.addBeh(new BehStoreBuildingTarget());

            add(new BehaviourTaskActor { id = nameof(start_build_new_housing) });
            t.addBeh(new BehTribeStartBuilding(SB.order_house_0));
            t.addBeh(new BehStoreBuildingTarget());

            add(new BehaviourTaskActor { id = nameof(construct_building) });
            t.addBeh(new BehLoadBuildingTarget(false));
            t.addBeh(new BehFindConstructionTile());
            t.addBeh(new BehGoToTileTarget());
            t.addBeh(new BehLookAtTarget("building_target"));
            t.addBeh(new BehAngleAnimation("building_target", "event:/SFX/BUILDINGS/СonstructionBuildingGeneric"));
            t.addBeh(new BehBuildTargetProgressFixed());
            t.addBeh(new BehRandomWait(0.5f));
            t.addBeh(new BehCheckTargetBuildProgressFixed());

            add(new BehaviourTaskActor { id = nameof(submit_building) });
            t.addBeh(new BehLoadBuildingTarget());
            t.addBeh(new BehTribeSubmitConstructBuildingQuest());

            add(new BehaviourTaskActor { id = nameof(submit_resource) });
            t.addBeh(new BehTribeFindStorage());
            t.addBeh(new BehFindRandomFrontBuildingTile());
            t.addBeh(new BehGoToTileTarget());
            t.addBeh(new BehTribeSubmitResources());
            t.addBeh(new BehRandomWait(1, 2));

            add(new BehaviourTaskActor { id = nameof(giveup_quest) });
            t.addBeh(new BehTribeSignalQuestRestart());

            add(new BehaviourTaskActor { id = nameof(hunter_check_end_job) });
            t.addBeh(new BehCheckHuntCount());
            t.addBeh(new BehCheckHealth(reverse: true));
            t.addBeh(new BehEndJob());
        }
    }
}