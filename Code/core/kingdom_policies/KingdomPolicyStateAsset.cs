using System.Collections.Generic;
namespace Figurebox;

public class KingdomPolicyStateAsset : Asset
{
    /// <summary>
    ///     所有当前状态类型可选的政策
    /// </summary>
    public HashSet<string> all_optional_policies;
    /// <summary>
    ///     国家实力计算方法, 不同政体应当有不同的计算方法
    /// </summary>
    public CalcKingdomStrength calc_kingdom_strength = KingdomPolicyStateLibrary.DefaultCalcKingdomPower;
    /// <summary>
    ///     查找下一个政策的方法
    /// </summary>
    public FindPolicy policy_finder;
    /// <summary>
    ///     状态类型. 如:"地方组织形式", "军队组织形式"等
    /// </summary>
    public string type = "";
    public void AddOptionalPolicy(string pPolicyID)
    {
        all_optional_policies ??= new HashSet<string>();
        all_optional_policies.Add(pPolicyID);
    }
}