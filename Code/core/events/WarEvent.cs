namespace Figurebox.core.events;

public class WarEvent : AEvent<WarEvent>
{
    public WarEvent(string pKingdomId) : base(pKingdomId)
    {
    }
}