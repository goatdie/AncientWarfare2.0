using Figurebox.core;

namespace Figurebox.content_libraries;

public class UnitGroupTypeLibrary : AssetLibrary<AW_UnitGroupAsset>
{
    public override void init()
    {
        base.init();
        add(new AW_UnitGroupAsset());
        t.id = "convention";
        t.path_icon = "ui/icons/minimap_flag";
        t.base_max_count = 10;

        add(new AW_UnitGroupAsset());
        t.id = "guards";
        t.path_icon = "ui/icons/minimap_flag";
        t.base_max_count = 5;

        add(new AW_UnitGroupAsset());
        t.id = "slaves";
        t.path_icon = "ui/icons/minimap_flag";
        t.base_max_count = 20;
    }
}