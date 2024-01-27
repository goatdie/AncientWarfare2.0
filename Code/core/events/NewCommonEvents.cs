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
                                        ColumnVal.Create("ID", pKing.data.id),
                                        ColumnVal.Create("CURR_NAME", pKing.getName()),
                                        ColumnVal.Create("HEAD", pKing.data.head),
                                        ColumnVal.Create("SKIN", pKing.data.skin),
                                        ColumnVal.Create("SKIN_SET", pKing.data.skin_set),
                                        ColumnVal.Create("ASSET_ID", pKing.asset.id),
                                        ColumnVal.Create("START_TIME", pKing.data.created_time),
                                        ColumnVal.Create("END_TIME", -1)
            );
        pManager.OperatingDB.Insert(KingRuleTableItem.GetTableName(),
                                    ColumnVal.Create("AID", pKing.data.id),
                                    ColumnVal.Create("KID", pKingdom.id),
                                    ColumnVal.Create("START_TIME", pKingdom.data.timestamp_king_rule),
                                    ColumnVal.Create("END_TIME", -1)
        );
    }

    public static void ChangeYearName(this EventsManager pManager, Kingdom pKingdom, string pNewYearName)
    {
        pManager.OperatingDB.Insert(KingdomChangeYearTableItem.GetTableName(),
                                    ColumnVal.Create("ID", pKingdom.king.data.id),
                                    ColumnVal.Create("NEW_NAME", pNewYearName),
                                    ColumnVal.Create("TIMESTAMP", World.world.getCreationTime()));
    }
    public static void StartMOH(this EventsManager pManager, Kingdom pKingdom)
    {
        pManager.OperatingDB.Insert(MOHTableItem.GetTableName(),
                                    ColumnVal.Create("AID", pKingdom.king.data.id),
                                    ColumnVal.Create("KID", pKingdom.id),
                                    ColumnVal.Create("START_TIME", World.world.getCreationTime()),
                                    ColumnVal.Create("KINGDOM_NAME", pKingdom.data.name),
                                    ColumnVal.Create("END_TIME", -1)
                                    );

    }
    public static void ENDMOH(this EventsManager pManager, Kingdom pKingdom)
    {
        pManager.OperatingDB.Insert(MOHTableItem.GetTableName(),
                                    ColumnVal.Create("AID", pKingdom.king.data.id),
                                    ColumnVal.Create("KID", pKingdom.id),
                                    ColumnVal.Create("END_TIME", World.world.getCreationTime()),
                                    ColumnVal.Create("KINGDOM_NAME", pKingdom.data.name),
                                    ColumnVal.Create("START_TIME", -1)

                                    );

    }
    public static void StartUsurpation(this EventsManager pManager, Actor Usuperer, string KName)
    {
        pManager.OperatingDB.Insert(UsurpationTableItem.GetTableName(),
                                    ColumnVal.Create("AID", Usuperer.data.id),
                                    ColumnVal.Create("KID", Usuperer.kingdom.id),
                                    ColumnVal.Create("TIMESTAMP", World.world.getCreationTime()),
                                    ColumnVal.Create("KINGDOM_NAME", KName)

                                    );

    }
    public static void Integration(this EventsManager pManager, Actor pKing, string originalKingdomName, string inheritingKingdomName)
    {
        pManager.OperatingDB.Insert(IntegrationTableItem.GetTableName(),
                                    ColumnVal.Create("AID", pKing.data.id),
                                    ColumnVal.Create("NEW_KINGDOM_NAME", inheritingKingdomName),
                                    ColumnVal.Create("TIMESTAMP", World.world.getCreationTime()),
                                    ColumnVal.Create("OLD_KINGDOM_NAME", originalKingdomName)

                                    );

    }
    public static void ChangeCapital(this EventsManager pManager, Actor pKing, string old_capital_name, string new_capital_name)
    {
        pManager.OperatingDB.Insert(KingdomCapitalChangeTableItem.GetTableName(),
                                    ColumnVal.Create("AID", pKing.data.id),
                                    ColumnVal.Create("NEW_CAPITAL_NAME", old_capital_name),
                                    ColumnVal.Create("TIMESTAMP", World.world.getCreationTime()),
                                    ColumnVal.Create("OLD_CAPITAL_NAME", new_capital_name)

                                    );

    }
}