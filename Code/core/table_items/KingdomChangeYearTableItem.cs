using Figurebox.attributes;

namespace Figurebox.core.table_items;

[TableDef("KingdomChangeYear")]
public class KingdomChangeYearTableItem : AbstractTableItem<KingdomChangeYearTableItem>
{
    public string id;
    public string old_name;
    public double timestamp;
}