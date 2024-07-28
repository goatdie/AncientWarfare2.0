using ai.behaviours;
using AncientWarfare.Abstracts;
using AncientWarfare.Core.AI.ActorBehs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AncientWarfare.Core.AI
{
    public class ActorTaskExtendLibrary : ExtendedLibrary<BehaviourTaskActor>
    {
        public static readonly BehaviourTaskActor try_join_tribe_here;
        public static readonly BehaviourTaskActor create_tribe_here;
        public static readonly BehaviourTaskActor check_join_empty_nearby_tribe;
        public static readonly BehaviourTaskActor nomad_try_create_tribe;

        public static readonly BehaviourTaskActor collect_fruits;

        public static readonly BehaviourTaskActor check_if_stuck_on_small_land;


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
        }
    }
}
