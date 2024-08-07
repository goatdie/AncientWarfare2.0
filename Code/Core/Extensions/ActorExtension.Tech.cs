using System;
using System.Collections.Generic;
using AncientWarfare.Core.Tech;

namespace AncientWarfare.Core.Extensions;

public static partial class ActorExtension
{
    public static bool HasTech(this Actor actor, string tech_id)
    {
        return actor.GetAdditionData(true)?.TechsOwned?.Contains(tech_id) ?? false;
    }

    public static bool HasTechToUnlock(this Actor actor)
    {
        throw new NotImplementedException();
    }

    public static string FindJobToUnlockTech(this Actor actor, string tech_to_unlock)
    {
        throw new NotImplementedException();
    }

    public static bool WantToStudy(this Actor actor, string tech_to_study)
    {
        throw new NotImplementedException();
    }

    public static string GetNextTechToUnlock(this Actor actor)
    {
        throw new NotImplementedException();
    }

    public static void TrackTechsToUnlock(this Actor actor, List<TechAsset> techs_required)
    {
        throw new NotImplementedException();
    }

    public static void AddTech(this Actor actor, string tech_id)
    {
        ActorAdditionData data = actor.GetAdditionData();
        data.TechsOwned ??= new HashSet<string>();
        data.TechsOwned.Add(tech_id);
    }
}