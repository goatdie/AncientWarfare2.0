using ai.behaviours;
using AncientWarfare.Core.Extensions;
using AncientWarfare.Core.Force;
using AncientWarfare.Utils;

namespace AncientWarfare.Core.AI.ActorBehs
{
    public class BehCheckCreateTribe : BehaviourActionActor
    {
        public override BehResult execute(Actor pObject)
        {
            var zone = pObject.currentTile.zone;
            var tribe = zone.GetTribe();
            if (tribe != null) return BehResult.Stop;

            var zone_data = zone.GetAdditionData();
            if (!zone_data.good_for_new_tribe) return BehResult.Stop;

            tribe = ForceManager.I.tribes.CreateNewTribe(zone);
            if (tribe == null) return BehResult.Stop;
            TribePlaceFinder.I.SetDirty();
            ForceManager.MakeJoinToForce(pObject, tribe);
            WorldLogHelper.LogNewTribe(tribe);

            return BehResult.Continue;
        }
    }
}