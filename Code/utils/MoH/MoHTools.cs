using Figurebox.core;
namespace Figurebox.Utils.MoH;

public class MoHTools
{
    public static AW_Kingdom MoHKingdom { get; private set; }
    public static bool ExistMoHKingdom => MoHKingdom != null && MoHKingdom.isAlive();
    /// <summary>
    ///     从存档中读取并直接缓存国家
    /// </summary>
    internal static void LoadFromSave()
    {

    }
}