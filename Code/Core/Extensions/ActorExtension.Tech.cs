using System.Collections.Generic;
using AncientWarfare.Core.Additions;
using AncientWarfare.Core.Tech;

namespace AncientWarfare.Core.Extensions;

public static partial class ActorExtension
{
    public static bool HasTech(this Actor actor, string tech_id)
    {
        return actor.GetAdditionData(true)?.TechsOwned?.Contains(tech_id) ?? false;
    }

    public static bool OverlapTechs(this Actor actor, IEnumerable<string> tech_ids)
    {
        return actor.GetAdditionData(true)?.TechsOwned.Overlaps(tech_ids) ?? false;
    }

    public static bool HasTechToUnlock(this Actor actor)
    {
        ActorAdditionData data = actor.GetAdditionData(true);
        if (data?.TechToUnlock.Count > 0)
        {
            if (data.TechsOwned != null)
            {
                data.TechToUnlock.RemoveAll(data.TechsOwned.Contains);
                return data.TechToUnlock.Count > 0;
            }

            return true;
        }

        return false;
    }

    public static string FindJobToUnlockTech(this Actor actor, string tech_to_unlock)
    {
        TechAsset tech = TechLibrary.Instance.get(tech_to_unlock);
        ActorJob best_job = null;
        var best_score = float.MinValue;
        foreach (ActorJob job in tech.Suggestions)
        {
            var score = actor.ComputeScoreFor(job);
            if (score > best_score)
            {
                best_score = score;
                best_job = job;
            }
        }

        return best_job?.id;
    }

    public static bool WantToStudy(this Actor actor, string tech_to_study)
    {
        return true;
    }

    public static string GetNextTechToUnlock(this Actor actor)
    {
        return actor.GetAdditionData(true)?.TechToUnlock.Last();
    }

    public static void TrackTechsToUnlock(this Actor actor, List<TechAsset> techs_required)
    {
        HashSet<string> techs_to_unlock = new();
        var techs_to_unlock_list = actor.GetAdditionData().TechToUnlock ?? new List<string>();
        while (techs_required.Count > 0)
        {
            TechAsset check_tech = techs_required.Pop();
            if (actor.HasTech(check_tech.id)) continue;
            if (techs_to_unlock.Add(check_tech.id))
            {
                techs_required.AddRange(check_tech.PreliminaryList);
                techs_to_unlock_list.Add(check_tech.id);
            }
        }

        actor.GetAdditionData().TechToUnlock = techs_to_unlock_list;
    }

    public static void AddTech(this Actor actor, string tech_id)
    {
        ActorAdditionData data = actor.GetAdditionData();
        data.TechsOwned ??= new HashSet<string>();
        data.TechsOwned.Add(tech_id);
        data.JobScoresDirty = true;
    }
}