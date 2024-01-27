using System.Linq;
using ai.behaviours;
using Figurebox.constants;
using Figurebox.Utils;
using NeoModLoader.api.attributes;
using System.Collections.Generic;

namespace Figurebox.ai.behaviours.city;

public class BehProduceNobles : CityBehProduceUnit
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
        var maxExclusive = pCity.status.population / 4 + 1; // 增加了尝试产生单位的次数
        var num = Toolbox.randomInt(1, maxExclusive * 4); // 增加了随机数的上限
        if (DebugConfig.isOn(DebugOption.CityFastPopGrowth) && num < 100) num = 100;
        var i = 0;
        while (i < num && _possibleParents.Count != 0 && isCityCanProduceUnits(pCity))
        {
            tryToProduceUnit(pCity);
            i++;
            Main.LogInfo($"Try to produce noble at {i} in {pCity.data.id}");
        }

        _possibleParents.Clear();
        return unitProduced ? BehResult.Continue : BehResult.Stop;
    }

    [Hotfixable]
    private new void findPossibleParents(City pCity)
    {
        _possibleParents.Clear();
        List<Actor> simpleList = pCity.units.getSimpleList();
        foreach (var actor in simpleList)
        {
            if (actor.isAlive() && actor.stats[S.fertility] > 0f && actor.hasClan())
            {
                _possibleParents.Add(actor);
                //Main.LogInfo($"Try to produce noble   {actor.getName()}");
            }
        }

        _possibleParents.Shuffle();
    }

}