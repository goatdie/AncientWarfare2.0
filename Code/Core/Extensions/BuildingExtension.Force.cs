using System.Linq;
using AncientWarfare.Core.Force;

namespace AncientWarfare.Core.Extensions;

public static partial class BuildingExtension
{
    public static Tribe GetTribe(this Building building)
    {
        BuildingAdditionData data = building.GetAdditionData();
        Tribe tribe = data.Forces.Select(ForceManager.GetForce<Tribe>).FirstOrDefault(x => x != null);
        return tribe;
    }

    public static void JoinForceOneside(this Building building, LowBaseForce force)
    {
        BuildingAdditionData data = building.GetAdditionData();

        data.Forces.Add(force.BaseData.id);
    }

    public static void LeaveForceOnesice(this Building building, LowBaseForce force)
    {
        BuildingAdditionData data = building.GetAdditionData();

        data.Forces.Remove(force.BaseData.id);
    }
}