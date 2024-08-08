using AncientWarfare.Abstracts;

namespace AncientWarfare.Core.Additions;

public class ActorJobAdditionAssetLibrary : AdditionDataManager<ActorJobAdditionAsset>
{
    public static ActorJobAdditionAsset Get(string id)
    {
        return _data.TryGetValue(id, out ActorJobAdditionAsset data) ? data : _data[id] = new ActorJobAdditionAsset(id);
    }
}