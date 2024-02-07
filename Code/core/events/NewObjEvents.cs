using Figurebox.utils;

namespace Figurebox.core.events;

public static class NewObjEvents
{
    public static void NewKingdom(this EventsManager pManager, Kingdom pKingdom)
    {
        pManager.OperatingDB.Insert("Kingdom",
            ColumnVal.Create("ID", pKingdom.id),
            ColumnVal.Create("CURR_NAME", pKingdom.data.name),
            ColumnVal.Create("BANNER_ICON", pKingdom.data.banner_icon_id),
            ColumnVal.Create("BANNER_BG", pKingdom.data.banner_background_id),
            ColumnVal.Create("COLOR_ID", pKingdom.data.colorID),
            ColumnVal.Create("START_TIME", pKingdom.data.created_time),
            ColumnVal.Create("END_TIME", -1)
        );
    }

    public static void NewCity(this EventsManager pManager, City pCity)
    {
        pManager.OperatingDB.Insert("City",
            ColumnVal.Create("ID", pCity.data.id),
            ColumnVal.Create("CURR_NAME", pCity.getCityName()),
            ColumnVal.Create("CURR_KINGDOM", pCity.kingdom?.id),
            ColumnVal.Create("START_TIME", pCity.data.created_time),
            ColumnVal.Create("END_TIME", -1)
        );
    }

    public static void NewWar(this EventsManager pManager, War pWar)
    {
        pManager.OperatingDB.Insert("War",
            ColumnVal.Create("ID", pWar.data.id),
            ColumnVal.Create("CURR_NAME", pWar.name),
            ColumnVal.Create("START_TIME", pWar.data.created_time),
            ColumnVal.Create("END_TIME", -1)
        );
    }

    public static void NewAlliance(this EventsManager pManager, Alliance pAlliance)
    {
        pManager.OperatingDB.Insert("Alliance",
            ColumnVal.Create("ID", pAlliance.data.id),
            ColumnVal.Create("CURR_NAME", pAlliance.name),
            ColumnVal.Create("BANNER_ICON", pAlliance.data.banner_icon_id),
            ColumnVal.Create("BANNER_BG", pAlliance.data.banner_background_id),
            ColumnVal.Create("START_TIME", pAlliance.data.created_time),
            ColumnVal.Create("END_TIME", -1)
        );
    }
}