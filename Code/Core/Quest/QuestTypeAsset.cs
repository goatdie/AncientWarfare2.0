using System.Collections.Generic;
using AncientWarfare.Core.Force;

namespace AncientWarfare.Core.Quest;

public delegate void QuestInitDelegate(QuestInst quest, LowBaseForce owner, Dictionary<string, object> setting);

public delegate bool QuestUpdateDelegate(QuestInst quest, LowBaseForce owner);

public class QuestTypeAsset : Asset
{
    public QuestInitDelegate   InitDelegate;
    public QuestUpdateDelegate UpdateDelegate;
}