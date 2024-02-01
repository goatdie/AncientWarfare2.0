using ai.behaviours;
using Figurebox.abstracts;
using Figurebox.ai.behaviours.city;

namespace Figurebox.ai;

public class CityTaskLibrary : ExtendedLibrary<BehaviourTaskCity>
{
    protected override void init()
    {
        add_slave_city_tasks();
        add_noble_city_tasks();
    }

    private void add_slave_city_tasks()
    {
        add(new BehaviourTaskCity
        {
            id = "check_slave_army"
        });
        t.addBeh(new BehCheckSlaveArmy());
        t.addBeh(new CityBehRandomWait(0.1f));

        add(new BehaviourTaskCity
        {
            id = "check_slave_job"
        });
        t.addBeh(new BehCheckSlaveJobs());
        t.addBeh(new CityBehRandomWait(0.1f));

        add(new BehaviourTaskCity
        {
            id = "produce_slaves"
        });
        t.addBeh(new BehProduceSlaves());
        t.addBeh(new CityBehRandomWait(0.1f));


    }
    private void add_noble_city_tasks()
    {
        add(new BehaviourTaskCity
        {
            id = "produce_noble"
        });
        t.addBeh(new BehProduceNobles());
        t.addBeh(new CityBehRandomWait(0.1f));
        add(new BehaviourTaskCity
        {
            id = "check_guard"
        });
        t.addBeh(new BehCheckGuard());
        t.addBeh(new CityBehRandomWait(0.1f));
        add(new BehaviourTaskCity
        {
            id = "check_retirement"
        });
        t.addBeh(new BehCheckRetirement());
        t.addBeh(new CityBehRandomWait(0.1f));
    }
}