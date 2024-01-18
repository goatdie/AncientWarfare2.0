using System.Linq;
using ai.behaviours;
using Figurebox.attributes;
using Figurebox.constants;
using Figurebox.Utils;
using NeoModLoader.api.attributes;

namespace Figurebox.patch.policies;

/// <summary>
///     这个类用于实现奴隶俘获, 奴隶控制, 奴隶暴乱等
/// </summary>
internal class SlavesPatch : AutoPatch
{
    [Hotfixable]
    [MethodReplace(typeof(CityBehProduceUnit), nameof(CityBehProduceUnit.findPossibleParents))]
    public static void findPossibleParents(CityBehProduceUnit pAction, City pCity)
    {
        pAction._possibleParents.Clear();

        var simpleList = pCity.units.getSimpleList();
        var num = BehaviourActionBase<City>.world.getCurWorldTime() / 5.0;

        pAction._possibleParents.AddRange(simpleList
            .Where(actor => !actor.isProfession(AWUnitProfession.Slave.C()) && !actor.hasTrait("slave")).Where(actor =>
                actor.isAlive() && actor.stats[S.fertility] > 0f && num - actor.data.had_child_timeout / 5.0 >= 8.0));

        pAction._possibleParents.Shuffle();
    }
}