using AncientWarfare.Core.Force;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AncientWarfare.Core.Extensions
{
    public static class TileZoneExtension
    {
        public static TileZoneAdditionData GetAdditionData(this TileZone zone)
        {
            if (zone == null) return null;
            return TileZoneAdditionDataManager.Get(zone.x, zone.y);
        }
        public static Tribe GetTribe(this TileZone zone)
        {
            return ForceManager.GetForce<Tribe>(zone.GetAdditionData().tribe_id);
        }
        public static void SetTribe(this TileZone zone, Tribe tribe)
        {
            zone.GetAdditionData().tribe_id = tribe?.BaseData.id;
        }
    }
}
