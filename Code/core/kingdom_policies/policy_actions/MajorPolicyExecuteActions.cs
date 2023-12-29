using Figurebox.core;
namespace Figurebox.policy_actions;

internal static class MajorPolicyExecuteActions
{
    public static void StartSlaves(KingdomPolicyAsset policy, AW_Kingdom kingdom)
    {
        Main.LogInfo($"{kingdom.name} 正在尝试推行奴隶制 {kingdom.policy_data.p_progress} / {policy.cost_in_progress}");
    }
}