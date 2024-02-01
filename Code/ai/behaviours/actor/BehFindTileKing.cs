using ai.behaviours;
using Figurebox.constants;
using NeoModLoader.api.attributes;
using System.Linq;
using System.Collections.Generic;

namespace Figurebox.ai.behaviours.actor
{
    public class BehFindTileKing : BehaviourActionActor
    {
        public override BehResult execute(Actor pActor)
        {
            // 检查是否有国王并且国王存活
            if (pActor.unit_group == null)
            {
                return BehResult.Stop;
            }
            if (pActor.kingdom?.king == null || !pActor.kingdom.king.isAlive())
            {
                return BehResult.Stop;
            }

            Actor king = pActor.kingdom.king;
            WorldTile worldTile;

            // 如果国王有当前路径，则选择路径的最后一个位置
            if (king.current_path != null && king.current_path.Count > 0)
            {
                worldTile = king.current_path[king.current_path.Count - 1].region.tiles.GetRandom<WorldTile>();
            }
            else
            {
                // 否则，从国王当前位置的地区或邻近地区中随机选择一个位置
                MapRegion mapRegion = king.currentTile.region;
                if (mapRegion.tiles.Count < 5 && mapRegion.neighbours.Count > 0)
                {
                    mapRegion = mapRegion.neighbours.GetRandom<MapRegion>();
                }
                worldTile = mapRegion.tiles.GetRandom<WorldTile>();
            }

            // 设置行为目标为找到的位置
            pActor.beh_tile_target = worldTile;
            return BehResult.Continue;
        }
    }
}
