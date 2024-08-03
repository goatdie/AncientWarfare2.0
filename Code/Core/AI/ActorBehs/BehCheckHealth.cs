using ai.behaviours;

namespace AncientWarfare.Core.AI.ActorBehs;

public class BehCheckHealth : BehaviourActionActor
{
    private readonly float min_ratio;
    private readonly bool  reverse;

    public BehCheckHealth(float ratio = 0.6f, bool reverse = false)
    {
        min_ratio = ratio;
        this.reverse = reverse;
    }

    public override BehResult execute(Actor pObject)
    {
        if (pObject.data.health < pObject.getMaxHealth() * min_ratio)
            return reverse ? BehResult.Continue : BehResult.Stop;

        return reverse ? BehResult.Stop : BehResult.Continue;
    }
}