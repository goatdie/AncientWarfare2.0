using System.Collections.Generic;
using System.IO;
using System.Linq;
using Figurebox.Utils;
using NeoModLoader.api.attributes;

namespace Figurebox.core.dbs;

public class CityPopRecordManager : AMultiTableDBManager<CityPopRecordManager>
{
    protected override void PrepareTableDef()
    {
        ColumnDefs = new List<SQLiteHelper.ColumnDef> { new("TIMESTAMP", SQLiteHelper.ColumnType.INTEGER) };
        ColumnDefs.AddRange(AssetManager.professions.dict_profession_id.Select(pair =>
                                                                                   new SQLiteHelper.
                                                                                       ColumnDef(pair.Value.id + "_Count",
                                                                                           SQLiteHelper.ColumnType
                                                                                               .INTEGER)));
    }

    protected override string GetDBFilePath()
    {
        return Path.Combine(Main.mainPath, ".tmp.CityPopComposition.db");
    }

    [Hotfixable]
    public static void NewCityPopCompositionRecord(City pCity)
    {
        if (!InitializeSuccessful) return;
        if (pCity.professionsDict == null || pCity.professionsDict.Count == 0) return;
        Instance.CheckTable(pCity.data.id);

        List<ColumnVal> column_vals = new() { ColumnVal.Create("TIMESTAMP", World.world.getCreationTime()) };
        column_vals.AddRange(pCity.professionsDict.Select(pair =>
                                                              ColumnVal
                                                                  .Create(AssetManager.professions.dict_profession_id[pair.Key].id + "_Count",
                                                                          pair.Value.Count)));

        Instance.OperatingDB.Insert(pCity.data.id, column_vals.ToArray());
    }
}