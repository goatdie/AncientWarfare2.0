using System.Collections.Generic;
using System.Linq;
using ai.behaviours;
using AncientWarfare.Core.AI.Abstract;
using AncientWarfare.Core.Extensions;
using HarmonyLib;

namespace AncientWarfare.Core.AI.ActorBehs;

public class BehTribeFindBuilding : BehTribe
{
    private string[] types;

    public BehTribeFindBuilding(string neccessary_type, params string[] types)
    {
        this.types = new List<string>(types).AddItem(neccessary_type).ToArray();
    }

    public override BehResult execute(Actor pObject)
    {
        var type = types.GetRandom();
        pObject.beh_building_target = pObject.FindBuildingTarget(type);

        if (IsResourceBuilding(type) && pObject.beh_building_target == null)
            pObject.GetTribe().NewExpandQuest(type);

        return BehResult.Continue;
    }

    private static bool IsResourceBuilding(string type)
    {
        if (type == SB.type_flower  || type == SB.type_vegetation || type == SB.type_tree ||
            type == SB.type_mineral || type == SB.type_fruits)
            return true;

        return false;
    }
}