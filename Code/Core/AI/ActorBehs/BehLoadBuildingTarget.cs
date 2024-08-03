using ai.behaviours;
using AncientWarfare.Const;

namespace AncientWarfare.Core.AI.ActorBehs;

public class BehLoadBuildingTarget : BehaviourActionActor
{
    private readonly bool   clean_after_loading;
    private readonly string key;

    public BehLoadBuildingTarget(bool clean_after_loading = true, string reason = "")
    {
        key = reason + ActorDataKeys.aw_stored_building_target;
        this.clean_after_loading = clean_after_loading;
    }

    public override BehResult execute(Actor pObject)
    {
        pObject.data.get(key, out string beh_building_target_id);
        if (clean_after_loading) pObject.data.removeString(key);
        pObject.beh_building_target = World.world.buildings.get(beh_building_target_id);
        if (pObject.beh_building_target == null) return BehResult.Stop;
        return BehResult.Continue;
    }
}