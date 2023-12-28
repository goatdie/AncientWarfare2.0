using System;
using System.Reflection;
namespace Figurebox.attributes;

public class MethodReplaceAttribute : Attribute
{
    public readonly string MethodName;
    public readonly Type TargetType;
    public MethodReplaceAttribute()
    {

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
        throw new NotImplementedException();
    }
}