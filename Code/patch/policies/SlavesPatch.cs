using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using ai.behaviours;
using Figurebox.attributes;
using Figurebox.constants;
using Figurebox.core;
using Figurebox.Utils;
using HarmonyLib;
using NeoModLoader.api.attributes;

namespace Figurebox.patch.policies;

/// <summary>
///     这个类用于实现奴隶俘获, 奴隶控制, 奴隶暴乱等
/// </summary>
internal class SlavesPatch : AutoPatch
{
    [Hotfixable]
    [MethodReplace(typeof(CityBehProduceUnit), nameof(CityBehProduceUnit.findPossibleParents))]
    private static void findPossibleParents(CityBehProduceUnit pAction, City pCity)
    {
        pAction._possibleParents.Clear();

        var simpleList = pCity.units.getSimpleList();
        var num = BehaviourActionBase<City>.world.getCurWorldTime() / 5.0;

        pAction._possibleParents.AddRange(simpleList
                                          .Where(actor => !actor.isProfession(AWUnitProfession.Slave.C()) &&
                                                          !actor.hasTrait("slave")).Where(actor =>
                                                       actor.isAlive() && actor.stats[S.fertility] > 0f &&
                                                       num - actor.data.had_child_timeout / 5.0    >= 8.0));

        pAction._possibleParents.Shuffle();
    }

    [HarmonyTranspiler]
    [HarmonyPatch(typeof(Actor), nameof(Actor.consumeCityFoodItem))]
    private static IEnumerable<CodeInstruction> updateHunger(IEnumerable<CodeInstruction> instructions)
    {
        var codes = instructions.ToList();

        var mood_change_index = HarmonyTools.FindCodeSnippet(codes, new List<CodeInstruction>
        {
            new(OpCodes.Ret),
            new(OpCodes.Ldarg_0),
            new(OpCodes.Ldfld,
                AccessTools.Field(typeof(Actor), nameof(Actor.data))),
            new(OpCodes.Ldfld,
                AccessTools.Field(typeof(ActorData), nameof(ActorData.mood))),
            new(OpCodes.Ldstr, "happy")
        });

        var prepend_codes = new List<CodeInstruction>();
        prepend_codes.Add(new CodeInstruction(OpCodes.Ldarg_0));
        prepend_codes.Add(new CodeInstruction(OpCodes.Ldfld,
                                              AccessTools.Field(typeof(Actor), nameof(Actor.city))));
        prepend_codes.Add(new CodeInstruction(OpCodes.Ldarg_0));
        if (mood_change_index == -1)
        {
            prepend_codes.Add(new CodeInstruction(OpCodes.Call,
                                                  AccessTools.Method(typeof(SlavesPatch), nameof(HasFoodFor))));
            Main.LogWarning("无法找到代码片段, 奴隶缺少食物将不会影响心情", true);
        }
        else
        {
            prepend_codes.Add(new CodeInstruction(OpCodes.Callvirt,
                                                  AccessTools.Method(typeof(SlavesPatch), nameof(HasFoodFor))));
            Label label;
            if (codes[mood_change_index + 1].labels.Count == 0)
            {
                label = new Label();
                codes[mood_change_index + 1].labels.Add(label);
            }

            label = codes[mood_change_index + 1].labels[0];
            prepend_codes.Add(new CodeInstruction(OpCodes.Brfalse, label));
        }

        codes.InsertRange(0, prepend_codes);
        return codes.AsEnumerable();
    }

    private static bool HasFoodFor(City pCity, Actor pActor)
    {
        if (!pActor.isProfession(AWUnitProfession.Slave.C()) && !pActor.hasTrait("slave")) return true;

        var city = (AW_City)pCity;

        if (city.food_count_for_slaves_this_year <= 0) return false;
        city.food_count_for_slaves_this_year--;

        return true;
    }
}