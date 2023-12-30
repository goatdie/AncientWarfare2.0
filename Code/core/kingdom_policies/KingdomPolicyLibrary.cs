using Figurebox.policy_actions;
namespace Figurebox;

class KingdomPolicyLibrary : AssetLibrary<KingdomPolicyAsset>
{
    public static KingdomPolicyLibrary Instance { get; } = new();
    // 这里可以根据需要添加特定于 KingdomPolicyAsset 的方法
    public override void init()
    {
        base.init();
        // 只注册政策, 不要链接状态
        // 开启奴隶制
        add("start_slaves", "start_slaves", "start_slaves_desc", "ui/icons/iconDamage", 100, 100, null, MajorPolicyExecuteActions.StartSlaves, false, false);
        // 强化奴隶控制
        add("control_slaves", "control_slaves", "control_slaves_desc", "ui/icons/iconDamage", 100, 100, null, MajorPolicyExecuteActions.EnforceSlavesControl, true, false);
        // 姓氏合流
        add("name_integration", "name_integration", "name_integration_desc", "ui/icons/iconDamage", 100, 100, null, MajorPolicyExecuteActions.NameIntegration, false, false);
        add("kingdom_yearname", "kingdom_yearname", "kingdom_yearname_desc", "ui/icons/iconDamage", 1, 1, null, MajorPolicyExecuteActions.MakeNewYearName, true, false);
    }
    public override void post_init()
    {
        base.post_init();
        //get("control_slaves").AddPreposition("slaves");
    }
    public override void linkAssets()
    {
        base.linkAssets();
    }
    public KingdomPolicyAsset add(
        string pID, string pNameKey, string pDescKey, string pPathIcon,
        int pPlanCost, int pProgressCost,
        CheckPolicy pCheckAction, ExecutePolicy pExecuteAction,
        bool pCanRepeat, bool pOnlyMoH
    )
    {
        return add(new KingdomPolicyAsset
        {
            id = pID,
            policyname = pNameKey,
            description = pDescKey,
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
}