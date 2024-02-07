using Figurebox.attributes;
using Figurebox.content;

namespace Figurebox.core;

public class AW_UnitGroupManager : UnitGroupManager
{
    public static AW_UnitGroupManager instance;

    internal static void init()
    {
        World.world.unitGroupManager.clear();
        World.world.unitGroupManager = new AW_UnitGroupManager();
        instance = (AW_UnitGroupManager)World.world.unitGroupManager;
    }

    [MethodReplace(nameof(createNewGroup))]
    public UnitGroup CreateNormalGroup(City pCity)
    {
        var data = new AW_UnitGroupData();
        var group = new AW_UnitGroup(pCity, UnitGroupTypeLibrary.convention, data);

        group.id = last_id++;
        data.id = "g_" + group.id;

        groups.Add(group);
        UnitGroupTypeLibrary.convention.on_create?.Invoke(pCity as AW_City, group);
        return group;
    }

    public AW_UnitGroup CreateGroup(AW_City pCity, AW_UnitGroupAsset pAsset)
    {
        var data = new AW_UnitGroupData
        {
            asset_id = pAsset.id
        };
        var group = new AW_UnitGroup(pCity, pAsset, data);

        group.id = last_id++;
        data.id = "g_" + group.id;

        groups.Add(group);

        pAsset.on_create?.Invoke(pCity, group);
        return group;
    }
}