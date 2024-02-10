namespace Figurebox.abstracts;

public class AW_AssetLibrary<TAsset, TLibrary> : AssetLibrary<TAsset> where TAsset : Asset
    where TLibrary : AW_AssetLibrary<TAsset, TLibrary>, new()
{
    public static TLibrary Instance { get; } = new TLibrary();

    public override TAsset get(string pID)
    {
        if (string.IsNullOrEmpty(pID))
        {
            Main.LogDebug($"{id}.get: pID is null or empty.", true, true);
            return null;
        }

        return base.get(pID);
    }
}