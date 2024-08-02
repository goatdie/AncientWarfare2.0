using System.Collections.Generic;
using AncientWarfare.Core.Force;

namespace AncientWarfare.Core.Quest;

public class QuestInst
{
    public readonly QuestAsset     asset;
    public readonly BaseSystemData Data;
    public readonly LowBaseForce   owner;

    public QuestInst(QuestAsset asset, LowBaseForce owner, Dictionary<string, object> setting = null)
    {
        this.asset = asset;
        this.owner = owner;
        Data = new BaseSystemData();
        Data.id = asset.id;
        Data.created_time = World.world.getCreationTime();
        asset.type.InitDelegate?.Invoke(this, this.owner, setting ?? new Dictionary<string, object>());
    }

    public bool Finished { get; private set; }
    public bool CanTake  { get; private set; } = true;

    public bool Disposable => asset.disposable;
    public bool Active     { get; protected set; }

    public void MarkFinished()
    {
        Finished = true;
    }

    public void Take()
    {
        if (!asset.multitable) CanTake = false;
    }

    public void UpdateProgress()
    {
        Active = asset.type.UpdateDelegate?.Invoke(this, owner) ?? true;
        if (!Active && Disposable) Finished = true;

        if (!Active) CanTake = true;
    }
}