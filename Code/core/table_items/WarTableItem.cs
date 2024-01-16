using Figurebox.attributes;

namespace Figurebox.core.table_items;

[TableDef("War")]
public class WarTableItem
{
    public string curr_name;
    [TableItemDef(pDefaultValue: "-1")] public double end_time;
    [TableItemDef(pIsPrimary: true)] public string id;
    public double start_time;
}