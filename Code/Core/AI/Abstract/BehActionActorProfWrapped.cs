using System;
using System.Collections.Generic;
using ai.behaviours;
using AncientWarfare.Core.Extensions;
using HarmonyLib;

namespace AncientWarfare.Core.AI.Abstract;

public abstract class BehActionActorProfWrapped : BehaviourActionActor, IProfessionalBeh
{
    private static readonly HashSet<Type> _patched_types = new();

    protected BehActionActorProfWrapped(bool auto_add_exp = true)
    {
        if (!auto_add_exp) return;
        Type type = GetType();
        if (type.IsAbstract) return;
        if (!_patched_types.Add(type)) return;

        new Harmony(type.FullName).Patch(AccessTools.Method(type, nameof(execute)),
                                         postfix: new HarmonyMethod(
                                             AccessTools.Method(typeof(BehActionActorProfWrapped),
                                                                nameof(Postfix_execute),
                                                                generics: new[] { GetType() })));
    }

    public abstract HashSet<string>         ProfessionRequired           { get; }
    public abstract Dictionary<string, int> ProfessionExpGivenPerExecute { get; }

    public void GiveExp(Actor actor)
    {
        if (ProfessionExpGivenPerExecute == null) return;
        foreach (var item in ProfessionExpGivenPerExecute) actor.IncreaseProfessionExp(item.Key, item.Value);
    }

    private static void Postfix_execute<T>(T __instance, Actor pObject) where T : IProfessionalBeh
    {
        __instance.GiveExp(pObject);
    }

    public override bool errorsFound(Actor pObject)
    {
        if (ProfessionRequired?.Count > 0 && !pObject.OverlapProfessions(ProfessionRequired)) return true;

        return base.errorsFound(pObject);
    }
}