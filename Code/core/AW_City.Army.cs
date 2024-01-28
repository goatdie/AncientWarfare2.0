using System.Collections.Generic;
using Figurebox.constants;

namespace Figurebox.core;

public partial class AW_City : City
{
    public Dictionary<string, AW_UnitGroup> groups = new();

    public void CreateGroup(AW_UnitGroupAsset pAsset)
    {
        if (groups.ContainsKey(pAsset.id))
        {
            if (DebugConst.LOG_ALL_EXCEPTION)
                Main.LogWarning($"AW_City.CreateGroup: group type {pAsset.id} already exists", true);

            return;
        }

        AW_UnitGroup group = AW_UnitGroupManager.instance.CreateGroup(this, pAsset);
        groups[group.asset.id] = group;
        Main.LogWarning("AW_City.CreateGroup: group type " + pAsset.id + " created");
    }

    public void SetMainGroup(AW_UnitGroup pGroup)
    {
        army = pGroup;
    }
}