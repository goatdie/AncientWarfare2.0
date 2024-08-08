using System.Collections.Generic;
using AncientWarfare.Abstracts;
using AncientWarfare.Core.Additions;
using AncientWarfare.Core.Extensions;
using AncientWarfare.Core.Force;
using NeoModLoader.api.attributes;

namespace AncientWarfare.Utils
{
    public class TribePlaceFinder : CityZoneWorkerBase, IManager
    {
        private const float update_interval = 1;

        private const   int            max_wave_depth = 3;
        public readonly List<TileZone> zones          = new();

        private       bool             dirty = true;
        private       float            timer = update_interval;
        public static TribePlaceFinder I { get; private set; }

        public void Initialize()
        {
            I = this;
        }

        public static bool HasPossibleZones()
        {
            return I.zones.Count > 0;
        }

        internal void Update(float elapsed)
        {
            if (!dirty) return;

            if (timer > 0)
            {
                timer -= elapsed;
            }
            else
            {
                DoCheck();
                dirty = false;
            }
        }

        private void DoCheck()
        {
            InitBasicZones();
            InitWaves();
            SimulateWave();
            CreateFinalList();
        }

        private void CreateFinalList()
        {
            foreach (TileZone zone in World.world.zoneCalculator.zones)
                if (zone.GetAdditionData().good_for_new_tribe)
                    zones.Add(zone);
        }

        [Hotfixable]
        private void SimulateWave()
        {
            var waves_checking = _wave;
            var waves_to_check = _next_wave;

            using var list_pool = new ListPool<MapRegion>();
            var curr_depth = 0;
            while (waves_checking.Count > 0 || waves_to_check.Count > 0)
            {
                if (waves_checking.Count == 0)
                {
                    (waves_checking, waves_to_check) = (waves_to_check, waves_checking);
                    curr_depth++;
                }

                if (curr_depth >= max_wave_depth) break;

                ZoneConnection c = waves_checking.Dequeue();
                MapRegion region = c.region;
                TileZone zone = c.zone;

                foreach (TileZone neighbour in zone.neighbours)
                {
                    if (neighbour.HasTribe()) continue;

                    list_pool.Clear();
                    if (!TileZone.hasZonesConnectedViaRegions(zone, neighbour, region, list_pool)) continue;

                    foreach (MapRegion path_region in list_pool)
                    {
                        var new_c = new ZoneConnection(neighbour, path_region);
                        if (_zones_checked.Add(new_c))
                        {
                            neighbour.GetAdditionData().good_for_new_tribe = false;
                            queueConnection(new_c, waves_to_check, true);
                        }
                    }
                }
            }
        }

        private void InitWaves()
        {
            prepareWave();
            foreach (Tribe tribe in ForceManager.I.tribes.All)
            {
                WorldTile center_tile = tribe.CenterTile;
                TileIsland center_island = center_tile.region.island;

                foreach (TileZone border_zone in tribe.border_zones)
                foreach (MapRegion map_region in border_zone.centerTile.chunk.regions)
                    if (map_region.isTypeGround() && map_region.zones.Contains(border_zone) &&
                        map_region.island == center_island)
                        queueConnection(new ZoneConnection(border_zone, map_region), _wave, true);
            }
        }

        [Hotfixable]
        private void InitBasicZones()
        {
            base.clearAll();
            zones.Clear();
            foreach (TileZone zone in World.world.zoneCalculator.zones)
                zone.GetAdditionData().good_for_new_tribe =
                    zone.CanStartTribeHere() || zone.centerTile.region.island.getTileCount() >= 300;
            //Main.LogDebug($"Initialize zones with {World.world.zoneCalculator.zones.Count(x=>x.GetAdditionData().good_for_new_tribe)} good for new tribe");
        }

        public void SetDirty()
        {
            dirty = true;
            timer = update_interval;
            ClearCurrentZones();
        }

        private void ClearCurrentZones()
        {
            foreach (TileZone zone in zones)
            {
                TileZoneAdditionData data = zone.GetAdditionData();
                data.good_for_new_tribe = false;
            }

            zones.Clear();
        }
    }
}