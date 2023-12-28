using System;
using System.Reflection;
using Figurebox.attributes;
namespace Figurebox.Utils;

internal class AutoMethodReplaceTool
{
    public static void ReplaceMethods()
    {
        Type[] types = Assembly.GetExecutingAssembly().GetTypes();
        foreach (Type type in types)
        {
            MethodInfo[] method_infos = type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (MethodInfo method_info in method_infos)
            {
                var attribute = method_info.GetCustomAttribute<MethodReplaceAttribute>();
                if (method_info.GetCustomAttribute<MethodReplaceAttribute>() != null)
                {

                }
            }
        }
    }
    private static void ReplaceMethod(MethodInfo pOriginalMethod, MethodInfo pTargetMethod)
    {

    }
}