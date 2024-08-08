using System.Linq;
using AncientWarfare.Core.Additions;
using AncientWarfare.Core.Force;

namespace AncientWarfare.Core.Extensions
{
    public static partial class ActorExtension
    {
        public static bool IsMemberOf(this Actor actor, LowBaseForce force)
        {
            return actor.GetAdditionData(true)?.Forces.Contains(force.BaseData.id) ?? false;
        }

        public static void JoinForceOneside(this Actor actor, LowBaseForce force)
        {
            var data = actor.GetAdditionData();

            data.Forces.Add(force.BaseData.id);
        }

        public static void LeaveForceOneside(this Actor actor, LowBaseForce force)
        {
            ActorAdditionData data = actor.GetAdditionData(true);

            data?.Forces.Remove(force.BaseData.id);
        }

        public static Tribe GetTribe(this Actor actor)
        {
            ActorAdditionData data = actor.GetAdditionData(true);
            return data?.Forces.Select(ForceManager.GetForce<Tribe>).FirstOrDefault(x => x != null);
        }
    }
}