using AncientWarfare.Abstracts;
using AncientWarfare.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AncientWarfare.Core
{
    public class TileZoneAdditionDataManager : IManager
    {
        private static TileZoneAdditionData[,] _data;
        public static TileZoneAdditionData Get(int x, int y)
        {
            return _data[x, y];
        }

        public void Initialize()
        {
            //Reset();
        }
        internal static void Reset()
        {
            var zone_size = 8;
            var width = Config.ZONE_AMOUNT_X * zone_size;
            var height = Config.ZONE_AMOUNT_Y * zone_size;

            _data = new TileZoneAdditionData[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    _data[x, y] = new TileZoneAdditionData();
                }
            }
            Main.LogDebug($"TileZoneAdditionDataManager Reset to ({width}*{height})");
        }
    }
}
