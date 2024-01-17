using Figurebox.attributes;

namespace Figurebox.core.table_items;

[TableDef("KingRule")]
public class KingRuleTableItem : AbstractTableItem<KingRuleTableItem>
{
    public string aid;
    [TableItemDef(pDefaultValue: "-1")] public double end_time;
    public string kid;
    public double start_time;
}