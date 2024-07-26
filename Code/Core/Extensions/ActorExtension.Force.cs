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
        public static void JoinForceOneside(this Actor actor, LowBaseForce force)
        {
            var data = actor.GetAdditionData();

            data.Forces.Add(force.BaseData.id);
        }
        public static void LeaveForceOneside(this Actor actor, LowBaseForce force)
        {
            var data = actor.GetAdditionData();

            data.Forces.Remove(force.BaseData.id);
        }
        public static Tribe GetTribe(this Actor actor)
        {
            var data = actor.GetAdditionData();
            var tribe = data.Forces.Select(ForceManager.GetForce<Tribe>).FirstOrDefault(x => x!=null);
            return tribe;
        }
    }
}
