using System.Linq;
using ai.behaviours;
using Figurebox.constants;
using Figurebox.content;
using Figurebox.core;
using Figurebox.utils;

namespace Figurebox.ai.behaviours.city;

/// <summary>
///     使用奴隶组织奴隶军
/// </summary>
public class BehCheckSlaveArmy : BehaviourActionCity
{
    public override BehResult execute(City pObject)
    {
        var city = (AW_City)pObject;
        if (city.groups.TryGetValue(UnitGroupTypeLibrary.slaves.id, out AW_UnitGroup group))
        {
            findWarriorsForGroup(city, group);
            return BehResult.Continue;
        }

        if (city.professionsDict.ContainsKey(AWUnitProfession.Slave.C()) &&
            city.professionsDict[AWUnitProfession.Slave.C()].Count > 0)
        {
            city.CreateGroup(UnitGroupTypeLibrary.slaves);
            findWarriorsForGroup(city, city.groups[UnitGroupTypeLibrary.slaves.id]);
            return BehResult.Continue;
        }

        return base.execute(pObject);
    }

    private void findWarriorsForGroup(AW_City pCity, AW_UnitGroup pGroup)
    {
        var left_count = (int)(pGroup.asset.base_max_count * pCity.getArmyMaxTotalPercentage()) - pGroup.units.Count;
        if (left_count <= 0) return;

        foreach (Actor unit in pCity.professionsDict[AWUnitProfession.Slave.C()].Where(unit => unit.unit_group == null)
                                    .Where(unit => unit.citizen_job                                            == null))
        {
            pGroup.addUnit(unit);
            unit.setCitizenJob(CitizenJobs.slave_warrior);
            left_count--;
            if (left_count <= 0) break;
        }
    }
}