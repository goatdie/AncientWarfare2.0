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
}