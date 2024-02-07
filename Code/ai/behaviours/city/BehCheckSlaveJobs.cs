using System.Collections.Generic;
using ai.behaviours;
using Figurebox.constants;
using Figurebox.core;
using Figurebox.utils;
using UnityEngine;

namespace Figurebox.ai.behaviours.city;

/// <summary>
///     检查奴隶数量（少则分配奴隶捕手）, 并给奴隶分配工作
/// </summary>
public class BehCheckSlaveJobs : BehaviourActionCity
{
    public override BehResult execute(City pObject)
    {
        var city = (AW_City)pObject;

        city.professionsDict.TryGetValue(AWUnitProfession.Slave.C(), out var slaves);

        city.aw_status.slaves_current = slaves?.Count ?? 0;

        if (city.aw_status.slaves_current > 0) GiveSlavesTasks(city, slaves);

        FindSlaveCatchers(city);

        return base.execute(pObject);
    }

    private void FindSlaveCatchers(AW_City pCity)
    {
        if (!world.worldLaws.world_law_civ_army.boolVal || pCity.status.populationAdults <= 15) return;

        pCity.jobs.occupied.TryGetValue(CitizenJobs.slave_catcher, out var curr_catcher_nr);
        if (curr_catcher_nr >= PolicyConst.MAX_SLAVE_CATCHER_COUNT) return;

        var max_slave_catcher_nr = (int)Mathf.Min(1, Mathf.Max(3, pCity.status.populationAdults * 0.05f));
        if (curr_catcher_nr >= max_slave_catcher_nr) return;

        pCity.jobs.addToJob(CitizenJobs.slave_catcher, max_slave_catcher_nr - curr_catcher_nr);
    }

    private void GiveSlavesTasks(AW_City pCity, List<Actor> pSlaves)
    {
        foreach (var slave in pSlaves)
        {
            if (slave.citizen_job != null) continue;
            pCity.getCitizenJob(slave);
        }
    }
}