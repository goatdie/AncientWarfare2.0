using System.Reflection;
using System.Runtime.CompilerServices;
using Figurebox.constants;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Figurebox.Utils;

/// <summary>
///     仅用于存放一些通用的方法, 别搞成之前FunctionHelper那样了
/// </summary>
public static class GeneralHelper
{
    private static readonly JsonSerializerSettings private_members_visit_settings = new()
    {
        ContractResolver = new DefaultContractResolver
        {
            // 反正不改版本, 就用这个吧
#pragma warning disable 618
            DefaultMembersSearchFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
#pragma warning restore 618
        }
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToJSON(object obj, bool private_members_included = false)
    {
        if (private_members_included) return JsonConvert.SerializeObject(obj, private_members_visit_settings);

        return JsonConvert.SerializeObject(obj);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T FromJSON<T>(string json, bool private_members_included = false)
    {
        //return System.Text.Json.JsonSerializer.Deserialize<T>(json);
        if (private_members_included) return JsonConvert.DeserializeObject<T>(json, private_members_visit_settings);

        return JsonConvert.DeserializeObject<T>(json);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UnitProfession C(this AWUnitProfession pThis)
    {
        return (UnitProfession)pThis;
    }
}