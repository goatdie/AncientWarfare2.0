namespace Figurebox.core;

public class AW_CitiesManager : CitiesManager
{
    internal static void init()
    {
        World.world.cities.clear();
        World.world.cities = new CitiesManager();
        World.world.list_base_managers[World.world.list_base_managers.FindIndex(x => x is CitiesManager)] =
            World.world.cities;
    }

    public override void addObject(City pObject)
    {
        base.addObject(pObject);
        ReconstructCityAI(pObject);
    }

    private void ReconstructCityAI(City pCity)
    {
        pCity.ai.nextJobDelegate = () =>
            ai.CityJobLibrary.CheckAndGetCityJob(((AW_Kingdom)pCity.kingdom).policy_data.current_states);
    }
}