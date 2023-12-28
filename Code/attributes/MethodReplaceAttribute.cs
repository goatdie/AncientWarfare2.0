using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
namespace Figurebox.attributes;

public class MethodReplaceAttribute : Attribute
{
    public readonly string MethodName;
    public readonly Type TargetType;
    public MethodReplaceAttribute()
    {

    }
    public MethodReplaceAttribute(string pMethodName)
    {
        MethodName = pMethodName;
    }
    public MethodReplaceAttribute(Type pTargetType)
    {
        TargetType = pTargetType;
    }
    public MethodReplaceAttribute(Type pTargetType, string pMethodName)
    {
        TargetType = pTargetType;
        MethodName = pMethodName;
    }
    public MethodInfo TrackTargetMethod(MethodInfo pOnMethod)
    {
        List<Type> parameter_types = new();
        foreach (var parameter in pOnMethod.GetParameters())
        {
            parameter_types.Add(parameter.ParameterType);
        }
        List<Type> generic_types = new();
        foreach (var generic_type in pOnMethod.GetGenericArguments())
        {
            generic_types.Add(generic_type);
        }

        var parameter_types_array = parameter_types.ToArray();
        var generic_types_array = generic_types.Count == 0 ? null : generic_types.ToArray();

        Type target_type = TargetType;

        var target_method_name = string.IsNullOrEmpty(MethodName) ? pOnMethod.Name : MethodName;

        if (target_type != null) return AccessTools.Method(TargetType, target_method_name, parameter_types_array, generic_types_array);

        if (pOnMethod.DeclaringType == null) return null;

        target_type = pOnMethod.DeclaringType;
        if (target_type.BaseType != null)
            target_type = target_type.BaseType;

        var target_method = AccessTools.Method(target_type, target_method_name, parameter_types_array, generic_types_array);
        while (target_method == null && target_type.BaseType != null)
        {
            target_type = target_type.BaseType;
            target_method = AccessTools.Method(target_type, target_method_name, parameter_types_array, generic_types_array);
        }
        return target_method;
    }
}