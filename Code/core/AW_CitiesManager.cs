using Figurebox.core.events;

namespace Figurebox.core;

public class AW_CitiesManager : CitiesManager
{
    internal static void init()
    {
        World.world.cities.clear();
        World.world.cities = new AW_CitiesManager();
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
            ai.CityJobLibrary.CheckAndGetCityJob(((AW_Kingdom)pCity.kingdom).addition_data.current_states);
    }

    public override City loadObject(CityData pData)
    {
        if (!checkDataOk(pData)) return null;
        AW_City val = new();
        val.setHash(_latest_hash++);
        val.loadData(pData);
        addObject(val);

        val.loadCity(pData);
        return val;
    }

    public override City newObject(string pSpecialID = null)
    {
        AW_City new_city = new();
        new_city.setHash(_latest_hash++);
        CityData tdata = new();
        tdata.id = string.IsNullOrEmpty(pSpecialID) ? World.world.mapStats.getNextId(type_id) : pSpecialID;
        tdata.created_time = World.world.getCreationTime();
        new_city.data = tdata;

        addObject(new_city);
        return new_city;
    }

    public override void removeObject(City pObject)
    {
        base.removeObject(pObject);
        EventsManager.Instance.EndCity(pObject);
    }
}