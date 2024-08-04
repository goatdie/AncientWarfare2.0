using System.Collections.Generic;
using ai.behaviours;
using AncientWarfare.Const;
using AncientWarfare.Core.AI.Abstract;
using AncientWarfare.Core.Profession;
using NeoModLoader.api.attributes;

namespace AncientWarfare.Core.AI.ActorBehs;

public class BehCheckHuntCount : BehActionActorProfWrapped
{
    private readonly int max_count;

    public BehCheckHuntCount(int max_count = 0)
    {
        this.max_count = max_count;
    }

    public override HashSet<string> ProfessionRequired { get; }

    public override Dictionary<string, int> ProfessionExpGivenPerExecute { get; } = new()
    {
        { nameof(NewProfessionLibrary.hunter), 1 }
    };

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