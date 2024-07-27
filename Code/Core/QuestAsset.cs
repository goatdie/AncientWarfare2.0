using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AncientWarfare.Core
{
    public delegate void UpdateQuestProgressDelegate(Quest quest);
    public class QuestAsset : Asset
    {
        public QuestType type;
        public List<string> allow_jobs = new();
        public UpdateQuestProgressDelegate update_action; 
    }
}
