using System.Linq;
using ai.behaviours;
using Figurebox.core;
using Figurebox.utils;
using Figurebox.attributes;
using NeoModLoader.api.attributes;
using System.Collections.Generic;

namespace Figurebox.ai.behaviours.city
{
    public class BehCheckRetirement : BehaviourActionCity
    {
        public override BehResult execute(City pObject)
        {
            var city = (AW_City)pObject;
            if (!city.professionsDict.ContainsKey(UnitProfession.Warrior))
            {
                return BehResult.Continue;
            }
            // 遍历城市中的所有单位
            foreach (var actor in city.professionsDict[UnitProfession.Warrior])
            {
                // 检查是否为军人并且达到退休年龄
                if (actor.data.profession == UnitProfession.Warrior &&
                    actor.data.getAge() >= ((int)(actor.stats[S.max_age] * 0.7)))
                {
                    actor.stopBeingWarrior();
                    actor.addTrait("veteran");
                    if (actor.hasTrait("禁卫军")) actor.removeTrait("禁卫军");
                    //Main.LogInfo("退休" + actor.getName());
                }
            }

            return BehResult.Continue;
        }
    }
}
