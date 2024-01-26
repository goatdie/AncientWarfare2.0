using System.Collections.Generic;

namespace Figurebox.core;

public partial class AW_City : City
{
    public List<AW_UnitGroup> groups = new();

    public void CreateGroup(AW_UnitGroupAsset pAsset)
    {
        AW_UnitGroup group = AW_UnitGroupManager.instance.CreateGroup(this, pAsset);
        groups.Add(group);
    }

    public void SetMainGroup(AW_UnitGroup pGroup)
    {
        army = pGroup;
    }
}