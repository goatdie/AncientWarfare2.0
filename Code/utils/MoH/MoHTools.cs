using Figurebox.core;
using System.Collections.Generic;
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
    public static int MOH_Value = 0;
    public static void SetMoHKingdom(AW_Kingdom kingdom)
    {
        MoHKingdom = kingdom;
        MOH_Value = 30;
    }
        public static void Clear_MoHKingdom()
    {
        MoHKingdom = null;
        MOH_Value = 0;
    }
    public static void SetMOH_Value(int value)
    {
        MOH_Value = value;
    }
        public static void ChangeMOH_Value(int value) //天命值改变
    {
        MOH_Value += value;
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
        int kingdomValue = 0;
        int populationTotal = k.getPopulationTotal();
        int cityCount = k.cities.Count * 100;
        int armySize = k.getArmy();
        int stewardship = 8;
        if (k.king != null)
        {
            stewardship = k.king.GetData().stewardship * 10;


        }

        if (k.policy_data.current_state_id == "default")
        {
            kingdomValue = populationTotal + cityCount + armySize + stewardship;
        }

        return kingdomValue;
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