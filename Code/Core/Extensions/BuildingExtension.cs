using System;

namespace AncientWarfare.Core.Extensions;

public static partial class BuildingExtension
{
    public static BuildingAdditionData GetAdditionData(this Building building)
    {
        return BuildingAdditionDataManager.Get(building.data.id);
    }

    public static bool CanUpgradeForSurrounding(this Building building)
    {
        return true;
        throw new NotImplementedException();
    }
}