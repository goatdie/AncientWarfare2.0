using System.Collections.Generic;
using Figurebox.content;
using Figurebox.core.city_techs;

namespace Figurebox.core;

public partial class AW_City
{
    private readonly HashSet<string> _available_techs = new();

    public bool HasTech(string pTechId)
    {
        return addition_data.own_tech.Contains(pTechId);
    }

    public bool BeginTechResearch(string pTechId, Actor pInitiator = null)
    {
        return BeginTechResearch(CityTechLibrary.Instance.get(pTechId), pInitiator);
    }

    public void GetTech(string pTechId)
    {
        if (string.IsNullOrEmpty(pTechId)) return;
        GetTech(CityTechLibrary.Instance.get(pTechId));
    }

    public void GetTech(AW_CityTechAsset pTechAsset)
    {
        if (!addition_data.own_tech.Add(pTechAsset.id)) return;
        addition_data.tech_research_data.Remove(pTechAsset.id);

        _available_techs.UnionWith(pTechAsset.all_branches);
    }

    public bool BeginTechResearch(AW_CityTechAsset pTechAsset, Actor pInitiator = null)
    {
        if (pTechAsset == null) return false;

        if (addition_data.own_tech.Contains(pTechAsset.id)) return false;

        if (addition_data.tech_research_data.ContainsKey(pTechAsset.id)) return false;

        addition_data.tech_research_data.Add(pTechAsset.id, new TechResearchData(pTechAsset));
        _available_techs.Remove(pTechAsset.id);
        return true;
    }
}