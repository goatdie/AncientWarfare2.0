using System.Collections.Generic;
using Figurebox.Utils;
using NeoModLoader.api.attributes;

namespace Figurebox.core.events;

public static class EndCommonEvents
{
    [Hotfixable]
    public static void EndKingRule(this EventsManager pThis, Kingdom pKingdom, Actor pKing)
    {
        pThis.OperatingDB.UpdateValue("KingRule", new List<SimpleColumnConstraint>
        {
            SimpleColumnConstraint.CreateEq("KID", pKingdom.id),
            SimpleColumnConstraint.CreateEq("AID", pKing.data.id),
            SimpleColumnConstraint.CreateLt("END_TIME", 0)
        }, ColumnVal.Create("END_TIME", World.world.getCreationTime()));
    }
}