using System.Linq;
using ai.behaviours;
using Figurebox.constants;
using Figurebox.utils;
using NeoModLoader.api.attributes;

namespace Figurebox.ai.behaviours.city;

public class BehProduceSlaves : CityBehProduceUnit
{
    [Hotfixable]
    public override BehResult execute(City pCity)
    {
        if (!world.worldLaws.world_law_civ_babies.boolVal) return BehResult.Stop;
        if (!pCity.hasAnyFood()) return BehResult.Stop;
        if (!isCityCanProduceUnits(pCity)) return BehResult.Stop;
        unitProduced = false;
        findPossibleParents(pCity);
        if (_possibleParents.Count == 0) return BehResult.Stop;
        var maxExclusive = pCity.status.population / 7 + 1;
        var num = Toolbox.randomInt(1, maxExclusive);
        if (DebugConfig.isOn(DebugOption.CityFastPopGrowth) && num < 100) num = 100;
        var i = 0;
        while (i < num && _possibleParents.Count != 0 && isCityCanProduceUnits(pCity))
        {
            tryToProduceUnit(pCity);
            i++;
            Main.LogInfo($"Try to produce slave at {i} in {pCity.data.id}");
        }

        _possibleParents.Clear();
        return unitProduced ? BehResult.Continue : BehResult.Stop;
    }

    [Hotfixable]
    private new void findPossibleParents(City pCity)
    {
        _possibleParents.Clear();
        if (pCity.professionsDict == null ||
            !pCity.professionsDict.TryGetValue(AWUnitProfession.Slave.C(), out var list)) return;
        var num = world.getCurWorldTime() / 5.0;
        _possibleParents.AddRange(list.Where(actor =>
            actor.isAlive() && actor.stats[S.fertility] > 0f && num - actor.data.had_child_timeout / 5.0 >= 8.0));

        pCity.professionsDict.TryGetValue(UnitProfession.Unit, out list);
        if (list != null)
            _possibleParents.AddRange(list.Where(actor =>
                actor.isAlive() && !actor.hasClan() && actor.stats[S.fertility] > 0f &&
                num - actor.data.had_child_timeout / 5.0 >= 8.0));

        _possibleParents.Shuffle();
    }
}