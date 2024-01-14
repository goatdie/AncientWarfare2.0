using Figurebox.core;
using NeoModLoader.api.attributes;

namespace Figurebox.policy_actions;

internal static class MajorPolicyExecuteActions
{
    [Hotfixable]
    public static void StartSlaves(KingdomPolicyAsset policy, AW_Kingdom kingdom)
    {
        if (kingdom.policy_data.p_progress == 0)
            Main.LogInfo($"{kingdom.name} 正在尝试推行奴隶制 {kingdom.policy_data.p_progress} / {policy.cost_in_progress}");
    }

    [Hotfixable]
    public static void EnforceSlavesControl(KingdomPolicyAsset policy, AW_Kingdom kingdom)
    {
        if (kingdom.policy_data.p_progress == 0)
            Main.LogInfo($"{kingdom.name} 正在尝试强化奴隶控制 {kingdom.policy_data.p_progress} / {policy.cost_in_progress}");
    }

    [Hotfixable]
    public static void EnableSlavesArmy(KingdomPolicyAsset policy, AW_Kingdom kingdom)
    {
    }

    [Hotfixable]
    public static void NameIntegration(KingdomPolicyAsset policy, AW_Kingdom kingdom)
    {
        if (kingdom.policy_data.p_progress == 0)
            Main.LogInfo($"{kingdom.name} 正在尝试推行姓氏合流{kingdom.policy_data.p_progress} / {policy.cost_in_progress}");
        if (kingdom.policy_data.p_status == KingdomPolicyData.PolicyStatus.InProgress &&
            kingdom.policy_data.p_progress == 0)
        {
            kingdom.ToggleNameIntegration(true);
        }
    }
#if 一米_中文名
    [Hotfixable]
    public static void MakeNewYearName(KingdomPolicyAsset policy, AW_Kingdom kingdom)
    {
        if (kingdom.policy_data.p_progress == 0)
            Main.LogInfo($"{kingdom.name} 正在尝试推行建立新年号{kingdom.policy_data.p_progress} / {policy.cost_in_progress}");
        if (kingdom.policy_data.p_status == KingdomPolicyData.PolicyStatus.InProgress &&
            kingdom.policy_data.p_progress == 0)
        {
            KingdomYearName.Make_New_YearName(kingdom);
        }
    }
#endif
    [Hotfixable]
    public static void ChangeCapital(KingdomPolicyAsset policy, AW_Kingdom kingdom)
    {
        if (kingdom.policy_data.p_progress == 0)
            Main.LogInfo($"{kingdom.name} 正在尝试推行迁都{kingdom.policy_data.p_progress} / {policy.cost_in_progress}");
        if (kingdom.policy_data.p_status == KingdomPolicyData.PolicyStatus.InProgress &&
            kingdom.policy_data.p_progress == 0)
        {
            AW_Kingdom.SetNewCapital(kingdom);
        }
    }

    [Hotfixable]
    public static void UpgradeKingdomTitle(KingdomPolicyAsset policy, AW_Kingdom kingdom)
    {
        //Main.LogInfo($"{kingdom.name} 正在尝试提升爵位{kingdom.policy_data.p_progress} / {policy.cost_in_progress}");

        if (kingdom.policy_data.p_status == KingdomPolicyData.PolicyStatus.InProgress &&
            kingdom.policy_data.p_progress == 0)
        {
            kingdom.PromoteTitle();
            kingdom.policy_data.p_promotion_done = World.world.mapStats.worldTime;
            KingdomYearName.changeYearname(kingdom);
        }
    }
}