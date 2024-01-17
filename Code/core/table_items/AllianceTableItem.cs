using Figurebox.attributes;

namespace Figurebox.core.table_items;

[TableDef("Alliance")]
public class AllianceTableItem : AbstractTableItem<AllianceTableItem>
{
    public int banner_bg;
    public int banner_icon;
    public string curr_name;
    [TableItemDef(pDefaultValue: "-1")] public double end_time;
    [TableItemDef(pIsPrimary: true)] public string id;
    public double start_time;
}