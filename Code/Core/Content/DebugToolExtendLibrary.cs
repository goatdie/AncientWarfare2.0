using System.Reflection;
using AncientWarfare.Abstracts;
using AncientWarfare.Const;
using AncientWarfare.Core.Extensions;

namespace AncientWarfare.Core.Content;

public class DebugToolExtendLibrary : ExtendedLibrary<DebugToolAsset>, IManager
{
    public static readonly DebugToolAsset Building_Info;

    public void Initialize()
    {
    }

    protected override void init()
    {
        init_fields();

        modify_assets();
    }

    private void modify_assets()
    {
        Building_Info.action_1 = ShowBuildingInfo;
    }

    private static void ShowBuildingInfo(DebugTool tool)
    {
        WorldTile mouse_tile = World.world.getMouseTilePos();
        if (mouse_tile == null) return;
        Building building = mouse_tile.building;
        if (building == null) return;
        tool.setText("id:",    building.data.id);
        tool.setText("asset:", building.asset.id);
        tool.setSeparator();
        tool.setText("health", $"{building.data.health}/{building.getMaxHealth()}");
        if (building.asset.housing > 0)
        {
            building.data.get(BuildingDataKeys.curr_housing_int, out int curr_housing);
            tool.setText("housing", $"{curr_housing}/{building.asset.housing}");
        }

        tool.setText("under construction:",
                     building.isUnderConstruction() ? building.getConstructionProgress() : "false");
        tool.setSeparator();
        BuildingAdditionData addition_data = building.GetAdditionData(true);
        if (addition_data != null)
        {
            var idx = 0;
            foreach (var f_id in addition_data.Forces) tool.setText($"force [{idx++}]", f_id);

            if (idx > 0) tool.setSeparator();
        }
    }

    protected override void _set_field(DebugToolAsset pObj)
    {
        if (_fields.TryGetValue(pObj.id, out FieldInfo field))
            field.SetValue(null,                                                                 pObj);
        else if (_fields.TryGetValue(pObj.id.Replace(" ", "_"), out field)) field.SetValue(null, pObj);
    }
}