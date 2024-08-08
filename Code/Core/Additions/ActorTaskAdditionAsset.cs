using System.Collections.Generic;
using ai.behaviours;
using AncientWarfare.Core.AI.Abstract;

namespace AncientWarfare.Core.Additions;

public class ActorTaskAdditionAsset
{
    private readonly BehaviourTaskActor _asset;
    private readonly HashSet<string>    _professions_about       = new();
    private readonly HashSet<string>    _techs_required          = new();
    private          bool               _dirty                   = true;
    private          bool               _techs_required_accurate = true;

    public ActorTaskAdditionAsset(string id)
    {
        _asset = AssetManager.tasks_actor.get(id);
        Update();
    }

    private void Update()
    {
        if (!_dirty) return;
        _dirty = false;
        _techs_required.Clear();
        _professions_about.Clear();
        _techs_required_accurate = true;

        foreach (BehaviourActionActor beh in _asset.list)
        {
            if (beh is not BehActionActorExtended beh_extended) continue;

            if (beh_extended.TechRequired != null) _techs_required.UnionWith(beh_extended.TechRequired);

            if (beh_extended.ExpGiven != null) _professions_about.UnionWith(beh_extended.ExpGiven.Keys);

            _techs_required_accurate &= beh_extended.TechRequiredAccurate;
        }
    }

    public HashSet<string> GetTechsRequired()
    {
        Update();
        return _techs_required;
    }

    public HashSet<string> GetProfessionsAbout()
    {
        Update();
        return _professions_about;
    }

    public bool IsTechsRequiredAccurate()
    {
        Update();
        return _techs_required_accurate;
    }
}