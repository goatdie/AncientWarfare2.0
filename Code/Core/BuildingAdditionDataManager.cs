using AncientWarfare.Abstracts;

namespace AncientWarfare.Core;

internal class BuildingAdditionDataManager : AdditionDataManager<BuildingAdditionData>
{
    public static BuildingAdditionData Get(string id)
    {
        return _data.TryGetValue(id, out BuildingAdditionData data) ? data : _data[id] = new BuildingAdditionData();
    }

    public static BuildingAdditionData TryGet(string id)
    {
        return _data.TryGetValue(id, out BuildingAdditionData data) ? data : null;
    }
}