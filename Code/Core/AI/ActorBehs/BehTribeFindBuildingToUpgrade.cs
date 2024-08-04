using System;
using System.Collections.Generic;
using ai.behaviours;
using AncientWarfare.Core.AI.Abstract;
using AncientWarfare.Core.Extensions;
using AncientWarfare.Core.Force;
using NeoModLoader.api.attributes;

namespace AncientWarfare.Core.AI.ActorBehs;

public class BehTribeFindBuildingToUpgrade : BehTribe
{
    private readonly Func<Building, bool> building_check;

    public BehTribeFindBuildingToUpgrade(Func<Building, bool> building_check)
    {
        this.building_check = building_check;
    }

    [Hotfixable]
    public override BehResult execute(Actor pObject)
    {
        Tribe tribe = pObject.GetTribe();
        List<Building> buildings_has_not_resource_to_upgrade = new();
        foreach (Building building in tribe.buildings)
        {
            if (!building_check?.Invoke(building) ?? false) continue;
            if (!building.asset.canBeUpgraded) continue;
            BuildingAsset upgrade_to = AssetManager.buildings.get(building.asset.upgradeTo);
            if (!tribe.Data.storage.HasResourceForConstruct(upgrade_to.cost))
            {
                buildings_has_not_resource_to_upgrade.Add(building);
                continue;
            }

            building.upgradeBuilding();
            building.setUnderConstruction();
            pObject.beh_building_target = building;
            return BehResult.Continue;
        }

        if (buildings_has_not_resource_to_upgrade.Count > 0)
        {
            Building nearest_b =
                pObject.GetNearestBuildingIn(buildings_has_not_resource_to_upgrade);
            tribe.NewResourceQuestsFromCost(nearest_b.asset.cost);
        }

        return BehResult.Stop;
    }
}