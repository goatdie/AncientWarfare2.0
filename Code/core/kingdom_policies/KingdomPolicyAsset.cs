using System.Collections.Generic;
using Figurebox.core;
namespace Figurebox;

public delegate void ExecutePolicy(KingdomPolicyAsset policy, AW_Kingdom kingdom, KingdomPolicyData policy_data, KingdomPolicyStateAsset state);
public delegate KingdomPolicyAsset FindPolicy(AW_Kingdom kingdom, KingdomPolicyData policy_data, KingdomPolicyStateAsset state);

public class KingdomPolicyStateAsset : Asset
{
    /// <summary>
    /// 所有当前状态类型可选的政策
    /// </summary>
    public HashSet<string> all_optional_policies;
    /// <summary>
    ///     查找下一个政策的方法
    /// </summary>
    public FindPolicy policy_finder;
}

public class KingdomPolicyAsset : Asset
{
    /// <summary>
    /// 所有前置政策
    /// </summary>
    public HashSet<string> all_prepositions;
    /// <summary>
    /// 所有分支政策
    /// </summary>
    public HashSet<string> branches;
    /// <summary>
    ///     在计划阶段的花费时间
    /// </summary>
    public int cost_in_plan = 100;
    /// <summary>
    ///     在执行阶段的花费时间
    /// </summary>
    public int cost_in_progress = 100;
    /// <summary>
    /// 描述文本的key
    /// </summary>
    public string description;
    /// <summary>
    /// 执行政策的相关逻辑行为
    /// </summary>
    public ExecutePolicy execute_policy;

    public string path_icon;
    /// <summary>
    /// 政策名称的key
    /// </summary>
    public string policyname;
    public Kingdom special_kingdom; //给特定国家的特殊国策
}

public class KingdomPolicyGraphAsset : Asset
{
    /// <summary>
    /// 所有政策
    /// </summary>
    public HashSet<KingdomPolicyAsset> all_policies;
    /// <summary>
    /// 所有状态
    /// </summary>
    public HashSet<KingdomPolicyStateAsset> all_states;
}

// 三个类继承自library