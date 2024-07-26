using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AncientWarfare.Core
{
    public class Quest
    {
        public readonly QuestAsset asset;
        public Quest(QuestAsset asset)
        {
            this.asset = asset;
        }
    }
}
