using Figurebox.attributes;

namespace Figurebox.core.table_items;

[TableDef("INTEGRATION")]
public class IntegrationTableItem : AbstractTableItem<IntegrationTableItem>
{
    public string aid;
    public string old_kingdom_name; //之后改成id
    public string new_kingdom_name;

    public double timestamp;
}