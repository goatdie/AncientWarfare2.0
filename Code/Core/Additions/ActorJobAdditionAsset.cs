using System.Collections.Generic;
using ai.behaviours;
using AncientWarfare.Core.Extensions;

namespace AncientWarfare.Core.Additions;

public class ActorJobAdditionAsset
{
    private readonly ActorJob        _asset;
    private readonly HashSet<string> _professions_about       = new();
    private readonly HashSet<string> _techs_required          = new();
    private          bool            _dirty                   = true;
    private          bool            _techs_required_accurate = true;

    public ActorJobAdditionAsset(string id)
    {
        _asset = AssetManager.job_actor.get(id);
        Update();
    }

    private void Update()
    {
        if (!_dirty) return;
        _dirty = false;
        _techs_required.Clear();
        _professions_about.Clear();
        _techs_required_accurate = true;

        foreach (var container in _asset.tasks)
        {
            BehaviourTaskActor task = AssetManager.tasks_actor.get(container.id);
            ActorTaskAdditionAsset addition_asset = task.GetAdditionAsset();

            _techs_required.UnionWith(addition_asset.GetTechsRequired());
            _professions_about.UnionWith(addition_asset.GetProfessionsAbout());
            _techs_required_accurate &= addition_asset.IsTechsRequiredAccurate();
        }
    }

    public void SetDirty()
    {
        _dirty = true;
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