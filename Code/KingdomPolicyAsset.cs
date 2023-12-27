using System;
using System.Collections.Generic;
using NeoModLoader.General;
using UnityEngine;

namespace Figurebox;



delegate void ExecutePolicy(KingdomPolicyAsset policy, Kingdom kingdom, KingdomPolicyStateAsset state);

class KingdomPolicyStateAsset : Asset
{
    /// <summary>
    /// 所有当前状态类型可选的政策
    /// </summary>
    public HashSet<string> all_optional_policies;
}


class KingdomPolicyAsset : Asset
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
    /// 执行政策的相关逻辑行为
    /// </summary>
    public ExecutePolicy execute_policy;
    public int cost = 100; //进度 //进度我认为不应该放在这里， 应该单独起一个PolicyData的玩意，是应该作为每个国家独特的数据
                                        // 在这里的应该是一个共同的信息, 可能更合适的是耗时之类的概念
    public string path_icon;
    /// <summary>
    /// 描述文本的key
    /// </summary>
    public string description;
    /// <summary>
    /// 政策名称的key
    /// </summary>
    public string policyname;
    public Kingdom special_kingdom; //给特定国家的特殊国策
}


class KingdomPolicyGraphAsset : Asset
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
