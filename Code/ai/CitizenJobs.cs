using System.Linq;
using Figurebox.abstracts;
using Figurebox.constants;

namespace Figurebox.ai;

public class CitizenJobs : ExtendedLibrary<CitizenJobAsset>
{
    public static CitizenJobAsset slave_catcher;

    protected override void init()
    {
        slave_catcher = add(new CitizenJobAsset
        {
            id = AWS.slave_catcher,
            priority = 5,
            ok_for_king = false,
            ok_for_leader = false,
            debug_option = DebugOption.CitizenJobAttacker,
            path_icon = "ui/policy/start_slaves"
        });


        post_init();
    }

    private void post_init()
    {
        foreach (var item in added_assets.Where(item => item.common_job))
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