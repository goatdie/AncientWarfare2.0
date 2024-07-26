using System;
using System.Reflection;
using System.Reflection.Emit;

namespace AncientWarfare.Utils.InstPredictors;

internal class MethodInstPredictor : GenericInstPredictor<MethodInfo>
{
    public MethodInstPredictor(OpCode pCode, string pMethodName)
    {
        predicate = (inst, operand) => { return OpcodeEquals(pCode, inst) && operand.Name == pMethodName; };
    }

    public MethodInstPredictor(OpCode pCode, Type pDeclaringType, string pMethodName)
    {
        predicate = (inst, operand) => OpcodeEquals(pCode, inst) && IsSameMethod(operand, pDeclaringType, pMethodName);
    }

    private static bool IsSameMethod(MethodInfo pMethod, Type pDeclaringType, string pMethodName)
    {
        return pMethod.DeclaringType != null && pMethod.DeclaringType.IsAssignableFrom(pDeclaringType) &&
               pMethod.Name          == pMethodName;
    }
}