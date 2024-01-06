using ai.behaviours;

namespace Figurebox.ai.behaviours.actor;

public class BehFindSlaveToCatchAround : BehaviourActionActor
{
    public override BehResult execute(Actor pObject)
    {
        World.world.getObjectsInChunks(pObject.currentTile, 8, MapObjectType.Actor);

        foreach (var o in World.world.temp_map_objects)
        {
            var actor = (Actor)o;
            if (!actor.asset.unit) continue;
            if (!pObject.kingdom?.isEnemy(actor.kingdom) ?? false) continue;

            pObject.beh_actor_target = actor;
            pObject.setAttackTarget(actor);
        }

        return BehResult.RestartTask;
    }
}