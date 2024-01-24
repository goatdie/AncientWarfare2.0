using System;
using UnityEngine;
using System.Collections.Generic;
using Figurebox.constants;
using Figurebox.attributes;
using NeoModLoader.api.attributes;
using Figurebox.core;
namespace Figurebox.Utils.MoH;

public class MoHTools
{
    public const int MOH_UnderLimit = -30;
    public const int MOH_UpperLimit = 100;
    public static readonly List<City> _moh_cities = new List<City>();
    public static double timestamp_moh_collapse;

    /// <summary>
    ///     当前天命国家
    /// </summary>
    public static AW_Kingdom MoHKingdom { get; private set; }
    /// <summary>
    ///     当前天命国家是否存在
    /// </summary>
    public static bool ExistMoHKingdom => MoHKingdom != null && MoHKingdom.isAlive();
    public static int MOH_Value { get; private set; }
    /// <summary>
    ///     设置当前天命国家
    /// </summary>
    /// <param name="kingdom"></param>
    public static void SetMoHKingdom(AW_Kingdom kingdom)
    {
        MoHKingdom = kingdom;
        kingdom.SetTitle(KingdomPolicyData.KingdomTitle.Emperor);
        MOH_Value = 30;
        if (kingdom.king != null) { kingdom.king.addTrait("天命"); }
        Debug.Log("天命值" + MOH_Value);
    }
    public static AW_Kingdom ConvertKtoAW(Kingdom kingdom)
    {
        AW_Kingdom awk = kingdom as AW_Kingdom;
        return awk;
    }
    public static void Clear_MoHKingdom()
    {
        if (MoHKingdom != null)
        {
            MoHKingdom.FomerMoh = true;
            CityTools.loglosekingdom(MoHKingdom);

        }
        MoHKingdom = null;
        MOH_Value = 0;
        timestamp_moh_collapse = World.world.getCurWorldTime();
    }
    public static void SetMOH_Value(int value)
    {
        MOH_Value = value;
    }
    public static void ChangeMOH_Value(int value) //天命值改变
    {
        MOH_Value += value;
        Main.LogInfo("天命改变" + value);
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
        if (!ExistMoHKingdom)
        {
            return false;
        }
        if (pForce)
        {
            ChangeMOH_Value(-pValue);
            return true;
        }
        if (MOH_Value >= MOH_UnderLimit)
        {
            ChangeMOH_Value(-pValue);
            return true;
        }
        return false;
    }
    /// <summary>
    ///     恢复的天命(取消国策时会使用)
    /// </summary>
    /// <param name="pValue">恢复数额</param>
    /// <return>天命国家不存在时返回false</return>
    internal static bool ReturnMoH(int pValue)
    {
        if (!ExistMoHKingdom)
        {
            return false;
        }
        ChangeMOH_Value(+pValue);
        return true;
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
        switch (MOH_Value)
        {
            case <= 0:
                return "moh_desc_bad";
            case <= 70:
                return "moh_desc_common";
            default:
                return "moh_desc_good";
        }
    }
    /// <summary>
    ///     超低天命，天命大爆炸 遍地义军
    /// </summary>
    /// <returns>天命国家不存在时返回失败</returns>
    public static void MoHKingdomBoom()
    {
        if (!ExistMoHKingdom) return;
        if (MoHKingdom.king != null && MoHKingdom.king.hasTrait("first")) return;
        if (MoHKingdom.king != null)
        {
            MoHKingdom.king.removeTrait("天命");
            MoHKingdom.king.addTrait("formerking");
        }
        _moh_cities.AddRange(MoHKingdom.cities);

        List<City> cityListCopy = new List<City>(MoHKingdom.cities);
        foreach (City city in cityListCopy)
        {
            PlotAsset rebellionPlot = AssetManager.plots_library.get("rebellion");
            if (city != MoHKingdom.capital && city.leader != null &&
                !city.neighbours_cities.Contains(MoHKingdom.capital))
            {
                Plot pPlot = World.world.plots.newPlot(city.leader, rebellionPlot);

                pPlot.initiator_city = city;
                pPlot._leader = city.leader;
                pPlot.initiator_actor = city.leader;
                pPlot.initiator_kingdom = MoHKingdom;
                pPlot.target_city = city;

                rebellionPlot.action(pPlot);
                WorldLog.logCityRevolt(city);
            }
        }

        Clear_MoHKingdom();

    }
    [Hotfixable]
    [MethodReplace(typeof(DiplomacyHelpers), nameof(DiplomacyHelpers.getWarTarget))]
    public static Kingdom getWarTarget(Kingdom pInitiatorKingdom)
    {
        // 当Rebel标志为true时，更改战争目标的选择逻辑
        if (ConvertKtoAW(pInitiatorKingdom).Rebel && World.world.mapStats.getYearsSince(timestamp_moh_collapse) <= 250)
        {
            // 遍历 _moh_cities 中的城市
            foreach (City mohCity in _moh_cities)
            {
                // 获取当前城市的邻近国家
                HashSet<Kingdom> neighbourKingdoms = mohCity.neighbours_kingdoms;

                // 检查邻近国家的城市列表是否包含 _moh_cities 中的城市
                foreach (Kingdom pkingdom in neighbourKingdoms)
                {
                    if (pkingdom.cities.Contains(mohCity))
                    {
                        // 如果找到交集，返回这个国家作为战争目标
                        return pkingdom;
                    }
                }
            }
            return null;
        }


        // 原有的选择逻辑
        Kingdom kingdom = null;
        float num = 0f;
        List<Kingdom> neutralKingdoms = DiplomacyHelpers.wars.getNeutralKingdoms(pInitiatorKingdom, false, false);
        int num2 = pInitiatorKingdom.getArmy();
        if (pInitiatorKingdom.hasAlliance())
        {
            num2 = pInitiatorKingdom.getAlliance().countWarriors();
        }
        foreach (Kingdom item in neutralKingdoms)
        {
            if (item.cities.Count != 0 && item.capital != null && item.getAge() >= SimGlobals.m.minimum_kingdom_age_for_attack)
            {
                int num3;
                if (item.hasAlliance())
                {
                    num3 = item.getAlliance().countWarriors();
                }
                else
                {
                    num3 = item.getArmy();
                }
                if (num2 >= num3 && pInitiatorKingdom.capital.reachableFrom(item.capital))
                {
                    DiplomacyRelation relation = DiplomacyHelpers.diplomacy.getRelation(pInitiatorKingdom, item);
                    if ((float)World.world.getYearsSince(relation.data.timestamp_last_war_ended) >= (float)SimGlobals.m.minimum_years_between_wars && !pInitiatorKingdom.isOpinionTowardsKingdomGood(item))
                    {
                        float num4 = Kingdom.distanceBetweenKingdom(pInitiatorKingdom, item);
                        if (kingdom == null || num4 < num)
                        {
                            num = num4;
                            kingdom = item;
                        }
                    }
                }
            }
        }
        return kingdom;
    }


}