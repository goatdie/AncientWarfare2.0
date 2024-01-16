using Figurebox.attributes;

namespace Figurebox.core.table_items;

[TableDef("CityChangeName")]
public class CityChangeNameTableItem
{
    public string id;
    public string old_name;
    public double timestamp;
}