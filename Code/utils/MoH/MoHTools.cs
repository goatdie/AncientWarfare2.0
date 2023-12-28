using Figurebox.core;
namespace Figurebox.Utils.MoH;

public class MoHTools
{
    /// <summary>
    ///     当前天命国家
    /// </summary>
    public static AW_Kingdom MoHKingdom { get; private set; }
    /// <summary>
    ///     当前天命国家是否存在
    /// </summary>
    public static bool ExistMoHKingdom => MoHKingdom != null && MoHKingdom.isAlive();
    /// <summary>
    ///     从存档中读取并直接缓存国家
    /// </summary>
    public static void SetMoHKingdom(AW_Kingdom kingdom)
    {
        MoHKingdom = kingdom;
    }
    public static bool IsMoHKingdom(AW_Kingdom kingdom)
    {
       return MoHKingdom == kingdom;
    }
    internal static void LoadFromSave()
    {

    }
}