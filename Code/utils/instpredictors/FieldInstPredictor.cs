using System;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace AncientWarfare.Utils.InstPredictors;

internal class FieldInstPredictor : GenericInstPredictor<FieldInfo>
{
    public FieldInstPredictor(OpCode pCode) : base(pCode)
    {
    }

    public FieldInstPredictor(FieldInfo pOperand) : this(pOperand.DeclaringType, pOperand.Name)
    {
    }

    public FieldInstPredictor(Type pDeclaringType, string pFieldName)
    {
        predicate = (inst, operand) => IsSameField(operand, pDeclaringType, pFieldName);
    }

    public FieldInstPredictor(OpCode pCode, Type pDeclaringType, string pFieldName)
    {
        predicate = (inst, operand) => OpcodeEquals(pCode, inst) && IsSameField(operand, pDeclaringType, pFieldName);
    }

    public FieldInstPredictor(OpCode pCode, FieldInfo pOperand) : this(pCode, pOperand.DeclaringType, pOperand.Name)
    {
    }

    public FieldInstPredictor(OpCode pCode, Func<FieldInfo, bool> pPredicate) : base(pCode, pPredicate)
    {
    }

    public FieldInstPredictor(Func<CodeInstruction, FieldInfo, bool> pPredicate) : base(pPredicate)
    {
    }

    private static bool IsSameField(FieldInfo pField, Type pDeclaringType, string pFieldName)
    {
        return pField.DeclaringType != null && pField.DeclaringType.IsAssignableFrom(pDeclaringType) &&
               pField.Name          == pFieldName;
    }
}