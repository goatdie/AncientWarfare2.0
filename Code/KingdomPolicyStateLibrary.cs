namespace Figurebox;

public class KingdomPolicyStateLibrary : AssetLibrary<KingdomPolicyStateAsset>
{
    public static KingdomPolicyStateLibrary Instance { get; } = new();
    public static KingdomPolicyStateAsset DefaultState { get; } = new();
    public override void init()
    {
        base.init();
        DefaultState.id = "default";
        DefaultState.policy_finder = (pKingdom, pPolicyData, pState) =>
        {
            var policy = KingdomPolicyLibrary.Instance.get("hello_world");
            if (policy == null)
            {
                Main.LogWarning("政策'hello_world'不存在, 终止", true);
                return null;
            }
            return policy;
        };
        add(DefaultState);
    }
}