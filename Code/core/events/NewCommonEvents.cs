using Figurebox.core.table_items;
using Figurebox.Utils;

namespace Figurebox.core.events;

public static class NewCommonEvents
{
    public static void NewKingRule(this EventsManager pManager, Kingdom pKingdom, Actor pKing)
    {
        if (!pManager.OperatingDB.CheckKeyExist(ActorTableItem.GetTableName(),
                                                SimpleColumnConstraint.CreateEq("ID", pKing.data.id)))
            pManager.OperatingDB.Insert(ActorTableItem.GetTableName(),
                                        ColumnVal.Create("ID",         pKing.data.id),
                                        ColumnVal.Create("CURR_NAME",  pKing.getName()),
                                        ColumnVal.Create("HEAD",       pKing.data.head),
                                        ColumnVal.Create("SKIN",       pKing.data.skin),
                                        ColumnVal.Create("SKIN_SET",   pKing.data.skin_set),
                                        ColumnVal.Create("ASSET_ID",   pKing.asset.id),
                                        ColumnVal.Create("START_TIME", pKing.data.created_time),
                                        ColumnVal.Create("END_TIME",   -1)
            );
        pManager.OperatingDB.Insert(KingRuleTableItem.GetTableName(),
                                    ColumnVal.Create("AID",        pKing.data.id),
                                    ColumnVal.Create("KID",        pKingdom.id),
                                    ColumnVal.Create("START_TIME", pKingdom.data.timestamp_king_rule),
                                    ColumnVal.Create("END_TIME",   -1)
        );
    }

    public static void ChangeYearName(this EventsManager pManager, Kingdom pKingdom, string pNewYearName)
    {
        pManager.OperatingDB.Insert(KingdomChangeYearTableItem.GetTableName(),
                                    ColumnVal.Create("ID",        pKingdom.id),
                                    ColumnVal.Create("NEW_NAME",  pNewYearName),
                                    ColumnVal.Create("TIMESTAMP", World.world.getCreationTime()));
    }
}