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
    }
}
