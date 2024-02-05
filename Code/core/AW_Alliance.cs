using Figurebox.Utils.extensions;

namespace Figurebox.core;
using Figurebox.attributes;
using Figurebox.Utils.MoH;
public class AW_Alliance : Alliance
{
    [MethodReplace]
    public new bool join(Kingdom pKingdom, bool pRecalc = true)
    {
        if (!this.canJoin(pKingdom))
        {
            return false;
        }
        this.kingdoms_hashset.Add(pKingdom);
        pKingdom.allianceJoin(this);
        if (pKingdom.AW().IsSuzerain())
        {
            foreach (var vassal in pKingdom.AW().GetVassals())
            {
                vassal.allianceJoin(this);
                this.kingdoms_hashset.Add(vassal);
            }
        }
        if (pRecalc)
        {
            this.recalculate();
        }
        this.data.timestamp_member_joined = World.world.getCurWorldTime();
        return true;
    }
}