using System;
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
    ///     消耗天命, 返回是否成功
    /// </summary>
    /// <param name="pValue">消耗量</param>
    /// <param name="pForce">强制消耗, 无视下限</param>
    /// <returns>天命国家不存在时返回失败</returns>
    internal static bool CostMoH(int pValue, bool pForce = false)
    {
        // 帮我写一下这个函数
        throw new NotImplementedException();
    }
    /// <summary>
    ///     恢复的天命(取消国策时会使用)
    /// </summary>
    /// <param name="pValue">恢复数额</param>
    /// <return>天命国家不存在时返回false</return>
    internal static bool ReturnMoH(int pValue)
    {
        // 帮我写一下这个函数
        throw new NotImplementedException();
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