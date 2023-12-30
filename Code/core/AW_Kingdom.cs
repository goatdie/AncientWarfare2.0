using System.Collections.Generic;
using System.Linq;
using Figurebox.constants;
using Figurebox.Utils.MoH;
using NeoModLoader.api.attributes;
using NeoModLoader.General;
using UnityEngine;
namespace Figurebox.core;

public partial class AW_Kingdom : Kingdom
{

    public Actor heir;
    public bool NameIntegration = false; //控制国家命名是否姓氏合流




    public KingdomPolicyData policy_data = new();

    public void ToggleNameIntegration(bool b)
    {
        this.NameIntegration = b;
    }
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
        this.heir = pActor;

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
    /// <summary>
    ///     更新政策进度
    /// </summary>
    /// <param name="pElapsed"></param>
    public void UpdateForPolicy(float pElapsed)
    {
        // 当目前政策都执行完毕或没有政策时，查找新的政策
        if (policy_data.p_status == KingdomPolicyData.PolicyStatus.Completed || string.IsNullOrEmpty(policy_data.current_policy_id))
        {
            KingdomPolicyAsset next_policy = null;
            if (policy_data.policy_queue.Count > 0)
            {
                var next_policy_in_queue = policy_data.policy_queue.Dequeue();
                next_policy = KingdomPolicyLibrary.Instance.get(next_policy_in_queue.policy_id);

                if (next_policy != null)
                {
                    StartPolicy(next_policy, false, next_policy_in_queue);
                }

                PolicyDataInQueue.Pool.Recycle(next_policy_in_queue);
                return;
            }

            foreach (string state_id in policy_data.current_states.Values)
            {
                var state = KingdomPolicyStateLibrary.Instance.get(state_id);
                if (state == null) continue;
                next_policy = state.policy_finder(this);
                if (!CheckPolicy(next_policy)) continue;

                var policy_data_in_queue = PolicyDataInQueue.Pool.GetNext();
                policy_data_in_queue.policy_id = next_policy.id;
                policy_data_in_queue.progress = next_policy.cost_in_plan;
                policy_data.policy_queue.Enqueue(policy_data_in_queue);
            }
            return;
        }

        // 政策随机进展
        if (Toolbox.randomChance(pElapsed))
        {
            var policy_asset = KingdomPolicyLibrary.Instance.get(policy_data.current_policy_id);
            if (policy_asset == null)
            {
                ForceStopPolicy();
                if (DebugConst.LOG_ALL_EXCEPTION) Main.LogWarning($"政策'{policy_data.current_policy_id}'不存在, 终止", true);
                return;
            }
            // 检查政策是否可用
            if (policy_asset.check_policy != null && !policy_asset.check_policy.Invoke(policy_asset, this))
            {
                ForceStopPolicy();
                return;
            }

            policy_data.p_progress--;
            // 每一帧都执行一次(按照概率计算期望是1秒执行一次), 有待调整
            policy_asset.execute_policy(policy_asset, this);

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
                        policy_data.p_timestamp_done = World.world.mapStats.worldTime;
                        policy_data.policy_history.Add(policy_data.current_policy_id);
                        if (!string.IsNullOrEmpty(policy_asset.target_state_id))
                        {
                            UpdatePolicyStateTo(policy_asset.target_state_id);
                        }
                        break;
                }
            }
        }
    }
    /// <summary>
    ///     检查政策是否可用
    /// </summary>
    /// <param name="pPolicyAsset"></param>
    /// <returns></returns>
    [Hotfixable]
    public bool CheckPolicy(KingdomPolicyAsset pPolicyAsset)
    {
        return pPolicyAsset != null
               && (pPolicyAsset.can_repeat ||
                   !policy_data.policy_history.Contains(pPolicyAsset.id)
                   && (policy_data.p_status == KingdomPolicyData.PolicyStatus.Completed || policy_data.current_policy_id != pPolicyAsset.id))
               && (!pPolicyAsset.only_moh || MoHTools.IsMoHKingdom(this))
               && (pPolicyAsset.all_prepositions == null ||
                   pPolicyAsset.pre_state_require_type == KingdomPolicyAsset.PreStateRequireType.All && pPolicyAsset.all_prepositions.All(pState => policy_data.current_states.ContainsValue(pState)) ||
                   pPolicyAsset.pre_state_require_type == KingdomPolicyAsset.PreStateRequireType.Any && pPolicyAsset.all_prepositions.Any(pState => policy_data.current_states.ContainsValue(pState)))
               && (pPolicyAsset.check_policy == null || pPolicyAsset.check_policy.Invoke(pPolicyAsset, this));
    }
    /// <summary>
    ///     开始尝试执行政策
    /// </summary>
    /// <remarks>
    ///     这个方法会自动检查政策是否为null，是否可用, 如果不可用则不会执行, 可以放心直接调用
    /// </remarks>
    /// <param name="pAsset">目标政策</param>
    /// <param name="pForce">是否将当前正在执行的政策后移</param>
    /// <param name="pPolicyDataInQueue">从队列中取出的政策的执行数据</param>
    /// <returns>是否成功执行</returns>
    public bool StartPolicy(KingdomPolicyAsset pAsset, bool pForce = false, PolicyDataInQueue pPolicyDataInQueue = null)
    {
        if (!CheckPolicy(pAsset)) return false;
        // 正在执行其他政策
        if (!string.IsNullOrEmpty(policy_data.current_policy_id) && policy_data.p_status != KingdomPolicyData.PolicyStatus.Completed)
        {
            if (!pForce) return false;
            PolicyDataInQueue policy_data_in_queue = PolicyDataInQueue.Pool.GetNext();
            policy_data_in_queue.policy_id = policy_data.current_policy_id;
            policy_data_in_queue.progress = policy_data.p_progress;
            policy_data_in_queue.status = policy_data.p_status;
            policy_data_in_queue.timestamp_start = policy_data.p_timestamp_start;
            Queue<PolicyDataInQueue> policy_queue = new();
            policy_queue.Enqueue(policy_data_in_queue);
            while (policy_data.policy_queue.Count > 0)
            {
                policy_queue.Enqueue(policy_data.policy_queue.Dequeue());
            }
            policy_data.policy_queue = policy_queue;
        }

        policy_data.current_policy_id = pAsset.id;
        policy_data.p_progress = pPolicyDataInQueue?.progress ?? pAsset.cost_in_plan;
        policy_data.p_status = pPolicyDataInQueue?.status ?? KingdomPolicyData.PolicyStatus.InPlanning;
        policy_data.p_timestamp_start = pPolicyDataInQueue?.timestamp_start ?? World.world.mapStats.worldTime;
        return true;
    }
    public void ForceStopPolicy()
    {
        policy_data.current_policy_id = "";
    }
    /// <summary>
    ///     更新政治状态
    /// </summary>
    /// <param name="pPolicyStateID">新政治状态的ID</param>
    public void UpdatePolicyStateTo(string pPolicyStateID)
    {
        if (string.IsNullOrEmpty(pPolicyStateID))
        {
            if (DebugConst.LOG_ALL_EXCEPTION) Main.LogWarning($"状态'{nameof(pPolicyStateID)}'为空, 终止", true);
            return;
        }
        UpdatePolicyStateTo(KingdomPolicyStateLibrary.Instance.get(pPolicyStateID));
    }
    /// <summary>
    ///     更新政治状态
    /// </summary>
    /// <param name="pPolicyState">新政治状态</param>
    public void UpdatePolicyStateTo(KingdomPolicyStateAsset pPolicyState)
    {
        if (pPolicyState == null)
        {
            if (DebugConst.LOG_ALL_EXCEPTION) Main.LogWarning($"状态'{nameof(pPolicyState)}'为空, 终止", true);
            return;
        }
        policy_data.current_states[pPolicyState.type] = pPolicyState.id;
    }
    /// <summary>
    ///     获取年号
    /// </summary>
    /// <param name="pWithYearNumber">是否带年份后缀</param>
    /// <returns></returns>
    public string GetYearName(bool pWithYearNumber)
    {
        string text = policy_data.year_name;
        if (string.IsNullOrEmpty(text))
        {
            text = LM.Get("public_year_name");
        }
        if (pWithYearNumber)
        {
            int num = World.world.mapStats.getYearsSince(policy_data.year_start_timestamp) + 1;
            return LM.Get("year_name_format").Replace("$year_name$", text).Replace("$year_number$", num == 1 ? LM.Get("first_year_number") : num.ToString());
        }
        return text;
    }
}