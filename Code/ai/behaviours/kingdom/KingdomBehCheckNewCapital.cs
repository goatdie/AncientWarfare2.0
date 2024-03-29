using ai.behaviours;
using Figurebox.content;
using Figurebox.core;
using Figurebox.core.kingdom_policies;

namespace Figurebox.ai.behaviours.kingdom;

public class KingdomBehCheckNewCapital : BehaviourActionKingdom
{
    public override BehResult execute(Kingdom pKingdom)
    {
        AW_Kingdom awKingdom = pKingdom as AW_Kingdom;

        City newCapital = AW_Kingdom.FindNewCapital(awKingdom);
        if (newCapital == awKingdom.capital || newCapital == null)
        {
            return BehResult.Continue;
        }
        else
        {

            Main.LogInfo("迁都" + newCapital.getCityName() + "旧都" + awKingdom.capital.getCityName());
            KingdomPolicyAsset policy = KingdomPolicyLibrary.Instance.get("change_capital");
            awKingdom.StartPolicy(policy, false);
        }

        return BehResult.Continue;
    }
}
