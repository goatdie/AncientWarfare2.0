using Figurebox.core;
using System.Collections.Generic;
namespace Figurebox.Utils;

public static class ActorTools
{
    /// <summary>
    ///     改变生物所属城市
    /// </summary>
    /// <param name="actor"></param>
    /// <param name="ncity"></param>
    public static void ChangeCity(this Actor actor, City ncity)
    {
        actor.city?.removeUnit(actor, false);
        ncity.addNewUnit(actor);
    }
    public static bool IsHeir(this Actor actor)
    {
        Clan pClan = actor.getClan();
        if (pClan == null)
        {
            return false;
        }
        if (pClan.units.Count == 0)
        {
            return false;
        }
        foreach (Actor value in pClan.units.Values)
        {
            if (value.isKing())
            {
                AW_Kingdom actorKingdom = value.kingdom as AW_Kingdom;
                return actorKingdom.heir == actor;
            }
        }

        return false;
    }
}