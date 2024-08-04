using ai.behaviours;
using AncientWarfare.Core.AI.Abstract;
using AncientWarfare.Core.Content;
using AncientWarfare.Core.Extensions;
using AncientWarfare.Core.Force;

namespace AncientWarfare.Core.AI.ActorBehs;

public class BehTribeFindCouple : BehTribe
{
    public override BehResult execute(Actor pObject)
    {
        ActorGender target_gender;
        if (pObject.data.gender == ActorGender.Male)
            target_gender = ActorGender.Female;
        else if (pObject.data.gender == ActorGender.Female)
            target_gender = ActorGender.Male;
        else
            return BehResult.Stop;

        Tribe tribe = pObject.GetTribe();

        foreach (var actor_id in tribe.Data.members)
        {
            Actor actor = World.world.units.get(actor_id);
            if (actor.data.gender != target_gender) continue;
            if (actor.getAge() < (actor.asset.procreate_age == 3 ? 18 : actor.asset.procreate_age))
                continue; // 3 is the default value of procreate_age
            if (actor.hasStatus(StatusEffectExtendLibrary.pregnant.id)) continue;
            pObject.beh_actor_target = actor;
            return BehResult.Continue;
        }

        return BehResult.Stop;
    }
}