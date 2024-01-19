using ai.behaviours;
using Figurebox.constants;
using NeoModLoader.api.attributes;

namespace Figurebox.ai.behaviours.actor;

public class BehFindSlaveToCatchAround : BehaviourActionActor
{
    [Hotfixable]
    public override BehResult execute(Actor pObject)
    {
        if (pObject.kingdom == null) return BehResult.RestartTask;

        World.world.getObjectsInChunks(pObject.currentTile, 8, MapObjectType.Actor);

        foreach (var o in World.world.temp_map_objects)
        {
            var actor = (Actor)o;
            if (!actor.asset.unit) continue;
            if (actor.data.health > actor.stats[S.health] * BehaviourConst.SlaveCaptureHealthThreshold) continue;
            if (!pObject.kingdom.isEnemy(actor.kingdom)) continue;
            if (actor.hasClan()) continue;
            pObject.beh_actor_target = actor;
            Main.LogInfo($"{pObject.data.id}找到奴隶捕捉目标{actor.data.id}");
            return BehResult.Continue;
        }

        //Main.LogInfo("未找到奴隶捕捉目标");
        return BehResult.RestartTask;
    }
}