using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AncientWarfare.Core
{
    public class Quest
    {
        public readonly BaseSystemData Data;
        public readonly QuestAsset asset;
        public Quest(QuestAsset asset)
        {
            this.asset = asset;
            Data = new();
            Data.id = asset.id;
            Data.created_time = World.world.getCreationTime();
        }
        public void UpdateProgress()
        {
            asset.update_action?.Invoke(this);
        }
    }
}
