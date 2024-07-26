using AncientWarfare.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AncientWarfare.Core
{
    public class QuestLibrary : AW_AssetLibrary<QuestAsset, QuestLibrary>
    {
        public Quest CreateResourceQuest(string resource_id)
        {
            throw new NotImplementedException();
        }
    }
}
