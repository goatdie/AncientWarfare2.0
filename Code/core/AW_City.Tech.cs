using System.Collections.Generic;
using Figurebox.constants;
using Figurebox.content;
using Figurebox.core.city_techs;
using Figurebox.utils;

namespace Figurebox.core;

public partial class AW_City
{
    private readonly HashSet<string> _available_techs = new();
    private readonly Dictionary<string, List<TechResearchData>> _tech_under_research = new();
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
        _removeTechFromUnderResearch(pTechAsset);

        _available_techs.UnionWith(pTechAsset.all_branches);
    }

    public bool BeginTechResearch(AW_CityTechAsset pTechAsset, Actor pInitiator = null)
    {
        if (pTechAsset == null) return false;

        if (addition_data.own_tech.Contains(pTechAsset.id)) return false;

        if (addition_data.tech_research_data.ContainsKey(pTechAsset.id)) return false;

        _addTechUnderResearch(pTechAsset, pInitiator);
        _available_techs.Remove(pTechAsset.id);
        return true;
    }
    /// <summary>
    /// <paramref name="pActor"/>以<paramref name="pJob"/>的身份推动某一项研究进展
    /// </summary>
    public void PushResearchThrough(Actor pActor, CitizenJobAsset pJob)
    {
        if (!_tech_under_research.TryGetValue(pJob.id, out var research_list) || research_list.Count == 0)
        {
            if (!_tech_under_research.TryGetValue(string.Empty, out research_list) || research_list.Count == 0) return;
        }
        var random_research = research_list.GetRandom();
        random_research.Update(this, pActor);
    }
    private void _addTechUnderResearch(AW_CityTechAsset pTechAsset, Actor pInitiator)
    {
        addition_data.tech_research_data.Add(pTechAsset.id, new TechResearchData(pTechAsset));
        if (pTechAsset.require_citizen_job != null && pTechAsset.require_citizen_job.Count > 0)
        {
            foreach (var job in pTechAsset.require_citizen_job)
            {
                if (!_tech_under_research.ContainsKey(job))
                {
                    _tech_under_research.Add(job, new List<TechResearchData>());
                }
                _tech_under_research[job].Add(addition_data.tech_research_data[pTechAsset.id]);
            }
        }
        else
        {
            if (!_tech_under_research.ContainsKey(string.Empty))
            {
                _tech_under_research.Add(string.Empty, new List<TechResearchData> { addition_data.tech_research_data[pTechAsset.id] });
            }
            else
            {
                _tech_under_research[string.Empty].Add(addition_data.tech_research_data[pTechAsset.id]);
            }
        }
    }
    private void _removeTechFromUnderResearch(AW_CityTechAsset pTechAsset)
    {
        addition_data.tech_research_data.Remove(pTechAsset.id);
        if (pTechAsset.require_citizen_job != null && pTechAsset.require_citizen_job.Count > 0)
        {
            foreach (var job in pTechAsset.require_citizen_job)
            {
                if (!_tech_under_research.ContainsKey(job)) continue;
                _tech_under_research[job].RemoveAll(data => data.tech_id == pTechAsset.id);
            }
        }
        else
        {
            _tech_under_research[string.Empty].RemoveAll(data => data.tech_id == pTechAsset.id);
        }
    }
    /// <summary>
    /// 总结科技研究成果
    /// </summary>
    private void updateTechResearch()
    {
        foreach (var research in addition_data.tech_research_data)
        {
            if (!research.Value.IsFinished()) continue;
            // TODO: 移出正在研究, 加入已拥有
        }
    }
}