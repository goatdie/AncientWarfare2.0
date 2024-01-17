using Figurebox.attributes;

namespace Figurebox.core.table_items;

[TableDef("City")]
public class CityTableItem : AbstractTableItem<CityTableItem>
{
    public string curr_kingdom;
    public string curr_name;
    [TableItemDef(pDefaultValue: "-1")] public double end_time;
    [TableItemDef(pIsPrimary: true)] public string id;
    public double start_time;
}