using AncientWarfare.Core;
using AncientWarfare.Core.Extensions;
using AncientWarfare.Core.Force;
using AncientWarfare.Utils;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace AncientWarfare.Patches
{
    internal static class PatchActor
    {
        [HarmonyPrefix, HarmonyPatch(typeof(ActorBase), nameof(ActorBase.getNextJob))]
        private static bool Patch_getNextJob(ActorBase __instance, ref string __result)
        {
            if (!__instance.asset.unit) return true;
            var actor = __instance as Actor;
            var data = actor.GetAdditionData();

            if (data.Forces.Count == 0)
            {
                __result = __instance.asset.job;
                return false;
            }

            __result = "random_move";

            return false;
        }
        [HarmonyPostfix, HarmonyPatch(typeof(Actor), nameof(Actor.killHimself))]
        private static void Postfix_killHimself(Actor __instance)
        {
            var data = __instance.GetAdditionData();
            var forces = new HashSet<string>(data.Forces);
            foreach (var force_id in forces)
            {
                ForceManager.MakeLeaveForce(__instance, force_id);
            }
        }
        [HarmonyTranspiler, HarmonyPatch(typeof(Actor), nameof(Actor.updateHunger))]
        private static IEnumerable<CodeInstruction> Transpiler_updateHunger(IEnumerable<CodeInstruction> insts)
        {
            var codes = new List<CodeInstruction>(insts);

            int remove_start_idx = HarmonyTools.FindInstructionIdx<MethodInfo>(codes, OpCodes.Call, x => x.Name == nameof(Actor.decreaseHunger))+1;
            int remove_end_idx = HarmonyTools.FindInstructionIdx<MethodInfo>(codes, OpCodes.Call, x => x.Name == nameof(Actor.consumeCityFoodItem));
            codes.RemoveRange(remove_start_idx, remove_end_idx - remove_start_idx + 1);

            int insert_idx = remove_start_idx;
            codes.InsertRange(insert_idx, new List<CodeInstruction>
            {
                new (OpCodes.Ldarg_0),
                new (OpCodes.Call, AccessTools.Method(typeof(PatchActor), nameof(TryToTakeTribeFood)))
            });

            return codes;
        }

        private static void TryToTakeTribeFood(Actor actor)
        {
            if (actor.data.hunger > 10 || actor.has_attack_target) return;
            var tribe = actor.GetTribe();
            if (tribe == null) return;

            var food = tribe.Data.storage.TakeFood(actor.data.favoriteFood);
            if (food == null) return;
            actor.ConsumeFood(food);

            if (!actor.hasTrait("gluttonous")) return;
            food = tribe.Data.storage.TakeFood(actor.data.favoriteFood);
            if (food == null) return;
            actor.ConsumeFood(food);
        }
    }
}
