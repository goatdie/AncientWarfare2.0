using ai.behaviours;
using Figurebox.content;
using Figurebox.core;
using Figurebox.core.kingdom_policies;

namespace Figurebox.ai.behaviours.kingdom;
public class KingdomBehCheckPromotion : BehaviourActionKingdom
{
    public override BehResult execute(Kingdom pKingdom)
    {

        AW_Kingdom awKingdom = pKingdom as AW_Kingdom;
        if (!CheckPromotionTime(awKingdom))
        {
            return BehResult.Continue;
            
        }
        KingdomPolicyAsset policy = KingdomPolicyLibrary.Instance.get("title_upgrade");

        awKingdom.StartPolicy(policy, false);
        return BehResult.Continue;
    }
    public bool CheckPromotionTime(AW_Kingdom kingdom)
    {
        if (kingdom.addition_data.p_promotion_done == 0)
        {
            return true;
        }
        int num = World.world.mapStats.getYearsSince(kingdom.addition_data.p_promotion_done);
        return num >= 50;

    }
}









