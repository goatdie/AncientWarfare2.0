using System.Collections.Generic;
using System.Linq;
using AncientWarfare.Core.Extend;
using AncientWarfare.Core.Force;
using tools;

namespace AncientWarfare.Core.Extensions;

public static class BuildingAssetExtension
{
    private static readonly List<WorldTile> temp_for_CanBuildOn = new();

    public static BuildingAdditionAsset GetAdditionAsset(this BuildingAsset asset)
    {
        return BuildingAdditionAssetLibrary.Get(asset.id);
    }

    public static bool CanBuildOn(this BuildingAsset     asset, WorldTile center_tile, Tribe tribe,
                                  ExtendBuildPlacingType place_type = ExtendBuildPlacingType.New)
    {
        var x_min = center_tile.x - asset.fundament.left;
        var y_min = center_tile.y - asset.fundament.bottom;
        var x_max = center_tile.x + asset.fundament.right;
        var y_max = center_tile.y + asset.fundament.top;

        var exist_suitable_for_dock = false;
        var any_unsuitable_for_dock = false;
        temp_for_CanBuildOn.Clear();
        var tiles = temp_for_CanBuildOn;
        for (var x = x_min; x <= x_max; x++)
        for (var y = y_min; y <= y_max; y++)
        {
            WorldTile check_tile = World.world.GetTile(x, y);
            if (check_tile == null) return false;
            if (place_type == ExtendBuildPlacingType.Upgrade)
                if (check_tile.building.asset.id == asset.upgradedFrom  &&
                    check_tile.building.currentTile.Equals(center_tile) && check_tile.building.GetTribe() == tribe)
                    continue;

            // 码头水量检查
            if (asset.docks && !any_unsuitable_for_dock)
            {
                tiles.Add(check_tile);
                if (check_tile.Type.ocean && OceanHelper.goodForNewDock(check_tile)) exist_suitable_for_dock = true;

                if (check_tile.Type.ground)
                    any_unsuitable_for_dock = true;
            }

            // 部落要求
            if (tribe != null && tribe.CenterTile != null)
            {
                if (asset.docks && !check_tile.isSameIsland(tribe.CenterTile)) return false;

                if (check_tile.zone.GetTribe() != tribe) return false;
            }

            // 建筑通用要求
            if (asset.onlyBuildTiles && !check_tile.Type.canBuildOn) return false;

            if (asset.canBePlacedOnlyOn?.Count > 0 && asset.canBePlacedOnlyOn.Contains(check_tile.Type.id))
                return false;

            if (asset.destroyOnLiquid && check_tile.Type.ocean) // 保留原版逻辑
                return false;

            if (!check_tile.canBuildOn(asset)) return false;

            // 特殊检查
            if (asset.vegetation)
            {
                if (check_tile.building != null && !check_tile.building.isRuin() &&
                    (check_tile.building.asset.buildingType != BuildingType.Plant ||
                     asset.buildingType                     != BuildingType.Tree))
                    return false;

                if (!check_tile.canGrow()) return false;
            }

            // 边界检查(避免建筑过近)
            if (!asset.checkForCloseBuilding || place_type == ExtendBuildPlacingType.Load) continue;

            if (x == x_min)
            {
                if (IsBuildingNearby(check_tile.tile_left)) return false;
            }
            else if (x == x_max)
            {
                if (IsBuildingNearby(check_tile.tile_right)) return false;
            }

            if (y == y_min)
            {
                if (IsBuildingNearby(check_tile.tile_down)) return false;
            }
            else if (y == y_max)
            {
                if (IsBuildingNearby(check_tile.tile_up)) return false;
            }
        }

        if (asset.docks && place_type == ExtendBuildPlacingType.New)
        {
            if (exist_suitable_for_dock && !any_unsuitable_for_dock)
                return tiles.Any(check_tile =>
                                     check_tile.neighbours.Any(
                                         neigh => neigh.Type.ground &&
                                                  neigh.region.island == tribe?.CenterTile?.region.island));

            return false;
        }

        return true;
    }

    private static bool IsBuildingNearby(WorldTile tile)
    {
        if (tile == null) return true;

        if (tile.building != null && tile.building.isUsable() && tile.building.GetTribe() != null) return true;

        return false;
    }
}