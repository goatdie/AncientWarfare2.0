using Figurebox.constants;
namespace Figurebox.core;

public class AW_Kingdom : Kingdom
{
    public KingdomPolicyData policy_data = new();
    /// <summary>
    ///     更新政策进度
    /// </summary>
    /// <param name="pElapsed"></param>
    public void UpdateForPolicy(float pElapsed)
    {
        // 当目前政策都执行完毕或没有政策时，查找新的政策
        if (policy_data.p_status == KingdomPolicyData.PolicyStatus.Completed || string.IsNullOrEmpty(policy_data.current_policy_id))
        {
            if (string.IsNullOrEmpty(policy_data.current_state_id))
            {
                policy_data.current_state_id = KingdomPolicyStateLibrary.DefaultState.id;
            }
            var state_asset = KingdomPolicyStateLibrary.Instance.get(policy_data.current_state_id);
            if (state_asset == null)
            {
                if (DebugConst.LOG_ALL_EXCEPTION) Main.LogWarning($"状态'{policy_data.current_state_id}'不存在, 使用默认", true);
                state_asset = KingdomPolicyStateLibrary.DefaultState;
            }

            var next_policy = state_asset.policy_finder?.Invoke(this, policy_data, state_asset);
            if (next_policy == null)
            {
                return;
            }
            StartPolicy(next_policy, false);
            return;
        }
        // 政策随机进展
        if (Toolbox.randomChance(pElapsed))
        {
            policy_data.p_progress--;
            var policy_asset = KingdomPolicyLibrary.Instance.get(policy_data.current_policy_id);
            if (policy_asset == null)
            {
                policy_data.current_policy_id = "";
                if (DebugConst.LOG_ALL_EXCEPTION) Main.LogWarning($"政策'{policy_data.current_policy_id}'不存在, 终止", true);
                return;
            }

            // 每一帧都执行一次(按照概率计算期望是1秒执行一次), 有待调整
            var state_asset = KingdomPolicyStateLibrary.Instance.get(policy_data.current_state_id);
            policy_asset.execute_policy(policy_asset, this, policy_data, state_asset);

            if (policy_data.p_progress <= 0)
            {
                switch (policy_data.p_status)
                {
                    // 完成计划阶段
                    case KingdomPolicyData.PolicyStatus.InPlanning:
                        policy_data.p_progress = policy_asset.cost_in_progress;
                        policy_data.p_status = KingdomPolicyData.PolicyStatus.InProgress;
                        break;
                    // 实施完成
                    case KingdomPolicyData.PolicyStatus.InProgress:
                        policy_data.p_status = KingdomPolicyData.PolicyStatus.Completed;
                        break;
                }
            }
        }
    }
    /// <summary>
    ///     开始尝试执行政策
    /// </summary>
    /// <param name="pAsset">目标政策</param>
    /// <param name="pForce">是否强制覆盖当前正在执行的政策</param>
    public void StartPolicy(KingdomPolicyAsset pAsset, bool pForce)
    {
        // 正在执行其他政策
        if (!string.IsNullOrEmpty(policy_data.current_policy_id) && policy_data.p_status != KingdomPolicyData.PolicyStatus.Completed && !pForce) return;

        policy_data.current_policy_id = pAsset.id;
        policy_data.p_progress = pAsset.cost_in_plan;
        policy_data.p_status = KingdomPolicyData.PolicyStatus.InPlanning;
    }
    public void ForceStopPolicy()
    {

    }
}