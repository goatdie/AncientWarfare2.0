using System.Collections.Generic;

namespace AncientWarfare.Core.Quest;

public delegate void QuestMergeDelegate(QuestInst source, QuestInst repeat_meet);

public class QuestAsset : Asset
{
    public List<string>       allow_jobs = new();
    public bool               disposable;
    public QuestMergeDelegate merge_action_when_repeat;
    public bool multitable = true;
    public QuestTypeAsset     type;
}