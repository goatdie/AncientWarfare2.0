using System.Linq;
using Figurebox.attributes;

namespace Figurebox.core;

public class AW_UnitGroup : UnitGroup
{
    public AW_UnitGroupAsset asset;
    public AW_UnitGroupData  data;

    public AW_UnitGroup(City pCity, AW_UnitGroupAsset pAsset, AW_UnitGroupData pData) : base(pCity)
    {
        asset = pAsset;
        data = pData;
    }

    [MethodReplace(nameof(UnitGroup.findGroupLeader))]
    public new void findGroupLeader()
    {
        if (groupLeader != null)
        {
            if (groupLeader.kingdom.isCiv()) return;
            setGroupLeader(null);
        }

        if (units.Count == 0) return;

        Actor new_leader = null;
        if (asset.find_new_leader != null)
        {
            new_leader = asset.find_new_leader.Invoke(city as AW_City, this);
        }
        else if (_prev_leader_position == null)
        {
            new_leader = units.GetRandom();
        }
        else
        {
            var min_dist = 0f;
            var members = units.getSimpleList();
            foreach (Actor actor2 in members.Where(x => x.isAlive()))
            {
                var dist = Toolbox.DistTile(actor2.currentTile, _prev_leader_position);

                if (new_leader != null && !(dist < min_dist)) continue;

                new_leader = actor2;
                min_dist = dist;
            }
        }

        if (new_leader != null) setGroupLeader(new_leader);
    }
}