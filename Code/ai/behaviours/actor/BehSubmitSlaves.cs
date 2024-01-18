using ai.behaviours;
using Figurebox.constants;
using Figurebox.Utils;
using Figurebox.Utils.extensions;
using NeoModLoader.api.attributes;

namespace Figurebox.ai.behaviours.actor;

public class BehSubmitSlaves : BehCity
{
    [Hotfixable]
    public override BehResult execute(Actor pObject)
    {
        Main.LogInfo($"{pObject.getName()} 提交奴隶中....");
        var caughtSlaves = pObject.data.ReadObj<string[]>(AWDataS.caught_slaves);
        if (caughtSlaves == null) return BehResult.Continue;

        foreach (var slaveId in caughtSlaves)
        {
            var slave = World.world.units.get(slaveId);
            if (slave == null) continue;
            if (!slave.hasTrait(AWS.slave)) continue;

            slave.setProfession(AWUnitProfession.Slave.C());
            Main.LogInfo($"{pObject.getName()} 提交奴隶 {slave.getName()} 至 {pObject.city.getCityName()}");
        }

        pObject.data.WriteObj<string[]>(AWDataS.caught_slaves, null);

        return base.execute(pObject);
    }
}