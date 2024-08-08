using AncientWarfare.Abstracts;

namespace AncientWarfare.Core.Additions;

public class ActorTaskAddtionAssetLibrary : AdditionDataManager<ActorTaskAdditionAsset>
{
    public static ActorTaskAdditionAsset Get(string id)
    {
        return _data.TryGetValue(id, out ActorTaskAdditionAsset data)
            ? data
            : _data[id] = new ActorTaskAdditionAsset(id);
    }
}