using AncientWarfare.Core.Extensions;

namespace AncientWarfare.Core.Content;

public static class WorldActionLibrary
{
    public static bool CheckChildBirth(BaseSimObject obj, WorldTile tile)
    {
        Actor actor = obj.a;
        actor.activeStatus_dict.TryGetValue(nameof(StatusEffectExtendLibrary.pregnant), out StatusEffectData status);
        if (status._end_time - World.world.getCurWorldTime() > status.asset.action_interval) return true;

        actor.GiveChildBirth();
        return true;
    }
}