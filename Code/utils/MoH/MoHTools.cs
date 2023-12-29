using System.Collections.Generic;
using Figurebox.constants;
using Figurebox.core;
namespace Figurebox.Utils.MoH;

public class MoHTools
{
    /// <summary>
    ///     当前天命国家
    /// </summary>
    public static AW_Kingdom MoHKingdom { get; private set; }
    /// <summary>
    ///     当前天命国家是否存在
    /// </summary>
    public static bool ExistMoHKingdom => MoHKingdom != null && MoHKingdom.isAlive();
    /// <summary>
    ///     设置当前天命国家
    /// </summary>
    /// <param name="kingdom"></param>
    public static void SetMoHKingdom(AW_Kingdom kingdom)
    {
        MoHKingdom = kingdom;
    }
    public static bool IsMoHKingdom(AW_Kingdom kingdom)
    {
        return MoHKingdom == kingdom;
    }
    public static AW_Kingdom FindMostPowerfulKingdom(List<AW_Kingdom> kingdoms)
    {
        if (kingdoms == null || kingdoms.Count == 0)
        {
            return null; // 如果没有王国或王国列表为空，则返回null
        }

        AW_Kingdom mostPowerfulKingdom = null;
        int highestValue = 0;

        foreach (var kingdom in kingdoms)
        {
            int kingdomValue = CalculateKingdomValue(kingdom);
            if (kingdomValue > highestValue)
            {
                highestValue = kingdomValue;
                mostPowerfulKingdom = kingdom;
            }
        }

        return mostPowerfulKingdom;
    }

    public static int CalculateKingdomValue(AW_Kingdom k)
    {
        string social_level_state_id = k.policy_data.GetPolicyStateId(PolicyStateType.social_level);
        if (string.IsNullOrEmpty(social_level_state_id))
        {
            return (int)KingdomPolicyStateLibrary.DefaultState.calc_kingdom_strength(k);
        }
        return (int)(KingdomPolicyStateLibrary.Instance.get(social_level_state_id)?.calc_kingdom_strength(k) ?? 0);
    }
    /// <summary>
    ///     从存档中读取并直接缓存国家
    /// </summary>
    internal static void LoadFromSave()
    {

    }
    public static string GetMoHDescKey()
    {
        if (!ExistMoHKingdom) return "";
        return "moh_desc_common";
    }
}