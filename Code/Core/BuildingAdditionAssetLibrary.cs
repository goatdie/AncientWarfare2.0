using AncientWarfare.Abstracts;

namespace AncientWarfare.Core;

public class BuildingAdditionAssetLibrary : AdditionDataManager<BuildingAdditionAsset>
{
    public static BuildingAdditionAsset Get(string id)
    {
        return _data.TryGetValue(id, out BuildingAdditionAsset data) ? data : _data[id] = new BuildingAdditionAsset();
    }
}