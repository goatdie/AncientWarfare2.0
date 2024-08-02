using System.Collections.Generic;
using System.Linq;
using ai.behaviours;
using AncientWarfare.Core.Extensions;
using AncientWarfare.Core.Force;
using NeoModLoader.api.attributes;

namespace AncientWarfare.Core.AI.ActorBehs;

public class BehTribeStartBuilding : BehTribe
{
    private readonly HashSet<TileZone> _temp_zones = new();
    private readonly string            building_key;

    public BehTribeStartBuilding(string building_key)
    {
        this.building_key = building_key;
    }

    [Hotfixable]
    public override BehResult execute(Actor pObject)
    {
        Tribe tribe = pObject.GetTribe();
        Race race = tribe.Race ?? pObject.race;
        if (race == null)
        {
            Main.LogDebug($"No race found for {pObject.data.id}(tribe.Race==null?{tribe.Race == null})");
            return BehResult.Continue;
        }

        if (!race.building_order_keys.TryGetValue(building_key, out var building_id))
        {
            Main.LogDebug($"Race {race.id} has not building order key {building_key}", pLogOnlyOnce: true,
                          pLevel: DebugMsgLevel.Warning);
            return BehResult.Continue;
        }

        BuildingAsset building_asset = AssetManager.buildings.get(building_id);
        WorldTile tile = FindPlaceToStartBuilding(building_asset, pObject, tribe, race);
        if (tile == null) return BehResult.RepeatStep;

        Building building = World.world.buildings.addBuilding(building_id, tile, false, true);
        building.setUnderConstruction();
        ForceManager.MakeJoinToForce(building, tribe);

        pObject.beh_building_target = building;
        return BehResult.Continue;
    }

    private bool IsZoneHasBuilding(TileZone zone, string building_type)
    {
        if (_temp_zones.Contains(zone)) return true;
        if (!zone.HasBuildingType(building_type)) return false;
        _temp_zones.Add(zone);
        return true;
    }

    [Hotfixable]
    private WorldTile FindPlaceToStartBuilding(BuildingAsset asset, Actor actor, Tribe tribe, Race race)
    {
        List<TileZone> possible_zones = new();
        _temp_zones.Clear();
        foreach (TileZone zone in tribe.zones)
        {
            if (asset.build_place_batch)
                if (!(IsZoneHasBuilding(zone, asset.type) ||
                      zone.neighboursAll.Any(neigh => neigh.HasBuildingType(asset.type))))
                    continue;

            if (asset.build_place_borders)
                if (!IsBorderZone(zone, tribe))
                    continue;

            if (asset.build_place_single)
                if (zone.HasBuildingType(asset.type) ||
                    zone.neighboursAll.Any(neigh => neigh.HasBuildingType(asset.type)))
                    continue;

            possible_zones.Add(zone);
        }

        if (asset.build_place_batch && possible_zones.Count == 0) possible_zones.AddRange(tribe.zones);

        _temp_zones.Clear();

        possible_zones.Sort((a, b) =>
        {
            var dist_a = Toolbox.DistTile(a.centerTile, actor.currentTile);
            var dist_b = Toolbox.DistTile(b.centerTile, actor.currentTile);
            return dist_a.CompareTo(dist_b);
        });
        WorldTile tile = null;
        var try_place_center = asset.build_place_center | (race.buildingPlacements == BuildingPlacements.Center);
        if (try_place_center) tile = FindTileInPossibleZones(possible_zones, asset, actor, tribe, true);

        if (tile == null) tile = FindTileInPossibleZones(possible_zones, asset, actor, tribe, false);

        return tile;
    }

    private bool IsBorderZone(TileZone pZone, Tribe pTribe)
    {
        return pZone.neighboursAll.Any(neigh => neigh.GetTribe() != pTribe);
    }

    private bool IsSuitableForBuilding(WorldTile tile, BuildingAsset asset, Tribe tribe)
    {
        return tile.canBuildOn(asset) && asset.CanBuildOn(tile, tribe);
    }

    private WorldTile FindTileInPossibleZones(List<TileZone> possible_zones, BuildingAsset asset, Actor actor,
                                              Tribe          tribe,          bool          center)
    {
        foreach (TileZone zone in possible_zones)
            if (center)
            {
                WorldTile tile = Toolbox.randomChance(0.2f)
                    ? zone.centerTile
                    : zone.centerTile.neighboursAll.GetRandom();
                if (IsSuitableForBuilding(tile, asset, tribe)) return tile;
            }
            else
            {
                zone.tiles.ShuffleOne();
                foreach (WorldTile tile in zone.tiles)
                    if (IsSuitableForBuilding(tile, asset, tribe))
                        return tile;
            }

        return null;
    }
}