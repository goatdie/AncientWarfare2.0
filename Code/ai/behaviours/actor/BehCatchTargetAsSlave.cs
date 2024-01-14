using System.Collections.Generic;
using ai.behaviours;
using Figurebox.constants;
using Figurebox.Utils;
using Figurebox.Utils.extensions;

namespace Figurebox.ai.behaviours.actor;

public class BehCatchTargetAsSlave : BehaviourActionActor
{
    public override BehResult execute(Actor pObject)
    {
        if (pObject.beh_actor_target == null || !pObject.beh_actor_target.isAlive() ||
            pObject.beh_actor_target.a == null) return BehResult.RestartTask;

        var pTarget = pObject.beh_actor_target.a;
        Main.LogInfo($"存在奴隶捕捉目标 {pTarget.data.id}({pTarget.getName()})");
        if (pTarget.data.health > pTarget.stats[S.health] * 0.4f)
        {
            pObject.setAttackTarget(pTarget);
            return BehResult.RestartTask;
        }

        pObject.clearAttackTarget();
        pObject.beh_actor_target = null;

        pTarget.addTrait(AWS.slave);
        pTarget.data.set(AWDataS.slave_caught_by, pObject.data.id);
        pTarget.city?.removeUnit(pTarget);
        pObject.city?.addNewUnit(pTarget);
        pTarget.setProfession(AWUnitProfession.Slave.C());
        Main.LogInfo($"捕获奴隶{pTarget.data.id}({pTarget.getName()})");
        pTarget.data.favorite = true;

        var slaves = pObject.data.ReadObj<List<string>>(AWDataS.caught_slaves);
        if (slaves == null) slaves = new List<string>();
        slaves.Add(pTarget.data.id);
        pObject.data.WriteObj(AWDataS.caught_slaves, slaves);

        return BehResult.Continue;
    }
}