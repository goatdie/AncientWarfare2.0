using System.Collections.Generic;
using ai.behaviours;
using Figurebox.constants;
using Figurebox.Utils;
using Figurebox.Utils.extensions;
using NeoModLoader.api.attributes;

namespace Figurebox.ai.behaviours.actor;

public class BehCatchTargetAsSlave : BehaviourActionActor
{
    [Hotfixable]
    public override BehResult execute(Actor pObject)
    {
        if (pObject.beh_actor_target == null || !pObject.beh_actor_target.isAlive() ||
            !pObject.beh_actor_target.isActor())
        {
            Main.LogInfo(
                $"奴隶捕捉目标不存在或已死亡 null: {pObject.beh_actor_target == null}, alive: {pObject.beh_actor_target?.isAlive() ?? false}, actor: {pObject.beh_actor_target?.isActor() ?? false}");
            return BehResult.RestartTask;
        }

        var pTarget = pObject.beh_actor_target.a;


        Main.LogInfo(
            $"存在奴隶捕捉目标 {pTarget.data.id}({pTarget.getName()}) 血量: {pTarget.data.health} -> {pTarget.stats[S.health] * 0.4f}/{pTarget.stats[S.health]}");

        if (pTarget.data.health > pTarget.stats[S.health] * BehaviourConst.SlaveCaptureHealthThreshold)
        {
            return BehResult.RestartTask;
        }

        pObject.beh_actor_target = null;

        pTarget.addTrait(AWS.slave);
        pTarget.data.set(AWDataS.slave_caught_by, pObject.data.id);
        pTarget.city?.removeUnit(pTarget);
        pObject.city?.addNewUnit(pTarget);
        pTarget.setProfession(AWUnitProfession.Slave.C());
        pTarget.data.favorite = true;

        var slaves = pObject.data.ReadObj<List<string>>(AWDataS.caught_slaves);
        if (slaves == null) slaves = new List<string>();
        slaves.Add(pTarget.data.id);
        pObject.data.WriteObj(AWDataS.caught_slaves, slaves);

        return BehResult.Continue;
    }
}