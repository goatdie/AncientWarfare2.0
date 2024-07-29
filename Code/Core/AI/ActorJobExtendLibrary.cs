﻿using AncientWarfare.Abstracts;

namespace AncientWarfare.Core.AI
{
    public class ActorJobExtendLibrary : ExtendedLibrary<ActorJob>
    {
        public static readonly ActorJob unit;
        public static readonly ActorJob random_move;
        public static readonly ActorJob gatherer_bushes;
        public static readonly ActorJob produce_children;

        protected override void init()
        {
            init_fields();

            modify_unit_job();
            add_produce_children();
        }

        private void add_produce_children()
        {
            add(new ActorJob { id = nameof(produce_children) });

            t.addTask(nameof(ActorTaskExtendLibrary.find_couple_and_make_pregnant));
            t.addTask(nameof(ActorTaskExtendLibrary.end_job));
        }

        private void modify_unit_job()
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