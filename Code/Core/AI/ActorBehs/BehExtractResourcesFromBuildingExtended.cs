using System.Collections.Generic;
using ai.behaviours;
using AncientWarfare.Core.AI.Abstract;
using AncientWarfare.Core.Extensions;
using AncientWarfare.Core.Profession;

namespace AncientWarfare.Core.AI.ActorBehs;

public class BehExtractResourcesFromBuildingExtended : BehActionActorExtended
{
    public BehExtractResourcesFromBuildingExtended(List<string>            tech_required = null,
                                                   Dictionary<string, int> exp_given = null) : base(BehResult.Continue)
    {
        TechRequired = tech_required;
        ExpGiven = exp_given;
        if (tech_required != null) TechRequiredAccurate = true;
    }


    public override Dictionary<string, int> ExpGiven             { get; }
    public override List<string>            TechRequired         { get; }
    public override bool                    TechRequiredAccurate { get; }

    public override void create()
    {
        base.create();
        null_check_building_target = true;
        check_building_target_non_usable = true;
    }

    public override (BehResult, bool) OnExecute(Actor actor)
    {
        BuildingAsset asset = actor.beh_building_target.asset;
        actor.beh_building_target.extractResources(actor);
        if (asset.resources_given != null)
        {
            foreach (ResourceContainer item in asset.resources_given)
            {
                var bonus_resource = GetBonusResource(actor, item.id);
                var num = item.amount + bonus_resource;
                if (asset.buildingType == BuildingType.Mineral && actor.hasTrait("miner") &&
                    Toolbox.randomBool()) num++;

                actor.addToInventory(item.id, num);
            }

            return (BehResult.Continue, true);
        }

        return (BehResult.Continue, false);
    }

    private static void AddExpForActor(Actor actor)
    {
        BuildingAsset asset = actor.beh_building_target.asset;
        switch (asset.buildingType)
        {
            case BuildingType.Mineral:
                actor.IncreaseProfessionExp(nameof(NewProfessionLibrary.mine));
                break;
            case BuildingType.Fruits:
            case BuildingType.Plant:
            case BuildingType.Wheat:
            case BuildingType.Tree:
                actor.IncreaseProfessionExp(nameof(NewProfessionLibrary.collect));
                break;
        }
    }

    // Token: 0x06001FFD RID: 8189 RVA: 0x000F3B40 File Offset: 0x000F1D40
    private static int GetBonusResource(Actor pActor, string pResourceID)
    {
        Culture culture = pActor.getCulture();
        if (culture == null) return 0;

        ResourceAsset resource_asset = AssetManager.resources.get(pResourceID);
        var result = 0;
        if (resource_asset.wood && culture.hasTech("sharp_axes"))
        {
            if (Toolbox.randomChance(culture.stats.bonus_res_chance_wood.value))
                result = (int)culture.stats.bonus_res_wood_amount.value;
        }
        else if (resource_asset.mineral && culture.hasTech("mining_efficiency") &&
                 Toolbox.randomChance(culture.stats.bonus_res_chance_mineral.value))
        {
            result = (int)culture.stats.bonus_res_mineral_amount.value;
        }

        return result;
    }
}