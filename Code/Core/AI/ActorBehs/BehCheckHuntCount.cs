using ai.behaviours;
using AncientWarfare.Const;
using NeoModLoader.api.attributes;

namespace AncientWarfare.Core.AI.ActorBehs;

public class BehCheckHuntCount : BehaviourActionActor
{
    private readonly int max_count;

    public BehCheckHuntCount(int max_count = 0)
    {
        this.max_count = max_count;
    }

    [Hotfixable]
    public override BehResult execute(Actor pObject)
    {
        pObject.data.get(ActorDataKeys.aw_hunt_count_int, out int hunt_count);
        if (hunt_count >= max_count)
        {
            pObject.data.removeInt(ActorDataKeys.aw_hunt_count_int);
            return BehResult.Continue;
        }

        pObject.data.set(ActorDataKeys.aw_hunt_count_int, ++hunt_count);

        return BehResult.Stop;
    }
}