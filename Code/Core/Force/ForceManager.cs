using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AncientWarfare.Core.Extensions;

namespace AncientWarfare.Core.Force
{
    public class ForceManager
    {
        private readonly Dictionary<string, LowBaseForce> total_container = new();
        public readonly TribeManager tribes;

        internal ForceManager()
        {
            tribes = new TribeManager(total_container);
        }

        public static ForceManager I { get; } = new();

        internal void Update()
        {
            Parallel.ForEach(total_container.Values, force => force.Update());
        }

        public static T GetForce<T>(string id) where T : LowBaseForce
        {
            if (string.IsNullOrEmpty(id)) return null;
            return I.total_container.TryGetValue(id, out var force) ? (T)force : null;
        }

        internal string GetNewId()
        {
            string id = Guid.NewGuid().ToString();
            while (total_container.ContainsKey(id))
            {
                id = Guid.NewGuid().ToString();
            }

            return id;
        }

        public static void MakeJoinToForce(Actor actor, IHasMember force)
        {
            if (force is not LowBaseForce base_force)
            {
                Main.LogDebug("force is not LowBaseForce", true);
                return;
            }

            actor.JoinForceOneside(base_force);
            force.AddMemberOneside(actor);
        }

        public static void MakeJoinToForce(Building building, IHasBuilding force)
        {
            if (force is not LowBaseForce base_force)
            {
                Main.LogDebug("force is not LowBaseForce", true);
                return;
            }

            building.JoinForceOneside(base_force);
            force.AddBuildingOneside(building);
        }

        public static void MakeLeaveForce(Actor actor, IHasMember force)
        {
            if (force is not LowBaseForce)
            {
                Main.LogDebug("force is not LowBaseForce", true);
                return;
            }

            actor.LeaveForceOneside(force as LowBaseForce);
            force.RemoveMemberOneside(actor.data.id);
        }

        public static void MakeLeaveForce(Actor actor, string id)
        {
            var force = GetForce<LowBaseForce>(id);
            if (force == null)
            {
                Main.LogDebug($"force {id} is null", true);
                return;
            }

            actor.LeaveForceOneside(force);
            if (force is IHasMember has_member)
            {
                has_member.RemoveMemberOneside(actor.data.id);
            }
        }
    }
}