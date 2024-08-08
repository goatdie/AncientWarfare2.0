using System.Collections.Generic;
using ai.behaviours;
using AncientWarfare.Core.Extensions;

namespace AncientWarfare.Core.AI.Abstract;

public abstract class BehActionActorExtended : BehaviourActionActor
{
    private readonly BehResult _res_on_tech_not_satisfied;

    protected BehActionActorExtended(BehResult res_on_tech_not_satisfied)
    {
        _res_on_tech_not_satisfied = res_on_tech_not_satisfied;
    }

    public abstract Dictionary<string, int> ExpGiven             { get; }
    public abstract List<string>            TechRequired         { get; }
    public virtual  bool                    TechRequiredAccurate => true;

    public sealed override BehResult execute(Actor pObject)
    {
        if (TechRequired != null && !pObject.OverlapTechs(TechRequired)) return _res_on_tech_not_satisfied;

        (BehResult res, var add_exp_or_not) = OnExecute(pObject);
        if (add_exp_or_not && ExpGiven != null)
            foreach (var item in ExpGiven)
                pObject.IncreaseProfessionExp(item.Key, item.Value);

        return res;
    }

    public abstract (BehResult, bool) OnExecute(Actor actor);
}