using Figurebox.content;
using Newtonsoft.Json;

namespace Figurebox.core.city_techs;

public class TechResearchData
{
    private AW_CityTechAsset _m_tech_asset;

    public TechResearchData(AW_CityTechAsset pTechAsset)
    {
        tech_id = pTechAsset.id;
        _m_tech_asset = pTechAsset;
        left_cost = pTechAsset.cost;
    }

    /// <summary>
    ///     科技id
    /// </summary>
    [JsonProperty("tech_id")]
    public string tech_id { get; private set; }

    /// <summary>
    ///     剩余消耗
    /// </summary>
    [JsonProperty("left_cost")]
    public int left_cost { get; private set; }

    /// <summary>
    ///     科技对应的asset
    /// </summary>
    public AW_CityTechAsset tech_asset => _m_tech_asset ??= CityTechLibrary.Instance.get(tech_id);

    public bool Update(AW_City pCity, Actor pResearcher)
    {
        if (left_cost <= 0) return false;

        if (tech_asset.research_action != null && !tech_asset.research_action(pCity, pResearcher, this)) return false;
        left_cost--;
        return true;
    }

    public bool IsFinished()
    {
        return left_cost <= 0;
    }
}