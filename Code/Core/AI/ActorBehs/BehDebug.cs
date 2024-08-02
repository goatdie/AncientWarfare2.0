using ai.behaviours;

namespace AncientWarfare.Core.AI.ActorBehs;

public class BehDebug : BehaviourActionActor
{
    private readonly string msg_prefix;

    public BehDebug(string msg_prefix = "")
    {
        this.msg_prefix = msg_prefix;
    }

    public override BehResult execute(Actor pObject)
    {
        Main.LogDebug($"{msg_prefix}. {pObject.data.id}");
        return BehResult.Continue;
    }
}