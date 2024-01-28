using Figurebox.attributes;
using Figurebox.constants;

namespace Figurebox.patch;

// ReSharper disable once UnusedType.Global
internal class ActorPatch
{
    [MethodReplace(typeof(ActorBase), nameof(ActorBase.nextJobActor))]
    public static string nextJobActor(ActorBase pActor)
    {
        if (pActor.has_trait_madness) return "random_move";
        if (pActor.asset.run_to_water_when_on_fire && pActor.has_status_burning) return "unit_on_fire";
        if (pActor.citizen_job != null) return pActor.citizen_job.unit_job_default;

        if (!pActor.asset.unit) return pActor.asset.job;

        if (pActor.city != null)
            return (AWUnitProfession)pActor.data.profession switch
            {
                AWUnitProfession.Warrior => "attacker",
                _                        => pActor.asset.baby ? "baby" : "citizen"
            };
        return pActor.asset.baby ? "baby" : pActor.asset.job;
    }
}