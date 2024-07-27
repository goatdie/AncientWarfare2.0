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
    public class BehTribeFindBuilding : BehTribe
    {
        private string[] types;
        public BehTribeFindBuilding(string neccessary_type, params string[] types)
        {
            this.types = new List<string>(types).AddItem(neccessary_type).ToArray();
        }
        public override BehResult execute(Actor pObject)
        {
            var type = types.GetRandom();
            // TODO: 改为更通用的方法，目前只能找到资源建筑
            pObject.beh_building_target = pObject.FindAvailableResourceBuildingInTribe(type);

            return BehResult.Continue;
        }
    }
}
