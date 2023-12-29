using System.Collections.Generic;
using Figurebox.core;
namespace Figurebox;

public class KingdomPolicyData : BaseSystemData
{
    public enum PolicyStatus { InPlanning, InProgress, Completed }
    /// <summary>
    ///     当前执行的国策
    /// </summary>
    public string current_policy_id = "";
    /// <summary>
    ///     当前所有政治状态
    /// </summary>
    /// <remarks>
    /// <para>key: 状态类型; value: 状态id</para>
    /// <para>不要直接修改这个的值, 使用<see cref="AW_Kingdom.UpdatePolicyStateTo(string)"/>或<see cref="AW_Kingdom.UpdatePolicyStateTo(KingdomPolicyStateAsset)"/></para>
    /// </remarks>
    public Dictionary<string, string> current_states = new();
    /// <summary>
    ///     国策在当前阶段的进度
    /// </summary>
    public int p_progress = 100;
    /// <summary>
    ///     国策执行状态
    /// </summary>
    public PolicyStatus p_status;
    public double p_timestamp_done;
    public double p_timestamp_start;
    /// <summary>
    ///     执行过的所有国策
    /// </summary>
    public HashSet<string> policy_history = new();
    /// <summary>
    ///     正在等待执行的国策
    /// </summary>
    public Queue<PolicyDataInQueue> policy_queue = new();
    /// <summary>
    ///     年号
    /// </summary>
    public string year_name = "";
    /// <summary>
    ///     年号起用时间
    /// </summary>
    public double year_start_timestamp = 0;
    /// <summary>
    ///     获取指定类型的政策状态id
    /// </summary>
    /// <param name="state_type"></param>
    /// <returns></returns>
    public string GetPolicyStateId(string state_type)
    {
        return string.IsNullOrEmpty(state_type) ? "" : current_states.TryGetValue(state_type, out var ret) ? ret : "";
    }
}