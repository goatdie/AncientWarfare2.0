using System;
using System.Collections.Generic;
using Figurebox.attributes;
using Figurebox.constants;

namespace Figurebox.core;

public class AW_City : City
{
    /// <summary>
    ///     拓展后的单位身份
    /// </summary>
    private static readonly AWUnitProfession[] ExtendUnitProfessions =
        (AWUnitProfession[])Enum.GetValues(typeof(AWUnitProfession));

    public AW_City()
    {
        status = new AW_CityStatus();
    }

    /// <summary>
    ///     拓展后的城市状态
    /// </summary>
    public AW_CityStatus aw_status => (AW_CityStatus)status;

    [MethodReplace(nameof(City.updateCitizens))]
    private new void updateCitizens()
    {
        _dirty_units = false;
        if (professionsDict.Count == 0)
            foreach (var prof in ExtendUnitProfessions)
                professionsDict.Add((UnitProfession)prof, new List<Actor>());

        foreach (var list in professionsDict.Values) list.Clear();
        var simpleList = units.getSimpleList();
        // 其他特殊身份
        foreach (var actor in simpleList)
            if (actor.asset.baby)
                professionsDict[UnitProfession.Baby].Add(actor);
            else if (actor == null || !actor.isAlive())
                units.Remove(actor);
            else
                professionsDict[actor.data.profession].Add(actor);
    }
    [MethodReplace(nameof(City.joinAnotherKingdom))]
    public new void joinAnotherKingdom(Kingdom pKingdom)
    {
        #region  原版代码
        Kingdom pKingdom2 = this.kingdom;
        this.removeFromCurrentKingdom();
        this.setKingdom(pKingdom, true);
        this.switchedKingdom();
        pKingdom.capturedFrom(pKingdom2);
        #endregion
        
    }
}