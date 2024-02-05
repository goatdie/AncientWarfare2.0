using Figurebox.Utils.extensions;
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
            MoHTools.MoHKingdomBoom(); //天命爆炸
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
            value_change++;

        }
        if (MoHTools.MoHKingdom.hasEnemies())
        {
            value_change--;

        }
        if (!MoHTools.MoHKingdom.isSupreme())
        {
            value_change -= 2;

        }
        if (World.world_era == AssetManager.era_library.get(S.age_hope) || World.world_era == AssetManager.era_library.get(S.age_wonders))
        {
            value_change += 2; // 正面纪元
        }
        if (World.world_era == AssetManager.era_library.get(S.age_despair) || World.world_era == AssetManager.era_library.get(S.age_ash) || World.world_era == AssetManager.era_library.get(S.age_chaos))
        {
            value_change -= 20; // 负面纪元

        }
        if (MoHTools.MoHKingdom.king != null)
        {
            if (MoHTools.MoHKingdom.king.hasTrait("first"))
            {
                value_change += 5;

            }
            if (MoHTools.MoHKingdom.king.getAge() <= 24)
            {
                value_change--;

            }
            if (MoHTools.MoHKingdom.king.data.intelligence <= 5)
            {
                value_change--;

            }
        }

        Clan kclan = BehaviourActionBase<Kingdom>.world.clans.get(MoHTools.MoHKingdom.data.royal_clan_id);
        if (kclan != null && kclan.units.Count <= 2)
        {
            value_change--;

        }

        MoHTools.ChangeMOH_Value(value_change);


    LIMIT_MOH_VALUE:

        if (MoHTools.MOH_Value >= MoHTools.MOH_UpperLimit)
        {
            MoHTools.SetMOH_Value(MoHTools.MOH_UpperLimit);

        }


    }

    public bool CheckNoMoreRebels()
    {
        foreach (Kingdom k in this.list_civs)
        {
            AW_Kingdom awKingdom = k.AW();
            if (awKingdom.Rebel)
            {
                return false; // 存在反叛国家
            }
        }
        return true; // 没有反叛国家
    }

    public void CheckDeclareEmpire()
    {
        if (MoHTools.ExistMoHKingdom) { return; }
        // 首先检查是否有任何国家是Rebel
        bool noRebels = CheckNoMoreRebels();

        foreach (Kingdom k in this.list_civs)
        {
            AW_Kingdom awKingdom = k.AW();

            // 如果国家是前MOH或Rebel或既不是前MOH也不是Rebel
            if ((awKingdom.FomerMoh && noRebels) || awKingdom.Rebel || (!awKingdom.FomerMoh && !awKingdom.Rebel))
            {
                if (CanDeclareEmpire(awKingdom) && MoHTools.Ismostpowerfulkingdom(awKingdom))
                {
                    MoHTools.SetMoHKingdom(awKingdom);
                    awKingdom.FomerMoh = false;
                    awKingdom.Rebel = false;
                    if (k.king != null)
                    {
                        k.king.addTrait("first");

                        KingdomYearName.changeYearname(awKingdom);

                    }
                }
            }
        }
    }


    public bool CanDeclareEmpire(AW_Kingdom kingodm)
    {
        int totalOriginalCities = MoHTools._moh_cities.Count;
        int rebelControlledCities = 0;

        // 计算起义军控制的城市数量
        foreach (City city in kingodm.cities)
        {
            if (MoHTools._moh_cities.Contains(city))
            {
                rebelControlledCities++;
            }
        }

        // 计算控制的城市百分比
        float controlPercentage = (float)rebelControlledCities / totalOriginalCities;

        // 如果控制的城市百分比 >= 70%，则可以称帝
        Main.LogInfo(kingodm.name + rebelControlledCities + "total:" + totalOriginalCities + "city数值" + controlPercentage);
        return controlPercentage >= 0.65;
    }
}