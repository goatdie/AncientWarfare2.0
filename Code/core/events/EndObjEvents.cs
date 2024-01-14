using System.Collections.Generic;
using Figurebox.Utils;

namespace Figurebox.core.events;

public static class EndObjEvents
{
    public static void EndKingdom(this EventsManager pThis, Kingdom pKingdom)
    {
        if (pKingdom?.data == null) return;
        pThis.OperatingDB.UpdateValue("Kingdom", new List<SimpleColumnConstraint>
        {
            SimpleColumnConstraint.CreateEq("ID", pKingdom.id)
        }, ColumnVal.Create("END_TIME", World.world.getCreationTime()));
    }

    public static void EndWar(this EventsManager pThis, War pWar)
    {
        if (pWar?.data == null) return;
        pThis.OperatingDB.UpdateValue("War", new List<SimpleColumnConstraint>
        {
            SimpleColumnConstraint.CreateEq("ID", pWar.data.id)
        }, ColumnVal.Create("END_TIME", World.world.getCreationTime()));
    }

    public static void EndCity(this EventsManager pThis, City pCity)
    {
        if (pCity?.data == null) return;
        pThis.OperatingDB.UpdateValue("City", new List<SimpleColumnConstraint>
        {
            SimpleColumnConstraint.CreateEq("ID", pCity.data.id)
        }, ColumnVal.Create("END_TIME", World.world.getCreationTime()));
    }

    public static void EndAlliance(this EventsManager pThis, Alliance pAlliance)
    {
        if (pAlliance?.data == null) return;
        pThis.OperatingDB.UpdateValue("Alliance", new List<SimpleColumnConstraint>
        {
            SimpleColumnConstraint.CreateEq("ID", pAlliance.data.id)
        }, ColumnVal.Create("END_TIME", World.world.getCreationTime()));
    }
}