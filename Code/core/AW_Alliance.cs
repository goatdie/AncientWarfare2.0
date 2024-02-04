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
        if (MoHTools.ConvertKtoAW(pKingdom).IsSuzerain())
        {
            foreach (var vassal in MoHTools.ConvertKtoAW(pKingdom).GetVassals())
            {
                vassal.allianceJoin(this);
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