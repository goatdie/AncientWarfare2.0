using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ai.behaviours;
using Figurebox.abstracts;
using Figurebox.ai.behaviours.kingdom;

namespace Figurebox.ai;

public class KingdomTaskLibrary : ExtendedLibrary<BehaviourTaskKingdom>
{
    protected override void init()
    {
        var lib = AssetManager.tasks_kingdom;
        var task = lib.get("do_checks");

        var original_king_check = task.list.FindIndex(t => t is KingdomBehCheckKing);
        if (original_king_check >= 0)
        {
            task.addBeh(new KingdomBehCheckMOH());
            //task.list[original_king_check]=new KingdomBehCheckMOH();
        }
        else
        {
            task.addBeh(new KingdomBehCheckMOH());
        }

        task.addBeh(new KingdomBehCheckHeir());
        task.addBeh(new KingdomBehCheckNewCapital());
        task.addBeh(new KingdomBehCheckPromotion());
    }
}