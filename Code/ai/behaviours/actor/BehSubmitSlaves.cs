using ai.behaviours;
using Figurebox.constants;
using Figurebox.Utils.extensions;

namespace Figurebox.ai.behaviours.actor;

public class BehSubmitSlaves : BehCity
{
    public override BehResult execute(Actor pObject)
    {
        var caughtSlaves = pObject.data.ReadObj<string[]>(AWDataS.caught_slaves);
        if (caughtSlaves == null) return BehResult.Continue;

        foreach (var slaveId in caughtSlaves)
        {
            var slave = World.world.units.get(slaveId);
            if (slave == null) continue;
            if (!slave.hasTrait(AWS.slave)) continue;

            Main.LogInfo($"{pObject.getName()} 提交奴隶 {slave.getName()}");
        }

        pObject.data.WriteObj<string[]>(AWDataS.caught_slaves, null);

        return base.execute(pObject);
    }
}