using ai.behaviours;
using Figurebox.abstracts;
using Figurebox.ai.behaviours.actor;

namespace Figurebox.ai;

public class ActorTaskLibrary : ExtendedLibrary<BehaviourTaskActor>
{
    protected override void init()
    {
        add_slave_catcher_tasks();
        add_guard_tasks();
    }

    private void add_slave_catcher_tasks()
    {
        add(new BehaviourTaskActor
        {
            id = "slave_catcher_catch_slave"
        });
        //t.addTaskVerifier(new BehVerifierCanCatchSlave());
        t.addBeh(new BehRandomWait(1f, 2f));
        t.addBeh(new BehFindTileNearbyGroupLeader());
        t.addBeh(new BehRawGoToTileTarget());
        t.addBeh(new BehFindSlaveToCatchAround());
        //t.addBeh(new BehRawGoToActorTarget("sameTile", true));
        t.addBeh(new BehCatchTargetAsSlave());

        add(new BehaviourTaskActor
        {
            id = "slave_catcher_submit_slaves"
        });
        t.addBeh(new BehCityFindBuilding("random_house_building"));
        t.addBeh(new BehFindRandomFrontBuildingTile());
        t.addBeh(new BehRawGoToTileTarget());
        t.addBeh(new BehStayInBuildingTarget(1f, 5f));
        t.addBeh(new BehSubmitSlaves());
        t.addBeh(new BehExitBuilding());

    }
    private void add_guard_tasks()
    {
        add(new BehaviourTaskActor
        {
            id = "guards_follow_king"
        });
        t.addBeh(new BehFindTileKing());
        t.addBeh(new BehGoToTileTarget());
        t.addBeh(new BehRandomWait(1f, 2f));
    }
}