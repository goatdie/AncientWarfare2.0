using System;
using System.Reflection.Emit;
using HarmonyLib;

namespace AncientWarfare.Utils.InstPredictors;

internal class GenericInstPredictor<T> : BaseInstPredictor
{
    protected Func<CodeInstruction, T, bool> predicate;

    protected GenericInstPredictor()
    {
    }

    protected internal GenericInstPredictor(OpCode pCode) : base(pCode)
    {
    }

    protected internal GenericInstPredictor(object pOperand) : base(pOperand)
    {
    }

    protected internal GenericInstPredictor(OpCode pCode, object pOperand) : base(pCode, pOperand)
    {
    }

    protected internal GenericInstPredictor(OpCode pCode, Func<T, bool> pPredicate)
    {
        predicate = (pInstruction, pOperand) =>
            OpcodeEquals(pCode, pInstruction) && (pPredicate?.Invoke(pOperand) ?? false);
    }

    protected internal GenericInstPredictor(Func<CodeInstruction, T, bool> pPredicate)
    {
        predicate = pPredicate;
    }

    public override bool Predict(CodeInstruction pInstruction)
    {
        if (pInstruction.operand is not T operand) return false;
        return predicate?.Invoke(pInstruction, operand) ?? false;
    }
}