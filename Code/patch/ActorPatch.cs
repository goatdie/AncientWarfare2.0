using System.Text;
using Figurebox.attributes;
using Figurebox.constants;
using Figurebox.utils.extensions;
using HarmonyLib;

namespace Figurebox.patch;

// ReSharper disable once UnusedType.Global
internal class ActorPatch : AutoPatch
{
    [MethodReplace(typeof(ActorBase), nameof(ActorBase.nextJobActor))]
    private static string nextJobActor(ActorBase pActor)
    {
        if (pActor.has_trait_madness) return "random_move";
        if (pActor.asset.run_to_water_when_on_fire && pActor.has_status_burning) return "unit_on_fire";
        if (pActor.citizen_job != null)
            return string.IsNullOrEmpty(pActor.citizen_job.unit_job_default)
                ? pActor.citizen_job.unit_jobs_list.GetRandom().id
                : pActor.citizen_job.unit_job_default;

        if (!pActor.asset.unit) return pActor.asset.job;

        if (pActor.city != null)
            return (AWUnitProfession)pActor.data.profession switch
            {
                AWUnitProfession.Warrior => "attacker",
                _                        => pActor.asset.baby ? "baby" : "citizen"
            };
        return pActor.asset.baby ? "baby" : pActor.asset.job;
    }
    [HarmonyPostfix, HarmonyPatch(typeof(ActorBase), nameof(ActorBase.taskSwitchedAction))]
    private static void checkTechProgress(ActorBase __instance)
    {
        if (__instance.city == null || !__instance.city.isAlive() || __instance.citizen_job == null)
        {
            return;
        }
        if (!Toolbox.randomChance(__instance.stats[S.intelligence] / 100)) return;

        __instance.city.AW().PushResearchThrough(__instance as Actor, __instance.citizen_job);
    }
    //[MethodReplace(typeof(ActorBase), nameof(ActorBase.getUnitTexturePath))]
    private static string getUnitTexturePath(ActorBase pActor)
    {
        var sb = new StringBuilder();
        sb.Append(pActor.race.main_texture_path);
        if (pActor.asset.baby)
        {
            sb.Append("unit_child");
            return sb.ToString();
        }

        Culture culture = World.world.cultures.get(pActor.data.culture);
        //if (pActor.pro)
        return "";
    }
}