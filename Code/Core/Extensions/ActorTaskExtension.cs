using ai.behaviours;
using AncientWarfare.Core.Additions;

namespace AncientWarfare.Core.Extensions;

public static class ActorTaskExtension
{
    public static ActorTaskAdditionAsset GetAdditionAsset(this BehaviourTaskActor task)
    {
        return ActorTaskAddtionAssetLibrary.Get(task.id);
    }
}