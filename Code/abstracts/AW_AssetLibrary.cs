namespace Figurebox.abstracts;

public class AW_AssetLibrary<TAsset, TLibrary> : AssetLibrary<TAsset> where TAsset : Asset
    where TLibrary : AW_AssetLibrary<TAsset, TLibrary>
{
    public static TLibrary Instance;

    public override void init()
    {
        base.init();
        Instance = (TLibrary)this;
    }
}