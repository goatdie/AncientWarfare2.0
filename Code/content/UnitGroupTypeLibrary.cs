using Figurebox.abstracts;
using Figurebox.core;

namespace Figurebox.content;

public class UnitGroupTypeLibrary : AW_AssetLibrary<AW_UnitGroupAsset, UnitGroupTypeLibrary>
{
    internal static AW_UnitGroupAsset convention;
    internal static AW_UnitGroupAsset guards;
    internal static AW_UnitGroupAsset slaves;

    public override void init()
    {
        base.init();
        convention = add(new AW_UnitGroupAsset());
        t.id = "convention";
        t.path_icon = "civ/icons/minimap_flag";
        t.base_max_count = 10;

        guards = add(new AW_UnitGroupAsset());
        t.id = "guards";
        t.path_icon = "civ/icons/minimap_guard";
        t.base_max_count = 5;

        slaves = add(new AW_UnitGroupAsset());
        t.id = "slaves";
        t.path_icon = "ui/icons/policy/start_slaves";
        t.base_max_count = 20;
    }
}