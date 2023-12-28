using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Figurebox.attributes;
using HarmonyLib;
namespace Figurebox.Utils;

internal class AutoMethodReplaceTool
{
    private static MethodInfo _origin_method;
    public static void ReplaceMethods()
    {
        Type[] types = Assembly.GetExecutingAssembly().GetTypes();

        HashSet<string> replaced_methods = new();
        foreach (Type type in types)
        {
            MethodInfo[] method_infos = type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (MethodInfo method_info in method_infos)
            {
                var attribute = method_info.GetCustomAttribute<MethodReplaceAttribute>();
                if (method_info.GetCustomAttribute<MethodReplaceAttribute>() == null) continue;

                MethodInfo target_method = attribute.TrackTargetMethod(method_info);

                if (target_method != null)
                {
                    StringBuilder method_full_name = new();
                    method_full_name.Append(target_method.DeclaringType.FullName);
                    method_full_name.Append(".");
                    method_full_name.Append(target_method.Name);
                    method_full_name.Append("<");
                    foreach (var parameter in target_method.GetParameters())
                    {
                        method_full_name.Append(parameter.ParameterType.FullName);
                        method_full_name.Append(",");
                    }
                    method_full_name.Append(">");

                    if (replaced_methods.Contains(method_full_name.ToString()))
                    {
                        Main.LogWarning("重复替换方法, 请检查MethodReplaceAttribute的参数是否正确");
                        Main.LogWarning($"Attribute作用于: {method_info.DeclaringType.FullName}:{method_info.Name}");
                        Main.LogWarning($"重复替换方法: {method_full_name}");
                        continue;
                    }
                    try
                    {
                        ReplaceMethod(method_info, target_method);
                        replaced_methods.Add(method_full_name.ToString());
                    }
                    catch (Exception e)
                    {
                        Main.LogWarning($"替换方法{method_full_name}时发生错误: {e}");
                        Main.LogWarning(e.StackTrace);
                    }
                }
                else
                {
                    Main.LogWarning("无法找到目标替换方法, 请检查MethodReplaceAttribute的参数是否正确");
                    Main.LogWarning($"Attribute作用于: {method_info.DeclaringType.FullName}:{method_info.Name}");
                }
            }
        }
    }
    private static void ReplaceMethod(MethodInfo pOriginalMethod, MethodInfo pTargetMethod)
    {
        _origin_method = pOriginalMethod;
        Harmony harmony = new($"Figurebox.AutoMethodReplaceTool.{pTargetMethod.DeclaringType.FullName}.{pTargetMethod.Name}");
        harmony.Patch(pTargetMethod, transpiler: new HarmonyMethod(typeof(AutoMethodReplaceTool), nameof(_method_replace_patch)));
    }
    private static IEnumerable<CodeInstruction> _method_replace_patch(IEnumerable<CodeInstruction> instr)
    {
        var codes = new List<CodeInstruction>();

        int param_count = _origin_method.GetParameters().Length + (_origin_method.IsStatic ? 0 : 1);
        for (int i = 0; i < param_count; i++)
        {
            codes.Add(new CodeInstruction(OpCodes.Ldarg, i));
        }
        codes.Add(new CodeInstruction(OpCodes.Callvirt, _origin_method));
        codes.Add(new CodeInstruction(OpCodes.Ret));

        foreach (var code in codes)
        {
            Main.LogInfo(code.ToString());
        }
        return codes;
    }
}