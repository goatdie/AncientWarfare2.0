namespace Figurebox.Core.KingdomEvents;

public class WarEvent : AEvent<WarEvent>
{

    public WarEvent(string pKingdomId) : base(pKingdomId)
    {
    }
}