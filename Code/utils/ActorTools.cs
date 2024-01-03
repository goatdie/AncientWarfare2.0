using System;
using UnityEngine;
using System.Collections.Generic;
using Figurebox.constants;
using Figurebox.core;
namespace Figurebox.Utils;
public static class ActorTools
{
    public static void ChangeCity(this Actor actor, City ncity)
    {
        actor.city?.removeUnit(actor, false);
        ncity.addNewUnit(actor);
    }
    public static void removeUnit(this City city, Actor pActor, bool pUnsetKingdom = true)
    {
        // 如果单位是船，从船只集合中移除
        if (pActor.asset.isBoat)
        {
            city.boats.Remove(pActor);
        }
        else
        {
            // 否则，从普通单位集合中移除
            city.units.Remove(pActor);
            city._dirty_units = true;
        }

        // 更新状态
        city.setStatusDirty();

        // 如果指定，从其王国中移除单位
        if (pUnsetKingdom)
        {
            pActor.setKingdom(null);
        }

        // 这里可以添加任何其他需要在移除单位时进行的操作

    }

}