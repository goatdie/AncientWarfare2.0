using Figurebox.abstracts;

namespace Figurebox.ai;

public class CityTaskLibrary : ExtendedLibrary<BehaviourTaskCity>
{
    protected override void init()
    {
        add_slave_city_tasks();
    }

    private void add_slave_city_tasks()
    {
    }
}