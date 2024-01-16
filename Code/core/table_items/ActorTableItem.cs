using Figurebox.attributes;

namespace Figurebox.core.table_items;

[TableDef("Actor")]
public class ActorTableItem
{
    public string asset_id;
    public string curr_name;
    [TableItemDef(pDefaultValue: "-1")] public double end_time;
    public int head;
    [TableItemDef(pIsPrimary: true)] public string id;
    public int skin;
    public int skin_set;
    public double start_time;
}