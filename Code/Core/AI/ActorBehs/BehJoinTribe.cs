using ai.behaviours;
using AncientWarfare.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AncientWarfare.Core.AI.ActorBehs
{
    public class BehJoinTribe : BehaviourActionActor
    {
        public override BehResult execute(Actor pObject)
        {
            var tribe = pObject.currentTile.zone.GetTribe();
            // 当前位置无部落
            if (tribe == null) return BehResult.Stop;
            // 部落已加入
            if (pObject.IsMemberOf(tribe)) return BehResult.Stop;
            if (tribe.IsFull()) return BehResult.Stop;

            pObject.JoinTribe(tribe);

            return BehResult.Continue;
        }
    }
}
