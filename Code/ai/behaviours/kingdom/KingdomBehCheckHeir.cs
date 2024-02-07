using ai.behaviours;
using Figurebox.core;

namespace Figurebox.ai.behaviours.kingdom;

public class KingdomBehCheckHeir : BehaviourActionKingdom
{
    public override BehResult execute(Kingdom pKingdom)
    {
        AW_Kingdom awKingdom = pKingdom as AW_Kingdom;
        awKingdom.CheckHeir();

        if (awKingdom.hasHeir())
        {
            return BehResult.Continue;
        }


        else
        {
            awKingdom.SetHeir(awKingdom.FindHeir());
        }

        return BehResult.Continue;
    }
}