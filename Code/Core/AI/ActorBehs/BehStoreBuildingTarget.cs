using ai.behaviours;
using AncientWarfare.Const;

namespace AncientWarfare.Core.AI.ActorBehs;

public class BehStoreBuildingTarget : BehaviourActionActor
{
    private readonly string key;

    public BehStoreBuildingTarget(string reason = "")
    {
        key = reason + ActorDataKeys.aw_stored_building_target;
    }

    public override BehResult execute(Actor pObject)
    {
        pObject.data.set(key, pObject.beh_building_target?.data.id);
        return BehResult.Continue;
    }
}