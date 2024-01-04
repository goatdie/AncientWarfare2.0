using System.Collections.Generic;
using Figurebox.constants;

namespace Figurebox.ai;

public class CitizenJobs
{
    public static CitizenJobAsset slave_catcher;

    private static readonly List<CitizenJobAsset> _list = new();

    internal static void init()
    {
        slave_catcher = add(new CitizenJobAsset
        {
            id = AWS.slave_catcher,
            priority = 5,
            ok_for_king = false,
            ok_for_leader = false,
            debug_option = DebugOption.CitizenJobAttacker
        });


        post_init();
    }

    private static CitizenJobAsset add(CitizenJobAsset pAsset)
    {
        AssetManager.citizen_job_library.add(pAsset);
        _list.Add(pAsset);
        return pAsset;
    }

    private static void post_init()
    {
        foreach (var item in _list)
            if (item.common_job)
            {
                if (item.priority_no_food > 0) AssetManager.citizen_job_library.list_priority_high_food.Add(item);
                if (item.priority > 0)
                    AssetManager.citizen_job_library.list_priority_high.Add(item);
                else
                    AssetManager.citizen_job_library.list_priority_normal.Add(item);
                item.unit_job_default = item.id;
            }
    }
}