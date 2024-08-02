using ai.behaviours;
using AncientWarfare.Abstracts;
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
        ///     寻找配偶并生育
        /// </summary>
        public static readonly BehaviourTaskActor find_couple_and_make_pregnant;

        /// <summary>
        ///     扩张部落
        /// </summary>
        public static readonly BehaviourTaskActor expand_tribe;

        public static readonly BehaviourTaskActor check_if_stuck_on_small_land;

        /// <summary>
        ///     开始建造或升级仓库
        /// </summary>
        public static readonly BehaviourTaskActor start_build_or_upgrade_storage;

        /// <summary>
        ///     完成建筑任务
        /// </summary>
        public static readonly BehaviourTaskActor construct_building;

        public static readonly BehaviourTaskActor random_move;
        public static readonly BehaviourTaskActor end_job;

        protected override void init()
        {
            init_fields();

            init_jobs_for_unit();
            modify_collect_fruits();
        }

        private void modify_collect_fruits()
        {
            t = collect_fruits;
            t.list.Clear();

            t.addBeh(new BehFindRandomTile());
            t.addBeh(new BehGoToTileTarget());
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
            t.addBeh(new BehRandomWait(2f, 3f));
            // TODO: 等写出实体仓库后移动到仓库提交
            t.addBeh(new BehTribeSubmitResources());
            // TODO: 等写出更具体的需求后改为判断是否重复该任务
            t.addBeh(new BehRestartTask());
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

            add(new BehaviourTaskActor { id = nameof(start_build_or_upgrade_storage) });
            t.addBeh(new BehTribeFindStorageToUpgrade());
            t.addBeh(new BehTribeStartBuilding(SB.order_hall_0));
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
            t.addBeh(new BehLoadBuildingTarget());
            t.addBeh(new BehTribeSubmitConstructBuildingQuest());
        }
    }
}