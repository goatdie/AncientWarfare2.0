using Figurebox.Utils.MoH;
namespace Figurebox.core;

public partial class AW_KingdomManager
{
    /// <summary>
    ///     根据天命值决定天命事件
    /// </summary>
    public static void UpdateMoHCondition()
    {
        if (!MoHTools.ExistMoHKingdom) return;

        if (MoHTools.MOH_Value >= 40)
        {
            //天命值在什么条件发生的事件
        }
        if (MoHTools.MOH_Value <= MoHTools.MOH_UnderLimit)
        {
            MoHTools.Clear_MoHKingdom(); //天命去除
        }
    }
    /// <summary>
    ///     根据天命国家的情况更新天命值
    /// </summary>
    public static void UpdateMoHValue()
    {
        if (!MoHTools.ExistMoHKingdom) goto LIMIT_MOH_VALUE;

        int value_change = 0;
        if (MoHTools.MoHKingdom.getEnemiesKingdoms().Count == 0)
        {
            // 如果没有敌国, 天命累加值+1
            value_change++;
        }

        if (MoHTools.MoHKingdom.king != null)
        {

            if (MoHTools.MoHKingdom.king.hasTrait("first"))
            {
                // 如果国王有"天子"特质, 天命累加值+3
                value_change += 3;
            }
            if (MoHTools.MoHKingdom.king.getAge() <= 24)
            {
                // 国王过于年轻, 天命累加值-1
                value_change--;
            }

        }
        Clan kclan = BehaviourActionBase<Kingdom>.world.clans.get(MoHTools.MoHKingdom.data.royal_clan_id);
        if (kclan != null && kclan.units.Count <= 2)
        {
            //如果王室家族人数小于等于2, 天命累加值-1
            value_change--;
        }
        MoHTools.ChangeMOH_Value(value_change);

        LIMIT_MOH_VALUE:

        if (MoHTools.MOH_Value >= MoHTools.MOH_UpperLimit)
        {
            MoHTools.SetMOH_Value(MoHTools.MOH_UpperLimit);
        }
    }
}