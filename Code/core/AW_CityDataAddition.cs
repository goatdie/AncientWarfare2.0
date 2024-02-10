using System.Collections.Generic;
using Figurebox.core.city_techs;

namespace Figurebox.core;

public class AW_CityDataAddition : BaseSystemData
{
    /// <summary>
    ///     所有已经拥有的科技
    /// </summary>
    public readonly HashSet<string> own_tech = new();

    /// <summary>
    ///     所有正在研究的科技
    /// </summary>
    public readonly Dictionary<string, TechResearchData> tech_research_data = new();
}