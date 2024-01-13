using System.Collections.Generic;
using System.Linq;
using Figurebox.constants;

namespace Figurebox;

public class KingdomPolicyStateAsset : Asset
{
    /// <summary>
    ///     所有当前状态类型可选的政策
    /// </summary>
    public HashSet<string> all_optional_policies = new();

    /// <summary>
    ///     国家实力计算方法, 不同政体应当有不同的计算方法
    /// </summary>
    public CalcKingdomStrength calc_kingdom_strength = KingdomPolicyStateLibrary.DefaultCalcKingdomPower;

    /// <summary>
    ///     该政治状态下的城市任务列表, 不包含wait
    /// </summary>
    public List<BehaviourTaskCity> city_task_list = new();

    /// <summary>
    ///     描述文本的key
    /// </summary>
    public string description;

    /// <summary>
    ///     政策名称的key
    /// </summary>
    public string name;

    /// <summary>
    ///     图标路径
    /// </summary>
    public string path_icon;

    /// <summary>
    ///     查找下一个政策的方法
    /// </summary>
    public FindPolicy policy_finder;

    /// <summary>
    ///     状态类型. 如:"地方组织形式", "军队组织形式"等
    /// </summary>
    public string type = "";

    public void AddOptionalPolicy(params KingdomPolicyAsset[] pPolicy)
    {
        all_optional_policies.UnionWith(pPolicy.Select(p => p.id));
    }

    public void AddCityTasks(params BehaviourTaskCity[] pCityTask)
    {
        city_task_list ??= new List<BehaviourTaskCity>();
        if (DebugConst.LOG_ALL_EXCEPTION)
            foreach (var task in pCityTask)
                if (task == null)
                    Main.LogWarning($"There is null task in {id}", true);
        city_task_list.AddRange(pCityTask);
    }
}