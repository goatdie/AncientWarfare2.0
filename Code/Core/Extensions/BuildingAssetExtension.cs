namespace AncientWarfare.Core.Extensions;

public static class BuildingAssetExtension
{
    public static BuildingAdditionAsset GetAdditionAsset(this BuildingAsset asset)
    {
        return BuildingAdditionAssetLibrary.Get(asset.id);
    }
}