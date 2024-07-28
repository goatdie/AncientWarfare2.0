using ai.behaviours;
using AncientWarfare.Core.Extensions;
using AncientWarfare.Core.Force;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AncientWarfare.Core.AI.ActorBehs
{
    public class BehTribeSubmitResources : BehTribe
    {
        public override BehResult execute(Actor pObject)
        {
            pObject.SubmitInventoryResourcesToTribe();
            return BehResult.Continue;
        }
    }
}
