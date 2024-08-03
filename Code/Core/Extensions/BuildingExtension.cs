using AncientWarfare.Core.Extend;
using AncientWarfare.Core.Force;

namespace AncientWarfare.Core.Extensions;

public static partial class BuildingExtension
{
    public static BuildingAdditionData GetAdditionData(this Building building, bool null_if_not_exists = false)
    {
        return null_if_not_exists
            ? BuildingAdditionDataManager.TryGet(building.data.id)
            : BuildingAdditionDataManager.Get(building.data.id);
    }

    public static bool CanUpgradeForSurrounding(this Building building)
    {
        BuildingAsset upgrade_asset = AssetManager.buildings.get(building.asset.upgradeTo);
        if (upgrade_asset == null)
        {
            Main.LogDebug(
                $"Building {building.asset.id} cannot upgrade because it has not valid upgrade building asset with id \"{building.asset.upgradeTo}\"",
                true, true);
            return false;
        }

        Tribe tribe = building.GetTribe();
        WorldTile tile = building.currentTile;

        return upgrade_asset.CanBuildOn(tile, tribe, ExtendBuildPlacingType.Upgrade);
    }
}