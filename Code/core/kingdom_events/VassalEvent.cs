namespace Figurebox.Core.KingdomEvents;

public class VassalEvent : AEvent<VassalEvent>
{
    public string SuzerainName;
    public VassalEvent(string pKingdomId, string pSuzerainName) : base(pKingdomId)
    {
        SuzerainName = pSuzerainName;
    }
}