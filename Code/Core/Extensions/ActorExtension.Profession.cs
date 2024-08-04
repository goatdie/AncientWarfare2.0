using System.Collections.Generic;
using AncientWarfare.Core.Profession;
using AncientWarfare.Core.Tech;
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
        var possible_techs = new List<TechAsset>();
        // TODO: 考虑智力等影响因素
        TechAsset tech_to_unlock = possible_techs.FindAll(tech =>
        {
            return tech.from_production && tech.min_intelligence_required <= actor.data.intelligence &&
                   tech.base_cost                                         <= prof_data.exp;
        }).GetRandom();

        actor.AddTech(tech_to_unlock.id);
        prof_data.exp -= tech_to_unlock.base_cost;
    }
}