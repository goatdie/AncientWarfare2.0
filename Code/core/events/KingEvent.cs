namespace Figurebox.core.events;

public class KingEvent : AEvent<KingEvent>
{
    public string KingName;

    public KingEvent(string pKingdomId, string pKingName) : base(pKingdomId)
    {
        KingName = pKingName;
    }
}