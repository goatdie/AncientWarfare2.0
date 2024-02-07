namespace Figurebox.abstracts;

public class AW_AssetLibrary<TAsset, TLibrary> : AssetLibrary<TAsset> where TAsset : Asset
    where TLibrary : AW_AssetLibrary<TAsset, TLibrary>, new()
{
    public static TLibrary Instance { get; } = new TLibrary();
}