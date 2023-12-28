using Figurebox.Utils.MoH;
namespace Figurebox.core;

public partial class AW_Kingdom
{
    public bool IsMoHKingdom => MoHTools.MoHKingdom == this;
}