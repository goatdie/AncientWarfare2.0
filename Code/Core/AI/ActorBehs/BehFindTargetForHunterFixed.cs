using ai.behaviours;
using AncientWarfare.Core.AI.Abstract;

namespace AncientWarfare.Core.AI.ActorBehs;

public class BehFindTargetForHunterFixed : BehTribe
{
    // Token: 0x06002023 RID: 8227 RVA: 0x000F4848 File Offset: 0x000F2A48
    public override BehResult execute(Actor pActor)
    {
        if (pActor.beh_actor_target != null && isTargetOk(pActor, pActor.beh_actor_target.a)) return BehResult.Continue;

        pActor.beh_actor_target = getClosestMeatActor(pActor, 3);
        if (pActor.beh_actor_target != null) return BehResult.Continue;

        return BehResult.Stop;
    }

    // Token: 0x06002024 RID: 8228 RVA: 0x000F489C File Offset: 0x000F2A9C
    private Actor getClosestMeatActor(Actor pActor, int pMinAge = 0, bool pCheckSame = false)
    {
        temp_actors.Clear();
        world.getObjectsInChunks(pActor.currentTile, 10, MapObjectType.Actor);
        for (var i = 0; i < world.temp_map_objects.Count; i++)
        {
            var actor = (Actor)world.temp_map_objects[i];
            if (isTargetOk(pActor, actor) && actor.asset.source_meat && (pMinAge <= 0 || actor.getAge() >= pMinAge))
                temp_actors.Add(actor);
        }

        return Toolbox.getClosestActor(temp_actors, pActor.currentTile);
    }

    private bool isTargetOk(Actor pActor, Actor pTarget)
    {
        return !(pTarget == pActor) && pActor.canAttackTarget(pTarget) &&
               pTarget.currentTile.isSameIsland(pActor.currentTile);
    }
}