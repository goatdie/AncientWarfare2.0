﻿using ai.behaviours;
using AncientWarfare.Core.Extensions;
using AncientWarfare.Core.Force;

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

            ForceManager.MakeJoinToForce(pObject, tribe);

            return BehResult.Continue;
        }
    }
}