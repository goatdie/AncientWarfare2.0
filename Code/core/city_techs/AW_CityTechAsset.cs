using System.Collections.Generic;
using Figurebox.constants;

namespace Figurebox.core.city_techs;

/// <summary>
///     科技研究行为
/// </summary>
/// <remarks>当用于检查科技研究是否能够开始时, <paramref name="pData" />必定为null</remarks>
/// <returns>作为推进进度时, 返回进度是否推进; 作为检查能否开始时, 返回能否开始;其他无要求</returns>
public delegate bool TechResearchAction(AW_City pCity, Actor pResearcher, TechResearchData pData);

public class AW_CityTechAsset : Asset
{
    /// <summary>
    ///     该科技的所有解锁分支
    /// </summary>
    public HashSet<string> all_branches;

    /// <summary>
    ///     所有前置科技要求
    /// </summary>
    public HashSet<string> all_prepositions;

    /// <summary>
    ///     检查是否能够开始研究
    /// </summary>
    public TechResearchAction check_action;

    /// <summary>
    ///     最多需要消耗的脱产者无差别思考时间
    /// </summary>
    public int cost;

    public string description;

    public string name;
    public string path_icon;

    /// <summary>
    ///     对前置科技的要求类型（ALL: 所有前置状态都必须满足, ANY: 任意一个前置状态满足即可）
    /// </summary>
    public PreStateRequireType pre_state_require_type;

    /// <summary>
    ///     在对应科技类型中的等级
    /// </summary>
    public int rank;

    /// <summary>
    ///     推进进度可用的职业
    /// </summary>
    /// <remarks>
    ///     为空时表示所有职业都可以推进进度
    /// </remarks>
    public HashSet<string> require_citizen_job;

    /// <summary>
    ///     尝试推进进度
    /// </summary>
    public TechResearchAction research_action;

    /// <summary>
    ///     科技分类(以冒号分隔的字符串, 如: "农业:农业科技")
    /// </summary>
    public string tech_category;
}