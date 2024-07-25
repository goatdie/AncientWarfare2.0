using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AncientWarfare.Core.Extensions
{
    public static partial class ActorExtension
    {
        public static bool IsValid(this Actor actor)
        {
            return actor != null && actor.isAlive();
        }
        public static ActorAdditionData GetAdditionData(this Actor actor)
        {
            return ActorAdditionDataManager.Get(actor.data.id);
        }
    }
}
