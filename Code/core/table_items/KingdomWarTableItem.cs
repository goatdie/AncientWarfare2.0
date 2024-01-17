using Figurebox.attributes;

namespace Figurebox.core.table_items;

[TableDef("KingdomWar")]
public class KingdomWarTableItem : AbstractTableItem<KingdomWarTableItem>
{
    [TableItemDef(pDefaultValue: "0")] public int dead;
    public string event_name;
    public string kid;
    [TableItemDef(pDefaultValue: "0")] public int kill;
    public double timestamp;
    public string wid;
}