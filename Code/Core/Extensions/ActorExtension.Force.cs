using AncientWarfare.Core.Force;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AncientWarfare.Core.Extensions
{
    public static partial class ActorExtension
    {
        public static bool IsMemberOf(this Actor actor, LowBaseForce force)
        {
            return actor.GetAdditionData().Forces.Contains(force.BaseData.id);
        }
        public static void JoinTribe(this Actor actor, Tribe tribe)
        {
            var data = actor.GetAdditionData();

            data.Forces.Add(tribe.BaseData.id);
            tribe.AddNewActor(actor);
        }
    }
}
