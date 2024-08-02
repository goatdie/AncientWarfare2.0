using AncientWarfare.Abstracts;

namespace AncientWarfare.Core;

internal class ActorAdditionDataManager : AdditionDataManager<ActorAdditionData>
{
    public static ActorAdditionData Get(string id)
    {
        return _data.TryGetValue(id, out var data) ? data : _data[id] = new ActorAdditionData();
    }

    public static ActorAdditionData TryGet(string id)
    {
        return _data.TryGetValue(id, out ActorAdditionData data) ? data : null;
    }
}