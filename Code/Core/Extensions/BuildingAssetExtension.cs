using System;
using AncientWarfare.Core.Force;

namespace AncientWarfare.Core.Extensions;

public static class BuildingAssetExtension
{
    public static BuildingAdditionAsset GetAdditionAsset(this BuildingAsset asset)
    {
        return BuildingAdditionAssetLibrary.Get(asset.id);
    }

    public static bool CanBuildOn(this BuildingAsset asset, WorldTile tile, Tribe tribe,
                                  BuildPlacingType   place_type = BuildPlacingType.New)
    {
        return true;
        throw new NotImplementedException();
    }
}