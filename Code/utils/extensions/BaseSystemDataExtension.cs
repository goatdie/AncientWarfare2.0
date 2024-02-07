using System.Runtime.CompilerServices;

namespace Figurebox.utils.extensions;

public static class BaseSystemDataExtension
{
    /// <summary>
    ///     以key为key, 将pObject JSON序列化后写入data
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteObj<T>(this BaseSystemData pData, string pKey, T pObject,
        bool pPrivateMembersIncluded = false)
    {
        if (pObject == null) pData.removeString(pKey);
        pData.set(pKey, GeneralHelper.ToJSON(pObject, pPrivateMembersIncluded));
    }

    /// <summary>
    ///     以key为key, 从data中读取JSON, 并反序列化为T, 若不存在则会返回default(T)
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ReadObj<T>(this BaseSystemData pData, string pKey, bool pPrivateMembersIncluded = false)
    {
        pData.get(pKey, out string obj_str);

        if (string.IsNullOrEmpty(obj_str)) return default;

        return GeneralHelper.FromJSON<T>(obj_str, pPrivateMembersIncluded);
    }
}