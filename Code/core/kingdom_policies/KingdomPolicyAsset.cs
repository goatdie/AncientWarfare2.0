using System.Collections.Generic;
using Figurebox.core;
namespace Figurebox;

/// <summary>
///     计算国家实力的方法
/// </summary>
public delegate float CalcKingdomStrength(AW_Kingdom pKingdom);

/// <summary>
///     在政策执行时检查政策是否可用的方法
/// </summary>
public delegate bool CheckPolicy(KingdomPolicyAsset policy, AW_Kingdom kingdom);

/// <summary>
///     执行政策的方法
/// </summary>
public delegate void ExecutePolicy(KingdomPolicyAsset policy, AW_Kingdom kingdom);

/// <summary>
///     寻找下一个政策的方法
/// </summary>
public delegate KingdomPolicyAsset FindPolicy(AW_Kingdom kingdom);

public class KingdomPolicyAsset : Asset
{
    public enum PreStateRequireType
    {
        All, Any
    }

    /// <summary>
    ///   所有前置状态, 可以为空.
    /// </summary>
    public HashSet<string> all_prepositions;
    /// <summary>
    ///     是否能够重复执行
    /// </summary>
    public bool can_repeat;
    /// <summary>
    ///     检查政策是否可用
    /// </summary>
    public CheckPolicy check_policy;
    /// <summary>
    ///   在计划阶段的花费时间
    /// </summary>
    public int cost_in_plan = 100;
    /// <summary>
    ///   在执行阶段的花费时间
    /// </summary>
    public int cost_in_progress = 100;
    /// <summary>
    ///   描述文本的key
    /// </summary>
    public string description;
    /// <summary>
    ///   执行政策的相关逻辑行为
    /// </summary>
    public ExecutePolicy execute_policy;
    /// <summary>
    ///     是否只能由天命国家执行
    /// </summary>
    public bool only_moh;

    public string path_icon;
    /// <summary>
    ///   政策名称的key
    /// </summary>
    public string policyname;
    /// <summary>
    ///   对前置状态的要求类型（ALL: 所有前置状态都必须满足, ANY: 任意一个前置状态满足即可）
    /// </summary>
    public PreStateRequireType pre_state_require_type = PreStateRequireType.All;
    public Kingdom special_kingdom; //给特定国家的特殊国策
    /// <summary>
    ///     执行后添加的政策id, 当为空时, 即不添加新的状态
    /// </summary>
    public string target_state_id;
    /// <summary>
    ///   添加前置状态
    /// </summary>
    public void AddPreposition(string pPrepositionStateID)
    {
        all_prepositions ??= new HashSet<string>();
        all_prepositions.Add(pPrepositionStateID);
    }
}