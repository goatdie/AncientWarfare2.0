using System.Linq;
using Figurebox.attributes;
using Figurebox.utils;
using JetBrains.Annotations;

namespace Figurebox.core;

public class AW_UnitGroup : UnitGroup
{
    public AW_UnitGroupAsset asset;
    public AW_UnitGroupData  data;

    public AW_UnitGroup(City pCity, AW_UnitGroupAsset pAsset, AW_UnitGroupData pData) : base(pCity)
    {
        asset = pAsset;
        data = pData;
        data.city_id = pCity.data.id;
    }

    /// <summary>
    ///     切换到指定城市，如果该城市已经有该组，则不会切换；如果该城市已经有该组，但是 pMerge 为 true，则会将该组合并到该城市的该组中
    ///     <para>成员也会切换城市</para>
    /// </summary>
    /// <param name="pCity"></param>
    /// <param name="pMerge"></param>
    public void SetCity([NotNull] AW_City pCity, bool pMerge = false)
    {
        if (city == pCity) return;

        if (pCity.groups.TryGetValue(asset.id, out AW_UnitGroup old_group) && !pMerge) return;
        removeFromCurrentCitySimply();

        units.checkAddRemove();
        foreach (Actor unit in units)
        {
            city.removeUnit(unit);
            pCity.addNewUnit(unit);
        }

        if (old_group != null && old_group != this)
        {
            foreach (Actor unit in units) old_group.addUnit(unit);
        }
        else
        {
            pCity.groups.Add(asset.id, this);
            pCity.army ??= this;
            city = pCity;
            data.city_id = pCity.data.id;
        }
    }

    /// <summary>
    ///     从当前城市移除，这将导致该组不受任何城市控制，该组成员将会不属于原城市，但仍属于其国家
    /// </summary>
    public void RemoveFromCurrentCity()
    {
        if (city == null) return;

        units.checkAddRemove();
        foreach (Actor unit in units) city.removeUnit(unit, false);
        removeFromCurrentCitySimply();
    }

    private void removeFromCurrentCitySimply()
    {
        var aw_city = (AW_City)city;
        if (aw_city.army == this) aw_city.army = null;

        aw_city.groups.Remove(asset.id);

        if (aw_city.groups.Count > 0)
        {
            var max_army_count = 0;
            AW_UnitGroup max_army = null;
            foreach (AW_UnitGroup group in aw_city.groups.Values.Where(
                         group => group.asset.base_max_count > max_army_count))
            {
                max_army = group;
                max_army_count = group.asset.base_max_count;
            }

            aw_city.SetMainGroup(max_army);
        }

        city = null;
        data.city_id = "";
    }

    public void LoadData(AW_UnitGroupData pData)
    {
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

    [MethodReplace(nameof(UnitGroup.setGroupLeader))]
    public new void setGroupLeader(Actor pActor)
    {
        if (pActor == null && groupLeader != null)
            groupLeader.setGroupLeader(false);
        groupLeader = pActor;
        if (groupLeader == null)
            return;
        data.group_leader_id = groupLeader.data.id;
        groupLeader.setGroupLeader(true);
    }
}