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


        public static readonly BehaviourTaskActor check_if_stuck_on_small_land;


        public static readonly BehaviourTaskActor random_move;
        public static readonly BehaviourTaskActor end_job;
        protected override void init()
        {
            init_fields();

            init_jobs_for_unit();
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
