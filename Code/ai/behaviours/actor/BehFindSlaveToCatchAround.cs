using ai.behaviours;
using NeoModLoader.api.attributes;

namespace Figurebox.ai.behaviours.actor;

public class BehFindSlaveToCatchAround : BehaviourActionActor
{
    [Hotfixable]
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
            Main.LogInfo($"找到奴隶捕捉目标{actor.data.id}");
            return BehResult.Continue;
        }

        Main.LogInfo("未找到奴隶捕捉目标");
        return BehResult.RestartTask;
    }
}