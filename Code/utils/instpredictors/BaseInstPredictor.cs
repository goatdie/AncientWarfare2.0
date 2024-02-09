using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace Figurebox.utils.instpredictors;

internal class BaseInstPredictor
{
    private static readonly Dictionary<OpCode, HashSet<OpCode>> equal_opcodes = new();
    private readonly        Func<CodeInstruction, object, bool> predicate;

    protected BaseInstPredictor()
    {
    }

    public BaseInstPredictor(OpCode pCode)
    {
        predicate = (inst, operand) => OpcodeEquals(pCode, inst);
    }

    public BaseInstPredictor(OpCode pCode, object pOperand)
    {
        predicate = (inst, operand) => OpcodeEquals(pCode, inst) && inst.operand == pOperand;
    }

    public BaseInstPredictor(Func<CodeInstruction, object, bool> pPredicate)
    {
        predicate = pPredicate;
    }

    public BaseInstPredictor(object pOperand)
    {
        predicate = (inst, operand) => inst.operand == pOperand;
    }

    public virtual bool Predict(CodeInstruction pInstruction)
    {
        return predicate?.Invoke(pInstruction, pInstruction.operand) ?? false;
    }

    protected static bool OpcodeEquals(OpCode pOpCode, OpCode pOpCodeAnother)
    {
        return pOpCodeAnother == pOpCode;
    }

    protected static bool OpcodeEquals(OpCode pOpCode, CodeInstruction pInstruction)
    {
        return pInstruction.opcode == pOpCode ||
               (equal_opcodes.TryGetValue(pOpCode, out var set) && set.Contains(pInstruction.opcode));
    }

    internal static void init()
    {
        AddEqualOpCodes(OpCodes.Br,      OpCodes.Br_S);
        AddEqualOpCodes(OpCodes.Brtrue,  OpCodes.Brtrue_S);
        AddEqualOpCodes(OpCodes.Brfalse, OpCodes.Brfalse_S);
    }

    private static void AddEqualOpCodes(params OpCode[] pOpCodes)
    {
        foreach (OpCode code in pOpCodes) equal_opcodes.Add(code, new HashSet<OpCode>(pOpCodes));
    }
}