using System.Collections.Generic;
using System.Data.SQLite;

namespace Figurebox.core.table_items;

// 手动定义表
public class CityPopCompositionTableItem : AbstractTableItem<CityPopCompositionTableItem>
{
    public readonly Dictionary<string, int> professions = new();
    public          double                  timestamp;

    public override void ReadFromReader(SQLiteDataReader reader)
    {
        foreach (ProfessionAsset prof in AssetManager.professions.list) professions[prof.id] = 0;

        timestamp = reader.GetDouble(reader.GetOrdinal("TIMESTAMP"));
        foreach (var pair in professions)
        {
            var ordinal = reader.GetOrdinal(pair.Key + "_Count");
            if (ordinal == -1) continue;
            professions[pair.Key] = reader.GetInt32(ordinal);
        }
    }

    public new static string GetTableName()
    {
        return "CityPopComposition";
    }
}