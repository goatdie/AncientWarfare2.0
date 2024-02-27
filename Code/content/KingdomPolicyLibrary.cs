using Figurebox.abstracts;
using Figurebox.core;
using Figurebox.core.kingdom_policies;
using Figurebox.policy_actions;

namespace Figurebox.content;

class KingdomPolicyLibrary : AW_AssetLibrary<KingdomPolicyAsset, KingdomPolicyLibrary>
{
    /// <summary>
    /// 开启奴隶制
    /// </summary>
    public static readonly KingdomPolicyAsset start_slaves;
    /// <summary>
    /// 强化奴隶控制
    /// </summary>
    public static readonly KingdomPolicyAsset control_slaves;
    /// <summary>
    /// 奴隶军
    /// </summary>
    public static readonly KingdomPolicyAsset slaves_army;
    /// <summary>
    /// 姓氏合流
    /// </summary>
    public static readonly KingdomPolicyAsset name_integration;
#if 一米_中文名
    /// <summary>
    /// 年号
    /// </summary>
    public static readonly KingdomPolicyAsset kingdom_yearname;
#endif
    /// <summary>
    /// 迁都
    /// </summary>
    public static readonly KingdomPolicyAsset change_capital;
    /// <summary>
    /// 升衔
    /// </summary>
    public static readonly KingdomPolicyAsset title_upgrade;
    /// <summary>
    /// 开启半奴隶半封建
    /// </summary>
    public static readonly KingdomPolicyAsset start_halfaristocrat;
    /// <summary>
    /// 分封雏形
    /// </summary>
    public static readonly KingdomPolicyAsset base_enfeoffment;
    /// <summary>
    /// 推恩令
    /// </summary>
    public static readonly KingdomPolicyAsset favor_order;
    /// <summary>
    /// 长久分封
    /// </summary>
    public static readonly KingdomPolicyAsset continuous_enfeoffment;
    // 这里可以根据需要添加特定于 KingdomPolicyAsset 的方法
    public override void init()
    {
        base.init();
        // 只注册政策, 不要链接状态
        add(nameof(start_slaves), "ui/policy/start_slaves", 100, 100, null,
            MajorPolicyExecuteActions.StartSlaves, false, false);
        add(nameof(control_slaves), "ui/icons/iconDamage", 100, 100, null,
            MajorPolicyExecuteActions.EnforceSlavesControl, true, false);
        add(nameof(slaves_army), "ui/icons/iconDamage", 100, 100, null,
            MajorPolicyExecuteActions.EnableSlavesArmy, false, false);
        add(nameof(name_integration), "ui/icons/iconDamage", 100, 100, null,
            MajorPolicyExecuteActions.NameIntegration, false, false);
#if 一米_中文名
        add(nameof(kingdom_yearname), "ui/policy/change_name", 1, 1, null,
            MajorPolicyExecuteActions.MakeNewYearName, true, false);
#endif
        add(nameof(change_capital), "ui/policy/move_capital", 100, 100, InPeace,
            MajorPolicyExecuteActions.ChangeCapital, true, false);
        add(nameof(title_upgrade), "ui/policy/change_name", 100, 100, CanBePromoted,
            MajorPolicyExecuteActions.UpgradeKingdomTitle, true, false);
        add(nameof(start_halfaristocrat), "ui/policy/start_halfaristocrat", 100, 100, null,
            MajorPolicyExecuteActions.StartHalfAristocrat, false, false);

        add(nameof(base_enfeoffment), "ui/policy/base_enfeoffment", 100, 100, null, null, false, false);
        add(nameof(favor_order), "ui/policy/favor_order", 100, 100, null, null, false, false);
        add(nameof(continuous_enfeoffment), "ui/policy/continuous_enfeoffment", 100, 100, null, null, false, false);
    }

