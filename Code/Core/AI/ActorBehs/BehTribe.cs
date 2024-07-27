using ai.behaviours;
using AncientWarfare.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AncientWarfare.Core.AI.ActorBehs
{
    public class BehTribe : BehaviourActionActor
    {
        public override bool errorsFound(Actor pObject)
        {
            if (base.errorsFound(pObject)) return true;
            var tribe = pObject.GetTribe();
            return tribe == null;
        }
    }
}
