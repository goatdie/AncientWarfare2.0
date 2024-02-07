using System.Linq;
using System;

namespace Figurebox.utils
{
    public static class CityTools
    {
        /// <summary>
        ///     从城市中移出单位
        /// </summary>
        /// <param name="city">城市</param>
        /// <param name="pActor">单位</param>
        /// <param name="pUnsetKingdom">是否从其王国中移出</param>
        public static void removeUnit(this City city, Actor pActor, bool pUnsetKingdom = true)
        {
            // 如果单位是船，从船只集合中移除
            city._dirty_units = true;
            if (pActor.asset.isBoat)
            {
                city.boats.Remove(pActor);
            }
            else
            {
                // 否则，从普通单位集合中移除
                city.units.Remove(pActor);
                if (pActor == city.leader) city.removeLeader();
            }

            // 更新状态
            city.setStatusDirty();

            // 如果指定，从其王国中移除单位
            if (pUnsetKingdom) pActor.setKingdom(null);

            // 这里可以添加任何其他需要在移除单位时进行的操作
        }
    }
}