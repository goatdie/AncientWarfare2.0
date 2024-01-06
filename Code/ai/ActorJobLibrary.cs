using ai.behaviours.conditions;
using Figurebox.abstracts;
using Figurebox.ai.conditions;
using Figurebox.constants;

namespace Figurebox.ai;

public class ActorJobLibrary : ExtendedLibrary<ActorJob>
{
    protected override void init()
    {
        add(new ActorJob
        {
            id = AWS.slave_catcher
        });
        t.addTask("warrior_check_city_army_group");
        t.addCondition(new CondHasUnitGroup());
        t.addCondition(new CondHasSlaveCaught(), false);
        t.addTask("warrior_army_follow_leader");
        t.addCondition(new CondHasSlaveCatchTarget(), false);
        t.addTask("slave_catcher_find_slave");
        t.addCondition(new CondHasSlaveCatchTarget());
        t.addTask("slave_catcher_catch_slave");
        t.addCondition(new CondHasSlaveCaught());
        t.addTask("check_warrior_transport");
        t.addTask("wait");
        t.addTask("try_to_return_home");
        t.addTask("check_if_stuck_on_small_land");
        t.addCondition(new CondInCity());
        t.addTask("slave_catcher_submit_slaves");
    }
}