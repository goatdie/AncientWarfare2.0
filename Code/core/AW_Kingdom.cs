using System.Collections.Generic;
using System.Linq;
using System;
using Figurebox.attributes;
using Figurebox.constants;
using Figurebox.patch.MoH;
using Figurebox.Utils;
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
        return heir != null && heir.isAlive();
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
                // 排除国王本人
                if (this.king != null && member == this.king && !member.has_trait_madness)
                {
                    continue;
                }

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
           // Debug.Log("找到了合适的继承人: " + heir.data.name);
            return heir;
        }
        else
        {
           // Debug.Log("没找到合适的继承人");
            return null;
        }
    }
    public void CheckHeir()
    {
        if (this.heir != null)
        {//等老马更新后检测继承人是否为自己的子嗣
            if (heir.has_trait_madness || heir == king)
            {
                this.clearHeirData();
            }
        }
    }

    private bool IsSuitableForHeir(Actor member)
    {
        // 检查成员是否活着，年龄是否符合，并且不是当前的国王
        return member.isAlive() && !member.isKing();
    }


    private int CompareCandidates(Actor a, Actor b)
    {
        // 男性优先
        if (a.data.gender == ActorGender.Male && b.data.gender != ActorGender.Male)
        {
            return -10;
        }
        else if (a.data.gender != ActorGender.Male && b.data.gender == ActorGender.Male)
        {
            return 10;
        }

        // 比较影响力，影响力更高的优先
        int influenceComparison = b.getInfluence().CompareTo(a.getInfluence());
        if (influenceComparison != 0)
        {
            return influenceComparison;
        }

        // 比较属性总和，总和更高的优先
        int aTotalAttributes = a.data.diplomacy + a.data.intelligence + a.data.stewardship + a.data.warfare;
        int bTotalAttributes = b.data.diplomacy + b.data.intelligence + b.data.stewardship + b.data.warfare;
        int attributeComparison = bTotalAttributes.CompareTo(aTotalAttributes);
        if (attributeComparison != 0)
        {
            return attributeComparison;
        }

        // 年龄比较，年龄更小的优先
        return a.getAge().CompareTo(b.getAge());
    }


    /// <summary>
    ///     更新政策进度
    /// </summary>
    /// <param name="pElapsed"></param>
    public void UpdateForPolicy(float pElapsed)
    {
        // 当目前政策都执行完毕或没有政策时，查找新的政策
        if (policy_data.p_status == KingdomPolicyData.PolicyStatus.Completed ||
            string.IsNullOrEmpty(policy_data.current_policy_id))
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
                if (policy_data.policy_queue.Count >= PolicyConst.MAX_POLICY_NR_IN_QUEUE) break;
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
                   && (policy_data.p_status == KingdomPolicyData.PolicyStatus.Completed ||
                       policy_data.current_policy_id != pPolicyAsset.id))
               && (!pPolicyAsset.only_moh || MoHTools.IsMoHKingdom(this))
               && (pPolicyAsset.all_prepositions == null ||
                   pPolicyAsset.pre_state_require_type == KingdomPolicyAsset.PreStateRequireType.All &&
                   pPolicyAsset.all_prepositions.All(pState => policy_data.current_states.ContainsValue(pState)) ||
                   pPolicyAsset.pre_state_require_type == KingdomPolicyAsset.PreStateRequireType.Any &&
                   pPolicyAsset.all_prepositions.Any(pState => policy_data.current_states.ContainsValue(pState)))
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
        if (!string.IsNullOrEmpty(policy_data.current_policy_id) &&
            policy_data.p_status != KingdomPolicyData.PolicyStatus.Completed)
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
    //检查是否有相同政策

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
            return LM.Get("year_name_format").Replace("$year_name$", text).Replace("$year_number$",
                num == 1 ? LM.Get("first_year_number") : num.ToString());
        }

        return text;
    }

    public static void SetNewCapital(AW_Kingdom kingdom)
    {
        MoHTools.IsMoHKingdom(kingdom);

        if (kingdom.capital != null)
        {
            City newCapital = FindNewCapital(kingdom);
            if (newCapital != null)
            {
                kingdom.capital = newCapital;
                kingdom.king.ChangeCity(newCapital);
                //Debug.Log("New capital set to " + newCapital.data.name);
            }
        }
    }

    public static City FindNewCapital(AW_Kingdom kingdom)
    {
        if (kingdom.capital == null || kingdom.cities == null || kingdom.cities.Count == 0)
        {
            return null;
        }

        // 获取得分最高的城市，数量取决于实际的城市数目，但不超过5个
        var topCities = kingdom.cities
            .Select(city =>
            {
                double score = (city.getAge() - kingdom.capital.getAge()) +
                               (city.getPopulationTotal() - kingdom.capital.getPopulationTotal()) * 2 +
                               (city.zones.Count - kingdom.capital.zones.Count) * 0.35 +
                               (city.neighbours_cities.SetEquals(city.neighbours_cities_kingdom) ? 50 : 0);

                // 计算该城市与其他城市的总距离，并将其反转作为额外得分
                double distanceScore = kingdom.cities
                    .Where(c => c != city)
                    .Sum(c => Toolbox.DistVec3(city.cityCenter, c.cityCenter));
                // 通过一个因子调整距离得分的影响力，可以根据需要调整
                distanceScore = 1 / (1 + distanceScore);

                return new { City = city, Score = score + distanceScore };
            })
            .OrderByDescending(cityScore => cityScore.Score)
            .Select(cityScore => cityScore.City)
            .FirstOrDefault();

        return topCities;
    }
    public void CheckAndSetPrimaryKingdom(Actor actor, AW_Kingdom kingdomToInherit)
    {
        // 检查Actor是否是国王
        if (actor.isKing() && kingdomToInherit != actor.kingdom)
        {
            // 获取Actor当前的王国
            AW_Kingdom currentKingdom = actor.kingdom as AW_Kingdom;

            // 获取即将继承的王国


            // 确保即将继承的王国存在且不是当前的王国
            if (kingdomToInherit != null && kingdomToInherit != currentKingdom)
            {
                // 计算两个王国的价值
                int currentValue = MoHTools.CalculateKingdomValue(currentKingdom);
                int newValue = MoHTools.CalculateKingdomValue(kingdomToInherit);

                // 比较价值，确定哪个王国更强
                if (newValue > currentValue)
                {
                    MergeKingdoms(kingdomToInherit, currentKingdom);
                    kingdomToInherit.setKing(actor); 
                    CityTools.LogKingIntegration(actor,currentKingdom,kingdomToInherit);//加一个worldlogmessage
                }
                else
                {
                    MergeKingdoms(currentKingdom, kingdomToInherit);
                    CityTools.LogKingIntegration(actor,currentKingdom,kingdomToInherit);
                }
            }
        }
    }

    private void MergeKingdoms(AW_Kingdom strongerKingdom, AW_Kingdom weakerKingdom)
    {
        // 创建一个列表来存储需要转移的城市
        List<City> citiesToTransfer = new List<City>();

        // 先收集所有需要转移的城市
        foreach (var city in weakerKingdom.cities)
        {
            citiesToTransfer.Add(city);
        }

        // 然后将这些城市加入到强国王国
        foreach (var city in citiesToTransfer)
        {
            city.joinAnotherKingdom(strongerKingdom);
        }
    }


    /// <summary>
    ///     国王即位相关行为
    /// </summary>
    /// <param name="pKing"></param>
    [MethodReplace(nameof(setKing))]
    public new void setKing(Actor pActor) //处理联统 并放出世界消息
    {
        #region 原版代码

        king = pActor;
        CheckAndSetPrimaryKingdom(king, this);

        if (king.city != capital && capital != null)
        {
            king.ChangeCity(capital);
        }
        king.setProfession(UnitProfession.King);
        data.kingID = king.data.id;
        data.timestamp_king_rule = World.world.getCurWorldTime();
        trySetRoyalClan();

        #endregion

        MoHCorePatch.check_and_add_moh_trait(this, pActor);
        clearHeirData();
        KingdomYearName.changeYearname(this);

    }
}