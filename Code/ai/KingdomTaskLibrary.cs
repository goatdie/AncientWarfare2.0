using Figurebox.abstracts;
using Figurebox.ai.behaviours.kingdom;

namespace Figurebox.ai;

public class KingdomTaskLibrary : ExtendedLibrary<BehaviourTaskKingdom>
{
    protected override void init()
    {
        var lib = AssetManager.tasks_kingdom;
        var task = lib.get("do_checks");

        task.addBeh(new KingdomBehCheckHeir());
        task.addBeh(new KingdomBehCheckNewCapital());
        task.addBeh(new KingdomBehCheckPromotion());
    }
}