using Figurebox.attributes;

namespace Figurebox.core.table_items;

[TableDef("KingdomChangeName")]
public class KingdomChangeNameTableItem : AbstractTableItem<KingdomChangeNameTableItem>
{
    public string id;
    public string old_name;
    public double timestamp;
}