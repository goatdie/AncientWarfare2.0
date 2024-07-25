using AncientWarfare.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AncientWarfare.Core.AI
{
    public class ActorJobExtendLibrary : ExtendedLibrary<ActorJob>
    {
        
        protected override void init()
        {
            modify_unit_job();
        }

        private void modify_unit_job()
        {
            t = get("unit");
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
