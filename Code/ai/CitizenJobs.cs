using Figurebox.constants;
namespace Figurebox.ai;

public class CitizenJobs
{
    public static CitizenJobAsset slave_catcher;
    internal static void init()
    {
        slave_catcher = AssetManager.citizen_job_library.add(new CitizenJobAsset
        {
            id = AWS.slave_catcher,
            priority = 5,
            ok_for_king = false,
            ok_for_leader = false,
            debug_option = DebugOption.CitizenJobAttacker
        });


        post_init();
    }
    private static void post_init()
    {

    }
}