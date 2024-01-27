using Figurebox.attributes;

namespace Figurebox.core.table_items;

[TableDef("MOH")]
public class MOHTableItem : AbstractTableItem<MOHTableItem>
{
    public string aid;
    [TableItemDef(pDefaultValue: "-1")] public double end_time;
    public string kid;
    public string kingdom_name;
    [TableItemDef(pDefaultValue: "-1")] public double start_time;
}