using Figurebox.core;
using Figurebox.core.kingdom_policies;
using Figurebox.policy_actions;

namespace Figurebox.content;

class KingdomPolicyLibrary : AssetLibrary<KingdomPolicyAsset>
{
    public static KingdomPolicyLibrary Instance { get; } = new();

    // 这里可以根据需要添加特定于 KingdomPolicyAsset 的方法
    public override void init()
    {
        base.init();
        // 只注册政策, 不要链接状态
        // 开启奴隶制
        add("start_slaves", "ui/policy/start_slaves", 100, 100, null,
            MajorPolicyExecuteActions.StartSlaves, false, false);
        // 强化奴隶控制
        add("control_slaves", "ui/icons/iconDamage", 100, 100, null,
            MajorPolicyExecuteActions.EnforceSlavesControl, true, false);
        // 奴隶军
        add("slaves_army", "ui/icons/iconDamage", 100, 100, null,
            MajorPolicyExecuteActions.EnableSlavesArmy, false, false);
        // 姓氏合流
        add("name_integration", "ui/icons/iconDamage", 100, 100, null,
            MajorPolicyExecuteActions.NameIntegration, false, false);
#if 一米_中文名
        add("kingdom_yearname", "ui/policy/change_name", 1, 1, null,
            MajorPolicyExecuteActions.MakeNewYearName, true, false);
#endif
        add("change_capital", "ui/policy/move_capital", 100, 100, InPeace,
            MajorPolicyExecuteActions.ChangeCapital, true, false);
        add("title_upgrade", "ui/policy/change_name", 100, 100, CanBePromoted,
            MajorPolicyExecuteActions.UpgradeKingdomTitle, true, false);
        // 开启半奴隶制半封建制
        add("start_halfaristocrat", "ui/policy/start_halfaristocrat", 100, 100, null,
            MajorPolicyExecuteActions.StartHalfAristocrat, false, false);
    }

    public override void post_init()
    {
        base.post_init();
        get("start_slaves").AddPreposition(KingdomPolicyStateLibrary.DefaultState);
        get("start_slaves").SetTargetState(KingdomPolicyStateLibrary.SocialLevel_SlaveOwner);

        get("control_slaves").AddPreposition(KingdomPolicyStateLibrary.SocialLevel_SlaveOwner);

        get("slaves_army").AddPreposition(KingdomPolicyStateLibrary.SocialLevel_SlaveOwner);
        get("slaves_army").SetTargetState(KingdomPolicyStateLibrary.MainSoldiers_Slaves);

        get("start_halfaristocrat").AddPreposition(KingdomPolicyStateLibrary.SocialLevel_SlaveOwner);
        get("start_halfaristocrat").AddPreposition(KingdomPolicyStateLibrary.MainSoldiers_Slaves);
        get("start_halfaristocrat").SetTargetState(KingdomPolicyStateLibrary.SocialLevel_HalfAristocrat);

        get("name_integration").AddPreposition(KingdomPolicyStateLibrary.SocialLevel_HalfAristocrat);
        get("name_integration").SetTargetState(KingdomPolicyStateLibrary.Name_Integration);



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