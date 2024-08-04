using System;
using System.Collections.Generic;
using System.Linq;
using AncientWarfare.Core.Force;

namespace AncientWarfare.Core.Quest;

public class QuestInst
{
    public readonly QuestAsset     asset;
    public readonly BaseSystemData Data;
    public readonly LowBaseForce   owner;
    private float _restart_timer;

    public QuestInst(QuestAsset asset, LowBaseForce owner, Dictionary<string, object> setting = null)
    {
        this.asset = asset;
        this.owner = owner;
        _restart_timer = this.asset.restart_timeout;
        Data = new BaseSystemData();
        Data.id = asset.id;
        Data.created_time = World.world.getCreationTime();
        setting ??= new Dictionary<string, object>();
        foreach (var item in asset.given_setting.Where(item => !setting.ContainsKey(item.Key)))
            setting[item.Key] = item.Value;

        asset.type.InitDelegate?.Invoke(this, this.owner, setting);
    }

    public bool Finished { get; private set; }
    public bool CanTake  { get; private set; } = true;

    public bool   Disposable => asset.disposable;
    public bool   Active     { get; protected set; }
    public string UID        { get; private set; } = Guid.NewGuid().ToString();

    public void MarkFinished()
    {
        Finished = true;
    }

    public void MarkRestart()
    {
        Finished = false;
        CanTake = true;
    }

    public void Take()
    {
        if (!asset.multitable) CanTake = false;
    }

    public void UpdateProgress(float elapsed)
    {
        if (_restart_timer <= 0)
        {
            CanTake = true;
            _restart_timer = asset.restart_timeout;
        }

        _restart_timer -= elapsed;
        Active = asset.type.UpdateDelegate?.Invoke(this, owner) ?? true;
        if (!Active && Disposable) Finished = true;

        if (!Active) CanTake = true;
    }
}