using AncientWarfare.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AncientWarfare.Attributes
{
    internal class ManagerInitializeAfterAttribute : Attribute
    {
        public readonly HashSet<Type> after_types = new();
        public ManagerInitializeAfterAttribute(params Type[] types)
        {
            foreach (var type in types)
            {
                if (!typeof(IManager).IsAssignableFrom(type))
                {
                    Main.LogDebug($"Type {type} is not a manager type.");
                    continue;
                }
                if (type.IsInterface)
                {
                    Main.LogDebug($"Type {type} is an interface type.");
                    continue;
                }
                after_types.Add(type);
            }
        }
    }
}
