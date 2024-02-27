using Figurebox.abstracts;
using Figurebox.constants;
using Figurebox.core;
using Figurebox.core.kingdom_policies;
using NeoModLoader.api.attributes;
using System.Collections.Generic;
using System.Reflection;

namespace Figurebox.content;

/// <summary>
///     政治状态（政体）
/// </summary>
public class KingdomPolicyStateLibrary : AW_AssetLibrary<KingdomPolicyStateAsset, KingdomPolicyStateLibrary>
{
    /// <summary>
    ///     社会统治阶级：奴隶主
    /// </summary>
    public static readonly KingdomPolicyStateAsset slaveowner;
    /// <summary>
    ///     社会统治阶级：半奴隶主半封建贵族
    /// </summary>
    public static readonly KingdomPolicyStateAsset halfaristocrat;

    /// <summary>
    ///     军队主体：奴隶
    /// </summary>
    public static readonly KingdomPolicyStateAsset slave_soldier;
    /// <summary>
    ///     杂项: 姓氏合流
    /// </summary>
    public static readonly KingdomPolicyStateAsset name_integration;
    /// <summary>
    /// 分封形式: 基础分封
    /// </summary>
    public static readonly KingdomPolicyStateAsset enfeoffment_base;
    /// <summary>
    /// 分封形式: 有限分封(推恩)
    /// </summary>
    public static readonly KingdomPolicyStateAsset enfeoffment_limit;
    /// <summary>
    /// 分封形式: 无限分封(类似西方封建)
    /// </summary>
    public static readonly KingdomPolicyStateAsset enfeoffment_unlimit;

    public static KingdomPolicyStateLibrary Instance { get; } = new();

    /// <summary>
    ///     默认政体(政治状态)
    /// </summary>
    public static KingdomPolicyStateAsset DefaultState { get; private set; }

    public override void init()
    {
        base.init();
        // 只注册状态, 不要链接政策
        // 原始公平
        DefaultState = add("default", PolicyStateType.social_level);
        // 奴隶制
        add(PolicyState.slaveowner, PolicyStateType.social_level);
        // 半奴隶制半封建
        add(PolicyState.halfaristocrat, PolicyStateType.social_level);
        // 封建贵族
        add(PolicyState.aristocrat, PolicyStateType.social_level);
        // 地主
        add(PolicyState.landlord, PolicyStateType.social_level);
        // 资本家
        add(PolicyState.capitalist, PolicyStateType.social_level);
        // 无产阶级
        add(PolicyState.proletarian, PolicyStateType.social_level);

        add(PolicyState.slave_soldier, PolicyStateType.army_main_soldiers);
        add(PolicyState.name_integration, PolicyStateType.name_organization);

        add(PolicyState.enfeoffment_base, PolicyStateType.enfeoffment_type);
        add(PolicyState.enfeoffment_limit, PolicyStateType.enfeoffment_type);
        add(PolicyState.enfeoffment_unlimit, PolicyStateType.enfeoffment_type);
    }

    public override void post_init()
    {
        // 添加可选政策
        DefaultState.AddOptionalPolicy(KingdomPolicyLibrary.start_slaves);

        slaveowner.AddOptionalPolicy(
            KingdomPolicyLibrary.control_slaves,
            KingdomPolicyLibrary.slaves_army,
           KingdomPolicyLibrary.start_halfaristocrat
        );
        slaveowner.AddCityTasks(
            AssetManager.tasks_city.get("check_slave_job"),
            AssetManager.tasks_city.get("produce_slaves")
        );
        halfaristocrat.AddOptionalPolicy(
            KingdomPolicyLibrary.name_integration
        );

        slave_soldier.AddCityTasks(
            AssetManager.tasks_city.get("check_slave_army")
        );
        halfaristocrat.AddCityTasks(
           AssetManager.tasks_city.get("check_slave_job"),
           AssetManager.tasks_city.get("produce_slaves")
        );

        enfeoffment_base.AddOptionalPolicy(
            KingdomPolicyLibrary.favor_order,
            KingdomPolicyLibrary.continuous_enfeoffment
        );
    }

    public override void linkAssets()
    {
        base.linkAssets();
    }

    public KingdomPolicyStateAsset add(string pID, string pType, string pPathIcon = null,
        FindPolicy pPolicyFinder = null,
        CalcKingdomStrength pCalcKingdomPower = null)
    {
        return add(new KingdomPolicyStateAsset
        {
            id = pID,
            type = pType,
            name = pID,
            description = pID + "_desc",
            path_icon = string.IsNullOrEmpty(pPathIcon) ? "ui/icons/iconDamage" : pPathIcon,
            policy_finder = pPolicyFinder ?? DefaultPolicyFinder,
            calc_kingdom_strength = pCalcKingdomPower ?? DefaultCalcKingdomPower
        });
    }

    public static float DefaultCalcKingdomPower(AW_Kingdom pKingdom)
    {
        int kingdomValue = 0;
        int populationTotal = pKingdom.getPopulationTotal();
        int cityCount = pKingdom.cities.Count * 100;
        int armySize = pKingdom.getArmy();
        int stewardship = 8;
        if (pKingdom.king != null)
        {
            stewardship = pKingdom.king.data.stewardship * 10;
        }

        return populationTotal + cityCount + armySize + stewardship;
    }

    [Hotfixable]
    public static KingdomPolicyAsset DefaultPolicyFinder(KingdomPolicyStateAsset pState, AW_Kingdom pKingdom)
    {
        return pState.all_optional_policies.Count == 0
            ? null
            : KingdomPolicyLibrary.Instance.get(pState.all_optional_policies.GetRandom());
    }
}