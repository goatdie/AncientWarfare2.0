using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Figurebox.constants;
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
        if (pCity.professionsDict == null) return;
        Instance.CheckTable(pCity.data.id);

        List<ColumnVal> column_vals = new() { ColumnVal.Create("TIMESTAMP", World.world.getCreationTime()) };
        column_vals.AddRange(pCity.professionsDict.Select(pair =>
                                                              ColumnVal
                                                                  .Create(AssetManager.professions.dict_profession_id[pair.Key].id + "_Count",
                                                                          pair.Value.Count)));
        try
        {
            Instance.OperatingDB.Insert(pCity.data.id, column_vals.ToArray());
        }
        catch (Exception e)
        {
            StringBuilder sb = new();
            sb.AppendLine($"Failed to insert record for city {pCity.data.id}");
            sb.AppendLine($"Exception: {e}");
            sb.AppendLine("Column values:");
            foreach (ColumnVal column_val in column_vals) sb.AppendLine($"{column_val.Name}: {column_val.Value}");

            sb.AppendLine("City professions:");
            foreach (var pair in pCity.professionsDict)
                sb.AppendLine($"{(AWUnitProfession)pair.Key}: {pair.Value.Count}");

            sb.AppendLine("All parameters:");
            foreach (SQLiteHelper.ColumnDef def in Instance.ColumnDefs) sb.AppendLine($"{def.Name}({def.ValueType})");
            Main.LogWarning(sb.ToString());
        }
    }
}