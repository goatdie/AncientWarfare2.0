using System.Collections.Generic;

namespace AncientWarfare.Core.Quest;

public delegate bool QuestMergeDelegate(QuestInst source, QuestInst repeat_meet);

public class QuestAsset : Asset
{
    public List<string>               allow_jobs = new();
    public bool                       disposable;
    public Dictionary<string, object> given_setting = new();
    public QuestMergeDelegate         merge_action_when_repeat;
    public bool                       multitable      = true;
    public float                      restart_timeout = float.MaxValue;
    public QuestTypeAsset             type;
}