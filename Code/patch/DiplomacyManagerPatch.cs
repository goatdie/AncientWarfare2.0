using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Figurebox.core;
using Figurebox.utils;
using Figurebox.utils.extensions;
using HarmonyLib;

namespace Figurebox.patch;

internal class DiplomacyManagerPatch : AutoPatch
{
    [HarmonyTranspiler]
    [HarmonyPatch(typeof(DiplomacyManager), nameof(DiplomacyManager.findSupremeKingdom))]
    private static IEnumerable<CodeInstruction> FindSupremeKingdomTranspiler(IEnumerable<CodeInstruction> inst)
    {
        List<CodeInstruction> codes = new(inst);

        var index = HarmonyTools.FindInstructionIdx<FieldInfo>(codes, OpCodes.Stfld,
                                                               x => x.Name == nameof(Kingdom.power));

        codes.Insert(index++, new CodeInstruction(OpCodes.Ldloc_2));
        codes.Insert(index++, new CodeInstruction(OpCodes.Ldloc_2));
        codes.Insert(
            index++,
            new CodeInstruction(OpCodes.Callvirt,
                                AccessTools.Method(typeof(KingdomExtension), nameof(KingdomExtension.AW))));
        codes.Insert(
            index++,
            new CodeInstruction(OpCodes.Callvirt,
                                AccessTools.Method(typeof(AW_Kingdom), nameof(AW_Kingdom.CalcPower))));
        codes.Insert(
            index++, new CodeInstruction(OpCodes.Stfld, AccessTools.Field(typeof(Kingdom), nameof(Kingdom.power))));


        return codes;
    }
}