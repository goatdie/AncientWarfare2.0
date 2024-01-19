using ai.behaviours;
using NeoModLoader.api.attributes;

namespace Figurebox.ai.behaviours.actor;

public class BehRawGoToActorTarget : BehaviourActionActor
{
    public bool pathOnWater;
    public string type;

    public BehRawGoToActorTarget(string pType = "sameTile", bool pPathOnWater = false)
    {
        pathOnWater = pPathOnWater;
        type = pType;
    }

    public override void create()
    {
        base.create();
        null_check_actor_target = true;
    }

    [Hotfixable]
    public override BehResult execute(Actor pActor)
    {
        var pTile = pActor.beh_actor_target.currentTile;
        switch (type)
        {
            case "sameTile":
                pTile = pActor.beh_actor_target.currentTile;
                break;
            case "sameRegion":
                pTile = pActor.beh_actor_target.currentTile.region.tiles.GetRandom();
                break;
        }

        var result = pActor.goTo(pTile, pathOnWater);
        Main.LogInfo($"{pActor.data.id} 尝试前往 {pActor.beh_actor_target.base_data.id} 的 {type} {result}");
        return BehResult.Continue;
    }
}