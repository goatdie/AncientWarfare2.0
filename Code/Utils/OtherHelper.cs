using System.Collections.Generic;

namespace AncientWarfare.Utils
{
    internal static class OtherHelper
    {
        public static void AddRange<T>(this List<T> list, params T[] values)
        {
            list.AddRange(values);
        }

        public static void Expand<T>(this List<T> list, params T[] values)
        {
            list.AddRange(values);
        }
    }
}