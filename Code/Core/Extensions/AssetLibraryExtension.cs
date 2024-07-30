namespace AncientWarfare.Core.Extensions;

public static class AssetLibraryExtension
{
    public static bool Contains<T>(this AssetLibrary<T> library, string id) where T : Asset
    {
        return !string.IsNullOrEmpty(id) && library.dict.ContainsKey(id);
    }
}