using System.Collections.Generic;

namespace Figurebox;



delegate void ExecutePolicy(KingdomPolicyAsset policy, Kingdom kingdom, KingdomPolicyStateAsset state);

class KingdomPolicyStateAsset : Asset
{
    public HashSet<KingdomPolicyAsset> all_optional_policies; // 所有可选政策
    public string[] all_optional_policies_ids; // 所有可选政策id，保存时使用的格式，为了更容易实现和维护在不同的游戏存档
}


class KingdomPolicyAsset : Asset
{
    public HashSet<KingdomPolicyAsset> all_prepositions; // 所有前置政策
    public string[] all_prepositions_ids; // 所有前置政策id，保存时使用的格式，为了更容易实现和维护在不同的游戏存档
    public ExecutePolicy execute_policy; // 执行政策的相关逻辑行为
    public int Progress = 100; //进度
    public string path_icon;
    public string description;
    public string policyname;
    public HashSet<KingdomPolicyAsset> branches; //分支政策
}


class KingdomPolicyGraphAsset : Asset
{
    public HashSet<KingdomPolicyAsset> all_policies; // 所有政策
    public HashSet<KingdomPolicyStateAsset> all_states; // 所有状态
}

// 三个类继承自library
