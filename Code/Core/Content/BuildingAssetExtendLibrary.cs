using AncientWarfare.Abstracts;
using AncientWarfare.Core.Extensions;

namespace AncientWarfare.Core.Content;

public class BuildingAssetExtendLibrary : ExtendedLibrary<BuildingAsset>, IManager
{
    public static readonly BuildingAsset bonfire;

    private BuildingAdditionAsset ta => t.GetAdditionAsset();

    public void Initialize()
    {
    }

    protected override void init()
    {
        init_fields();

        t = get(nameof(bonfire));
        t.storage = true;
        ta.storage_size = 10;
        Main.LogDebug($"bonfire initialized with {ta.storage_size}");
    }
}