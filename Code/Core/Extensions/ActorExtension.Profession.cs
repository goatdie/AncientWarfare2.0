using System;
using System.Collections.Generic;
using System.Linq;
using AncientWarfare.Core.Additions;
using AncientWarfare.Core.Profession;
using AncientWarfare.Core.Quest;
using AncientWarfare.Core.Tech;
using NeoModLoader.api.attributes;
using UnityEngine;

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
            ? prof_data.exp_until_now
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

        var old_exp = prof_data.exp_until_now;
        prof_data.AddExp(count);
        CheckTech(actor, data, profession_id, prof_data);
        if (old_exp != (int)Mathf.Log10(prof_data.exp_until_now)) data.JobScoresDirty = true;
    }

    private static void CheckTech(Actor             actor, ActorAdditionData addition_data, string profession_id,
                                  NewProfessionData prof_data)
    {
        var tech_to_get = "";
        if (actor.HasTechToUnlock())
        {
            if (addition_data.TechsOwned == null) return;

            foreach (var tech_id in addition_data.TechToUnlock)
            {
                TechAsset tech = TechLibrary.Instance.get(tech_id);
                if (tech.ProfessionList.Contains(profession_id) && tech.base_cost <= prof_data.exp_left)
                {
                    prof_data.TakeExp(tech.base_cost);
                    tech_to_get = tech.id;
                    break;
                }
            }
        }

        if (string.IsNullOrEmpty(tech_to_get))
            foreach (var tech_id in addition_data.TechsOwned)
            {
                TechAsset tech = TechLibrary.Instance.get(tech_id);
                foreach (TechAsset insp_tech in tech.InspirationList)
                    if (insp_tech.ProfessionList.Contains(profession_id) && insp_tech.base_cost <= prof_data.exp_left)
                    {
                        prof_data.TakeExp(insp_tech.base_cost);
                        tech_to_get = insp_tech.id;
                        break;
                    }

                if (!string.IsNullOrEmpty(tech_to_get)) break;
            }

        if (!string.IsNullOrEmpty(tech_to_get))
            actor.AddTech(tech_to_get);
    }

    public static float ComputeScoreFor(this Actor actor, QuestInst quest)
    {
        var best_score = float.MinValue;
        foreach (ActorJob job in quest.asset.allow_jobs) best_score = Math.Max(actor.ComputeScoreFor(job), best_score);

        return best_score;
    }

    public static float ComputeScoreFor(this Actor actor, ActorJob job)
    {
        ActorJobAdditionAsset addition_asset = job.GetAdditionAsset();

        float score = 0;
        ActorAdditionData addition_data = actor.GetAdditionData(true);
        if (addition_data != null)
            if (addition_data.JobScores.TryGetValue(job.id, out score))
                return score;

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
        addition_data ??= actor.GetAdditionData();
        addition_data.JobScoresInternal[job.id] = score;
        return score;
    }
}