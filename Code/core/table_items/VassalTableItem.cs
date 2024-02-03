using Figurebox.attributes;

namespace Figurebox.core.table_items;

[TableDef("VASSAL")]
public class VassalTableItem : AbstractTableItem<VassalTableItem>
{
    public string vaid;
    public string said;
    [TableItemDef(pDefaultValue: "-1")] public double start_time;
    [TableItemDef(pDefaultValue: "-1")] public double end_time;
    public string kid;
    public string Skid;
    public string vassal_name;
    public string lord_name;
}