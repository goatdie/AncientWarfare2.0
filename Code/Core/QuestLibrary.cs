using AncientWarfare.Abstracts;
using AncientWarfare.Attributes;
using AncientWarfare.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AncientWarfare.Core
{
    [ManagerInitializeAfter(typeof(QuestType))]
    public class QuestLibrary : AW_AssetLibrary<QuestAsset, QuestLibrary>, IManager
    {
        public static readonly QuestAsset food_base_collect;
        public Quest CreateResourceCollectQuest(string resource_id, int target_count)
        {
            return null;
        }
        public override void init()
        {
            add(new() { id = nameof(food_base_collect) });
            t.allow_jobs.AddRange("", "");
        }
        public void Initialize()
        {
            init();
            id = "aw_quests";
        }
    }
}
