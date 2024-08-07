using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using AncientWarfare.Core;
using AncientWarfare.Core.AI;
using AncientWarfare.Core.Content;
using AncientWarfare.Core.Extensions;
using AncientWarfare.Core.Force;
using AncientWarfare.Core.Quest;
using AncientWarfare.Core.Tech;
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
            if (actor == null) throw new Exception();

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

            if (actor.getAge() <= Math.Max(actor.asset.procreate_age, 3))
            {
                __result = nameof(ActorJobExtendLibrary.baby);
                return false;
            }

            if (actor.HasTechToUnlock() && tribe.AllowUnlockTechWithoutProduction(actor))
            {
                var tech_to_unlock = actor.GetNextTechToUnlock();
                if (tribe.HasTech(tech_to_unlock))
                {
                    __result = nameof(ActorJobExtendLibrary.learn_tech);
                    return false;
                }

                if (actor.WantToStudy(tech_to_unlock))
                {
                    __result = actor.FindJobToUnlockTech(tech_to_unlock);
                    return false;
                }
            }

            if (tribe.AllowFindJobItSelf(actor) && Toolbox.randomChance(actor.GetPossibilityToFindJobItSelf()))
            {
                __result = actor.FindJobItSelf();
                return false;
            }

            List<KeyValuePair<QuestInst, float>> quests = new();
            var suitable_quest_found = false;
            foreach (QuestInst q in tribe.quests)
            {
                if (!q.Active) continue;
                if (!q.CanTake) continue;

                var score = actor.ComputeScoreFor(q);
                quests.Add(new KeyValuePair<QuestInst, float>(q, score));
                suitable_quest_found |= score > 0;
            }

            if (!suitable_quest_found)
                if (tribe.AllowFindJobItSelf(actor) && Toolbox.randomChance(actor.GetPossibilityToFindJobItSelf()))
                {
                    __result = actor.FindJobItSelf();
                    return false;
                }

            quests.Sort((a, b) => a.Value.CompareTo(b.Value));
            var quest_to_take = quests.GetRandom(5);
            if (!suitable_quest_found)
                if (tribe.AllowUnlockTechWithoutProduction(actor))
                    actor.TrackTechsToUnlock(new List<TechAsset>()); // TODO: 从任务中获取科技要求

            quest_to_take.Key.Take();
            __result = quest_to_take.Key.asset.allow_jobs.GetRandom();
            return false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Actor), nameof(Actor.updateAge))]
        private static void Postfix_updateAge(Actor __instance)
        {
            if (!__instance.asset.unit) return;
            if (__instance.isHomeBuildingUsable()) return;

            Tribe tribe = __instance.GetTribe();
            if (tribe == null) return;
            foreach (Building b in tribe.buildings.getSimpleList())
            {
                if (b.asset.housing <= 0) continue;
                if (b.IsFull()) continue;

                __instance.setHomeBuilding(b);
                break;
            }

            // 建新房子
            if (__instance.homeBuilding == null) tribe.NewExpandHousingQuest();
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Actor), nameof(Actor.setHomeBuilding))]
        private static void Prefix_setHomeBuilding(Actor __instance, Building pBuilding)
        {
            __instance.homeBuilding?.ChangeCurrHousing(-1);
            pBuilding?.ChangeCurrHousing(1);
        }

        [HarmonyPostfix, HarmonyPatch(typeof(Actor), nameof(Actor.killHimself))]
        private static void Postfix_killHimself(Actor __instance)
        {
            __instance.setHomeBuilding(null);
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

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Actor), nameof(Actor.newKillAction))]
        private static void Postfix_newKillAction(Actor __instance, Actor pDeadUnit)
        {
            Tribe tribe = __instance.GetTribe();
            if (tribe == null) return;

            if (!pDeadUnit.asset.animal) return;

            var base_drop = Toolbox.randomInt(1, (int)pDeadUnit.asset.actorSize);
            if (__instance.hasTrait(ActorTraitExtendLibrary.savage.id)) base_drop *= 2;

            __instance.addToInventory(SR.meat,    base_drop);
            __instance.addToInventory(SR.bones,   Toolbox.randomInt(1, base_drop + 1));
            __instance.addToInventory(SR.leather, Toolbox.randomInt(0, base_drop));
            Main.LogDebug($"Hunter {__instance.data.id} get resources with base drop count {base_drop}");
        }
    }
}