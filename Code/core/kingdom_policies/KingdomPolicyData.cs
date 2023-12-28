using System.Collections.Generic;
namespace Figurebox;

public class KingdomPolicyData : BaseSystemData
{
    public enum PolicyStatus { InPlanning, InProgress, Completed }
    /// <summary>
    ///     当前执行的国策
    /// </summary>
    public string current_policy_id = "";
    /// <summary>
    ///     当前所处的政治状态
    /// </summary>
    public string current_state_id = "";
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
    ///     年号
    /// </summary>
    public string year_name = "";
    /// <summary>
    ///     年号起用时间
    /// </summary>
    public double year_start_timestamp = 0;
}