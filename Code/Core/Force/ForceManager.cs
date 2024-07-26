﻿using AncientWarfare.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AncientWarfare.Core.Force
{
    public class ForceManager
    {
        public static ForceManager I { get; private set; } = new();
        public readonly TribeManager tribes;
        private readonly Dictionary<string, LowBaseForce> total_container = new();
        internal ForceManager()
        {
            tribes = new TribeManager(total_container);
        }
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
            if (force is not LowBaseForce)
            {
                Main.LogDebug("force is not LowBaseForce", true);
                return;
            }
            actor.JoinForceOneside(force as LowBaseForce);
            force.AddMemberOneside(actor);
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