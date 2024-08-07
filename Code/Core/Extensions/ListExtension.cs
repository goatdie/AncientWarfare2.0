using System;
using System.Collections.Generic;

namespace AncientWarfare.Core.Extensions;

public static class ListExtension
{
    private static Random rnd = new();

    public static T GetRandom<T>(this List<T> list, int first_k)
    {
        return list[ListExtensions.rnd.Next(0, Math.Min(list.Count, first_k))];
    }
}