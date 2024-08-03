using AncientWarfare.Const;

namespace AncientWarfare.Core.AI.ActorConds;

public class CondActorHasBuildingTarget : BehaviourActorCondition
{
    public override bool check(Actor pActor)
    {
        pActor.data.get(ActorDataKeys.aw_stored_building_target, out string building_target_id);
        Building building = World.world.buildings.get(building_target_id);
        return building != null;
    }
}