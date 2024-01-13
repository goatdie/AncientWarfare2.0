using Figurebox.Utils;

namespace Figurebox.core.events;

public abstract class BaseEvent : IHasKey<int>
{
    public double StartTimestamp { get; private set; }
    public double EndTimestamp { get; private set; } = -1;

    public int GetKey()
    {
        return World.world.getYearsSince(StartTimestamp);
    }

    public void Start()
    {
        StartTimestamp = World.world.mapStats.worldTime;
    }

    public void End()
    {
        EndTimestamp = World.world.mapStats.worldTime;
    }
}

public abstract class AEvent<T> : BaseEvent, IHasKey<int> where T : AEvent<T>
{
    public AEvent(string pKingdomId)
    {
        KingdomId = pKingdomId;
    }

    public bool IsInProgress => EndTimestamp < 0;
    public string KingdomId { get; private set; }

    public static void CommitEvent(T pEvent)
    {
        if (pEvent.IsInProgress)
        {
            pEvent.End();
        }
    }
}