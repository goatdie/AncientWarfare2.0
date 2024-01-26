using System;

namespace Figurebox.core;

public class AW_UnitGroupAsset : Asset
{
    public int                                base_max_count;
    public Func<AW_City, AW_UnitGroup, Actor> find_new_leader;
    public string                             job;
    public Action<AW_City, AW_UnitGroup>      on_create;
    public string                             path_icon;
}