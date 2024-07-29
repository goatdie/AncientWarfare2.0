using AncientWarfare.Core.Force;

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

        public static bool HasTribe(this TileZone zone)
        {
            return !string.IsNullOrEmpty(zone.GetAdditionData().tribe_id);
        }

        public static bool CanStartTribeHere(this TileZone zone)
        {
            return !zone.HasTribe() && zone.tilesWithGround >= 64 && zone.haveTiles(TileLibrary.hills);
        }
    }
}