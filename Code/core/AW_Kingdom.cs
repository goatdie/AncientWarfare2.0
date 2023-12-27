using Figurebox.constants;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Figurebox.core;


public class AW_Kingdom : Kingdom
{

    public Actor heir;

    public void clearHeirData()
    {
        this.heir = null;
    }
    public bool hasHeir()
    {
        return this.heir != null;
    }
    public override void Dispose()
    {
        this.heir = null;
        base.Dispose();


    }
    public void SetHeir(Actor pActor)
	{
        this.clearHeirData();
		this.heir= pActor;
		
	}
    public Actor FindHeir()
    {
        List<Actor> candidates = new List<Actor>();

        // 假设继承人必须来自王室家族
        Clan royalClan = BehaviourActionBase<Kingdom>.world.clans.get(this.data.royal_clan_id);
        if (royalClan != null)
        {
            foreach (var member in royalClan.units.Values)
            {
                // 添加适合的候选人
                if (IsSuitableForHeir(member))
                {
                    candidates.Add(member);
                }
            }
        }

        // 根据某些标准排序候选人
        candidates.Sort((a, b) => CompareCandidates(a, b));

        // 返回最合适的候选人
        
           Actor heir = candidates.FirstOrDefault();
    if (heir != null)
    {
        Debug.Log("找到了合适的继承人: " + heir.data.name); 
        return heir;
    }
    else
    {
        Debug.Log("没找到合适的继承人");
        return null;
    }
    }
    private bool IsSuitableForHeir(Actor member)
    {
        // 检查成员是否活着，年龄是否符合，并且不是当前的国王
        return member.isAlive() && !member.isKing();
    }


    private int CompareCandidates(Actor a, Actor b)
    {
        // 定义比较候选人的逻辑，例如根据年龄、领导能力等
        return a.getAge().CompareTo(b.getAge()); // 示例逻辑
    }











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