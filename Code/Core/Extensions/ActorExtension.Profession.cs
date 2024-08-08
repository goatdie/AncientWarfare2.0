using System;
using System.Collections.Generic;
using System.Linq;
using AncientWarfare.Core.Additions;
using AncientWarfare.Core.Profession;
using AncientWarfare.Core.Quest;
using NeoModLoader.api.attributes;

namespace AncientWarfare.Core.Extensions;

public static partial class ActorExtension
{
    public static bool HasProfession(this Actor actor, string profession_id)
    {
        ActorAdditionData data = actor.GetAdditionData(true);
        return data?.ProfessionDatas?.ContainsKey(profession_id) ?? false;
    }

    public static bool OverlapProfessions(this Actor actor, HashSet<string> professions)
    {
        ActorAdditionData data = actor.GetAdditionData(true);
        if (data?.ProfessionDatas == null) return false;
        foreach (var prof in professions)
            if (!data.ProfessionDatas.ContainsKey(prof))
                return false;

        return true;
    }

    public static int GetProfessionExp(this Actor actor, string profession_id)
    {
        ActorAdditionData data = actor.GetAdditionData(true);
        return data?.ProfessionDatas?.TryGetValue(profession_id, out NewProfessionData prof_data) ?? false
            ? prof_data.exp
            : 0;
    }

    [Hotfixable]
    public static void IncreaseProfessionExp(this Actor actor, string profession_id, int count = 1)
    {
        if (count <= 0)
        {
            Main.LogDebug($"Profession exp increase count(current {count}) should be greater than zero.",
                          pLevel: DebugMsgLevel.Warning, pLogOnlyOnce: true, pShowStackTrace: true);
            return;
        }

        ActorAdditionData data = actor.GetAdditionData();
        data.ProfessionDatas ??= new Dictionary<string, NewProfessionData>();
        if (!data.ProfessionDatas.TryGetValue(profession_id, out NewProfessionData prof_data))
        {
            prof_data = new NewProfessionData();
            data.ProfessionDatas[profession_id] = prof_data;
        }

        prof_data.exp += count;
        //CheckTech(actor, data, profession_id, prof_data);
    }

    private static void CheckTech(Actor             actor, ActorAdditionData addition_data, string profession_id,
                                  NewProfessionData prof_data)
    {
        throw new NotImplementedException();
    }

    public static float ComputeScoreFor(this Actor actor, QuestInst quest)
    {
        var best_score = float.MinValue;
        foreach (var job_id in quest.asset.allow_jobs)
        {
            ActorJob job = AssetManager.job_actor.get(job_id);
            ActorJobAdditionAsset addition_asset = job.GetAdditionAsset();

            float score = 0;
            if (!addition_asset.IsTechsRequiredAccurate())
                score -= job.tasks.Select(container => AssetManager.tasks_actor.get(container.id).GetAdditionAsset())
                            .Count(task_addition => task_addition.IsTechsRequiredAccurate()) * 10;

            var tech_required = addition_asset.GetTechsRequired();
            var prof_about = addition_asset.GetProfessionsAbout();

            if (tech_required.Count > 0)
                score -= (tech_required.Count - tech_required.Count(actor.HasTech)) *
                         NewProfessionLibrary.Instance.Count;

            if (prof_about.Count > 0)
                score += prof_about.Sum(prof => Math.Min((int)Math.Log10(actor.GetProfessionExp(prof)), 3));

            best_score = Math.Max(score, best_score);
        }

        return best_score;
    }
}