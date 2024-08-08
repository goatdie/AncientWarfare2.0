using System.Reflection;
using AncientWarfare.Abstracts;
using AncientWarfare.Const;
using AncientWarfare.Core.Additions;
using AncientWarfare.Core.Extensions;
using AncientWarfare.Core.Force;
using NeoModLoader.api.attributes;

namespace AncientWarfare.Core.Content;

public class DebugToolExtendLibrary : ExtendedLibrary<DebugToolAsset>, IManager
{
    public static readonly DebugToolAsset Building_Info;
    public static readonly DebugToolAsset New_Unit_Info;

    public void Initialize()
    {
    }

    protected override void init()
    {
        init_fields();

        modify_assets();

        add(new DebugToolAsset { id = nameof(New_Unit_Info), action_1 = ShowUnitInfo });
    }

    private void modify_assets()
    {
        Building_Info.action_1 = ShowBuildingInfo;
    }

    [Hotfixable]
    private static void ShowUnitInfo(DebugTool tool)
    {
        Actor actor = World.world.getActorNearCursor();
        if (actor == null) return;
        tool.setText("id:",       actor.data.id);
        tool.setText("name:",     actor.getName());
        tool.setText("asset id:", actor.asset.id);
        ActorAdditionData data = actor.GetAdditionData(true);
        if (data != null)
        {
            tool.setText("clan name:",   data.clan_name);
            tool.setText("family name:", data.family_name);
            tool.setSeparator();
            if (data.Forces.Count > 0)
            {
                tool.setText("forces:", "below");
                var idx = 0;
                foreach (var f_id in data.Forces)
                    tool.setText($"[{idx++}]", $"{ForceManager.GetForce<LowBaseForce>(f_id).GetName()}");

                tool.setSeparator();
            }

            if (data.ProfessionDatas?.Count > 0)
            {
                tool.setText("professions:", "below");
                foreach (var p_data in data.ProfessionDatas) tool.setText(p_data.Key, p_data.Value.exp);

                tool.setSeparator();
            }

            if (data.TechsOwned?.Count > 0)
            {
                tool.setText("techs:", "below");
                var idx = 0;
                foreach (var tech in data.TechsOwned) tool.setText($"[{idx++}]", tech);

                tool.setSeparator();
            }
        }
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

    protected override void set_field(DebugToolAsset pObj)
    {
        if (_fields.TryGetValue(pObj.id, out FieldInfo field))
            field.SetValue(null,                                                                 pObj);
        else if (_fields.TryGetValue(pObj.id.Replace(" ", "_"), out field)) field.SetValue(null, pObj);
    }
}