using System.Collections.Generic;

namespace AncientWarfare.Core.Extensions;

public static partial class ActorExtension
{
    public static bool HasTech(this Actor actor, string tech_id)
    {
        return actor.GetAdditionData(true)?.TechsOwned?.Contains(tech_id) ?? false;
    }

    public static void AddTech(this Actor actor, string tech_id)
    {
        ActorAdditionData data = actor.GetAdditionData();
        data.TechsOwned ??= new HashSet<string>();
        data.TechsOwned.Add(tech_id);
    }
}