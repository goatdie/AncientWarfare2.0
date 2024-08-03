using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using AncientWarfare.Core;
using AncientWarfare.Core.AI;
using AncientWarfare.Core.Extensions;
using AncientWarfare.Core.Force;
using AncientWarfare.Core.Quest;
using AncientWarfare.Utils;
using AncientWarfare.Utils.InstPredictors;
using HarmonyLib;
using NeoModLoader.api.attributes;

namespace AncientWarfare.Patches
{
    internal static class PatchActor
    {
        [Hotfixable]
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

            // TODO: 多势力任务协调
            Tribe tribe = actor.GetTribe();
            if (tribe == null)
            {
                Main.LogDebug($"{actor.data.id}'s tribe==null(actor has forces: {data.Forces.Join()})",
                              pLevel: DebugMsgLevel.Warning, pShowStackTrace: true, pLogOnlyOnce: true);
                __result = nameof(ActorJobExtendLibrary.random_move);
                return false;
            }

            var work_chance = 0.8f;
            if (Toolbox.randomChance(work_chance))
            {
                tribe.quests.ShuffleOne();
                foreach (QuestInst quest in tribe.quests)
                {
                    if (!quest.Active) continue;
                    if (!quest.CanTake) continue;
                    quest.Take();
                    __result = quest.asset.allow_jobs.GetRandom();
                    Main.LogDebug($"{actor.data.id} takes quest {quest.asset.id} with job {__result}");
                    return false;
                }
            }

            var produce_chance = 0.6f;
            __result = Toolbox.randomChance(produce_chance)
                ? nameof(ActorJobExtendLibrary.produce_children)
                : nameof(ActorJobExtendLibrary.random_move);


            return false;
        }

        [HarmonyPostfix, HarmonyPatch(typeof(Actor), nameof(Actor.killHimself))]
        private static void Postfix_killHimself(Actor __instance)
        {
            ActorAdditionData data = __instance.GetAdditionData(true);
            if (data == null) return;
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

            var remove_start_idx =
                HarmonyTools.FindInstructionIdx<MethodInfo>(codes, OpCodes.Call,
                                                            x => x.Name == nameof(Actor.decreaseHunger)) + 1;
            int remove_end_idx = HarmonyTools.FindCodeSnippetIdx(codes,
                                                                 new MethodInstPredictor(
                                                                     OpCodes.Call, nameof(Actor.consumeCityFoodItem)),
                                                                 new BaseInstPredictor(OpCodes.Ldarg_0),
                                                                 new FieldInstPredictor(
                                                                     AccessTools.Field(
                                                                         typeof(Actor), nameof(Actor.data))));
            codes.RemoveRange(remove_start_idx, remove_end_idx - remove_start_idx + 1);

            int insert_idx = remove_start_idx;
            codes.InsertRange(insert_idx, new List<CodeInstruction>
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, AccessTools.Method(typeof(PatchActor), nameof(TryToTakeTribeFood)))
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