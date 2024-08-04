using AncientWarfare.Abstracts;
using AncientWarfare.Core.AI.ActorConds;
using AncientWarfare.Core.Extensions;

namespace AncientWarfare.Core.AI
{
    public class ActorJobExtendLibrary : ExtendedLibrary<ActorJob>
    {
        public static readonly ActorJob unit;
        public static readonly ActorJob random_move;
        public static readonly ActorJob gatherer_bushes;
        public static readonly ActorJob woodcutter;
        public static readonly ActorJob hunter;
        public static readonly ActorJob produce_children;
        public static readonly ActorJob expand_tribe;
        public static readonly ActorJob build_or_upgrade_storage;
        public static readonly ActorJob builder;

        protected override void init()
        {
            init_fields();

            modify_unit_jobs();
            modify_resource_collect_jobs();

            add(new ActorJob { id = nameof(produce_children) });
            t.addTask(nameof(ActorTaskExtendLibrary.find_couple_and_make_pregnant));
            t.addTask(nameof(ActorTaskExtendLibrary.end_job));

            add(new ActorJob { id = nameof(expand_tribe) });
            t.addTask(nameof(ActorTaskExtendLibrary.expand_tribe));
            t.addTask(nameof(ActorTaskExtendLibrary.end_job));

            add(new ActorJob { id = nameof(build_or_upgrade_storage) });
            t.addTask(nameof(ActorTaskExtendLibrary.find_storage_to_upgrade));
            t.addTask(nameof(ActorTaskExtendLibrary.start_build_new_storage));
            t.addCondition(new CondActorHasBuildingTarget(), false);
            t.addTask(nameof(ActorTaskExtendLibrary.construct_building));
            t.addCondition(new CondActorHasBuildingTarget());
            t.addTask(nameof(ActorTaskExtendLibrary.submit_building));
            t.addCondition(new CondActorHasBuildingTarget());
            t.addTask(nameof(ActorTaskExtendLibrary.giveup_build_storage_quest));
            t.addCondition(new CondActorHasBuildingTarget(), false);
            t.addTask(nameof(ActorTaskExtendLibrary.end_job));
        }

        private void modify_resource_collect_jobs()
        {
            t = gatherer_bushes;
            t.InsertTask(-2, nameof(ActorTaskExtendLibrary.submit_resource),
                         new CondActorNeedSubmitResource { expected_result = true });

            t = woodcutter;
            t.InsertTask(-2, nameof(ActorTaskExtendLibrary.submit_resource),
                         new CondActorNeedSubmitResource { expected_result = true });

            t = hunter;
            t.tasks.Clear();
            t.addTask(nameof(ActorTaskExtendLibrary.random_move));
            t.addTask(nameof(ActorTaskExtendLibrary.look_for_animals));
            t.addTask(nameof(ActorTaskExtendLibrary.submit_resource));
            t.addCondition(new CondActorNeedSubmitResource());
            t.addTask(nameof(ActorTaskExtendLibrary.hunter_check_end_job));
        }

        private void modify_unit_jobs()
        {
            t = unit;
            t.tasks.Clear();
            t.addTask(nameof(ActorTaskExtendLibrary.try_join_tribe_here));
            t.addTask(nameof(ActorTaskExtendLibrary.create_tribe_here));
            t.addTask(nameof(ActorTaskExtendLibrary.random_move));
            t.addTask(nameof(ActorTaskExtendLibrary.check_join_empty_nearby_tribe));
            t.addTask(nameof(ActorTaskExtendLibrary.nomad_try_create_tribe));
            t.addTask(nameof(ActorTaskExtendLibrary.check_if_stuck_on_small_land));
            t.addTask(nameof(ActorTaskExtendLibrary.end_job));
        }
    }
}