    public override void post_init()
    {
        base.post_init();
        start_slaves.AddPreposition(KingdomPolicyStateLibrary.DefaultState);
        start_slaves.SetTargetState(KingdomPolicyStateLibrary.slaveowner);

        control_slaves.AddPreposition(KingdomPolicyStateLibrary.slaveowner);

        slaves_army.AddPreposition(KingdomPolicyStateLibrary.slaveowner);
        slaves_army.SetTargetState(KingdomPolicyStateLibrary.slave_soldier);

        start_halfaristocrat.AddPreposition(KingdomPolicyStateLibrary.slaveowner);
        start_halfaristocrat.AddPreposition(KingdomPolicyStateLibrary.slave_soldier);
        start_halfaristocrat.SetTargetState(KingdomPolicyStateLibrary.halfaristocrat);

        name_integration.AddPreposition(KingdomPolicyStateLibrary.halfaristocrat);
        name_integration.SetTargetState(KingdomPolicyStateLibrary.name_integration);


        base_enfeoffment.AddPreposition(KingdomPolicyStateLibrary.halfaristocrat);
        base_enfeoffment.AddPreposition(KingdomPolicyStateLibrary.name_integration);
        base_enfeoffment.SetTargetState(KingdomPolicyStateLibrary.enfeoffment_base);

        favor_order.AddPreposition(KingdomPolicyStateLibrary.enfeoffment_base);
        favor_order.AddTechRequire(CityTechLibrary.enfeoffment_power_analysis);
        favor_order.SetTargetState(KingdomPolicyStateLibrary.enfeoffment_limit);

        continuous_enfeoffment.AddPreposition(KingdomPolicyStateLibrary.enfeoffment_base);
        continuous_enfeoffment.AddTechRequire(CityTechLibrary.enfeoffment_range_analysis);
        continuous_enfeoffment.SetTargetState(KingdomPolicyStateLibrary.enfeoffment_unlimit);
    }

    public override void linkAssets()
    {
        base.linkAssets();
    }

    public KingdomPolicyAsset add(
        string pID, string pPathIcon,
        int pPlanCost, int pProgressCost,
        CheckPolicy pCheckAction, ExecutePolicy pExecuteAction,
        bool pCanRepeat, bool pOnlyMoH
    )
    {
        return add(new KingdomPolicyAsset
        {
            id = pID,
            policyname = pID,
            description = pID + "_desc",
            path_icon = pPathIcon,
            cost_in_plan = pPlanCost,
            cost_in_progress = pProgressCost,
            check_policy = pCheckAction,
            execute_policy = pExecuteAction,
            can_repeat = pCanRepeat,
            only_moh = pOnlyMoH
        });
    }

    // 例如，执行所有政策的方法
    public void ExecuteAllPolicies(Kingdom kingdom, KingdomPolicyStateAsset state)
    {
    }

    // 根据ID加载政策资产的方法
    public void LoadPolicyAssetsFromFile(string filePath)
    {
        // 从文件中加载政策资产的逻辑
        // 假设有一个方法从文件中读取政策数据，并转换为 KingdomPolicyAsset 对象
    }

    // 根据ID保存政策资产的方法
    public void SavePolicyAssetsToFile(string filePath)
    {
        // 将政策资产保存到文件的逻辑
        // 假设有一个方法将 KingdomPolicyAsset 对象转换为适合保存的数据格式
    }

    public static bool InPeace(KingdomPolicyAsset policy, AW_Kingdom kingdom)
    {
        // 检查 kingdom 是否存在
        if (kingdom == null)
        {
            return false;
        }
        if (kingdom.Rebel)
        {
            return true;//起义军可以无视和平直接迁都
        }
        // 检查国家是否有任何正在进行的战争
        var isAtWar = kingdom.getWars().Any();

        // 如果国家没有参与任何战争，则返回 true，表示国家处于和平状态
        return !isAtWar;
    }

    public static bool CanBePromoted(KingdomPolicyAsset policy, AW_Kingdom kingdom)
    {
        int zoneCount = kingdom.countZones();
        // 如果是宗主国，将附庸国的疆土也考虑进去，但以较低的权重计算
        if (kingdom.IsSuzerain())
        {
            foreach (var vassal in kingdom.GetVassals())
            {
                // 假设附庸国的疆土权重为0.6
                zoneCount += (int)(vassal.countZones() * 0.65);
            }
        }

        switch (kingdom.addition_data.Title)
        {
            case AW_KingdomDataAddition.KingdomTitle.Baron:
                // 伯国升级到侯国的条件
                return zoneCount > 300;

            case AW_KingdomDataAddition.KingdomTitle.Marquis:
                // 侯国升级到公国的条件
                return zoneCount > 800;

            case AW_KingdomDataAddition.KingdomTitle.Duke:
                // 公国升级到王国的条件
                return zoneCount > 1300;

            case AW_KingdomDataAddition.KingdomTitle.King:
                // 王国升级到帝国的条件
                return zoneCount > 2000;

            // 对于帝国级别，可能没有进一步的升级，或者可以根据需要添加逻辑
            case AW_KingdomDataAddition.KingdomTitle.Emperor:
                return false;

            default:
                return false;
        }
    }
}