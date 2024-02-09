using System;
using System.Reflection.Emit;
using HarmonyLib;

namespace Figurebox.utils.instpredictors;

internal class LocalVarInstPredictor : GenericInstPredictor<LocalBuilder>
{
    public LocalVarInstPredictor(OpCode pCode, int pLocalIndex)
    {
        predicate = (inst, operand) =>
        {
            Main.LogDebug($"{inst.opcode} == {pCode} && {operand.LocalIndex} == {pLocalIndex}");
            return OpcodeEquals(pCode, inst) && operand.LocalIndex == pLocalIndex;
        };
    }

    public LocalVarInstPredictor(OpCode pCode, LocalBuilder pOperand)
    {
        predicate = (inst, operand) => OpcodeEquals(pCode, inst) && IsSameLocalVar(operand, pOperand);
    }

    public LocalVarInstPredictor(OpCode pCode, Func<LocalBuilder, bool> pPredicate) : base(pCode, pPredicate)
    {
    }

    public LocalVarInstPredictor(Func<CodeInstruction, LocalBuilder, bool> pPredicate) : base(pPredicate)
    {
    }

    private static bool IsSameLocalVar(LocalBuilder pLocalVar, int pLocalIndex)
    {
        return pLocalVar.LocalIndex == pLocalIndex;
    }

    private static bool IsSameLocalVar(LocalBuilder pLocalVar, LocalBuilder pOperand)
    {
        return pLocalVar.LocalIndex == pOperand.LocalIndex;
    }
}