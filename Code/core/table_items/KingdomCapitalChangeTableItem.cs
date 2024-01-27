using Figurebox.attributes;

namespace Figurebox.core.table_items;

[TableDef("CAPITALCHANGE")]
public class KingdomCapitalChangeTableItem : AbstractTableItem<KingdomCapitalChangeTableItem>
{
    public string aid;
    public string old_capital_name; //之后改成id
    public string new_capital_name;

    public double timestamp;
}