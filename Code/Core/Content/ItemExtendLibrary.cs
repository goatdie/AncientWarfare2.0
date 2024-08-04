using AncientWarfare.Abstracts;

namespace AncientWarfare.Core.Content;

public class ItemExtendLibrary : ExtendedLibrary<ItemAsset>
{
    protected override void init()
    {
    }

    protected override AssetLibrary<ItemAsset> find_target_library()
    {
        return AssetManager.items;
    }
}