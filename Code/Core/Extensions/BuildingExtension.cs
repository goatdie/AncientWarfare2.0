using System;
using AncientWarfare.Const;
using AncientWarfare.Core.Additions;

namespace AncientWarfare.Core.Extensions;

public static partial class BuildingExtension
{
    public static BuildingAdditionData GetAdditionData(this Building building, bool null_if_not_exists = false)
    {
        return null_if_not_exists
            ? BuildingAdditionDataManager.TryGet(building.data.id)
            : BuildingAdditionDataManager.Get(building.data.id);
    }

    public static bool IsFull(this Building building)
    {
        if (building.asset.housing <= 0)
        {
            Main.LogDebug($"You should not judge a building {building.asset.id} without housing as full",
                          pLevel: DebugMsgLevel.Warning, pShowStackTrace: true, pLogOnlyOnce: true);
            return true;
        }

        building.data.get(BuildingDataKeys.curr_housing_int, out int curr_housing);
        return curr_housing >= building.asset.housing;
    }

    public static void ChangeCurrHousing(this Building building, int change)
    {
        building.data.get(BuildingDataKeys.curr_housing_int, out int curr_housing);
        building.data.set(BuildingDataKeys.curr_housing_int, Math.Max(0, curr_housing + change));
    }
}