using ai.behaviours;
using AncientWarfare.Const;
using NeoModLoader.api.attributes;

namespace AncientWarfare.Core.AI.ActorBehs;

public class BehCheckHuntCount : BehaviourActionActor
{
    [Hotfixable]
    public override BehResult execute(Actor pObject)
    {
        pObject.data.get(ActorDataKeys.aw_hunt_count, out int hunt_count);
        if (hunt_count >= 10)
        {
            pObject.data.set(ActorDataKeys.aw_hunt_count, 0);
            return BehResult.Continue;
        }

        if (hunt_count > 0)
            Main.LogDebug($"{pObject.data.id} hunt count: {hunt_count}");

        return BehResult.Stop;
    }
}