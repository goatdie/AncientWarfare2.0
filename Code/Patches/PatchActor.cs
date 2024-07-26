using AncientWarfare.Core;
using AncientWarfare.Core.Extensions;
using AncientWarfare.Core.Force;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
