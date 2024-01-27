using Figurebox.attributes;

namespace Figurebox.core.table_items;

[TableDef("USURPATION")]
public class UsurpationTableItem : AbstractTableItem<UsurpationTableItem>
{
    public string aid;
    public double timestamp;
    public string kid;
    public string kingdom_name;

}