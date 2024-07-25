using AncientWarfare.Core;
using AncientWarfare.Core.Extensions;
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
    }
}
