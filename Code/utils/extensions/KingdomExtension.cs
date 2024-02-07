using System.Runtime.CompilerServices;
using Figurebox.core;

namespace Figurebox.utils.extensions;

public static class KingdomExtension
{
    /// <summary>
    ///     将Kingdom简单转换为AW_Kingdom
    /// </summary>
    /// <param name="pKingdom"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AW_Kingdom AW(this Kingdom pKingdom)
    {
        return pKingdom as AW_Kingdom;
    }
}