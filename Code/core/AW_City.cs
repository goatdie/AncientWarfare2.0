using System;
using System.Collections.Generic;
using Figurebox.attributes;
using Figurebox.constants;
using Figurebox.core.dbs;

namespace Figurebox.core;

public class AW_City : City
{
    /// <summary>
    ///     拓展后的单位身份
    /// </summary>
    private static readonly AWUnitProfession[] ExtendUnitProfessions =
        (AWUnitProfession[])Enum.GetValues(typeof(AWUnitProfession));

    public int capital_tax_income;

    public int gold_pay_tax;

    public AW_City()
    {
        status = new AW_CityStatus();
    }

    /// <summary>
    ///     拓展后的城市状态
    /// </summary>
    public AW_CityStatus aw_status => (AW_CityStatus)status;

    public void PayTax()
    {
        AW_City Capital = kingdom.capital as AW_City;
        if (this == Capital || Capital == null)
        {
            return;
        }

        // 获取忠诚度
        int loyalty = getLoyalty();

        // 忠诚度低于 -100，则不交税
        if (loyalty < -100)
        {
            return;
        }

        // 基础税收值是 gold_change 的一半
        int baseTax = gold_change / 2;

        // 根据忠诚度调整税收值
        // 忠诚度高于 100 时，全额交税
        int finalTax;
        if (loyalty > 100)
        {
            finalTax = baseTax; // 全额税收
        }
        else
        {
            // 忠诚度在 -100 到 100 之间，按比例减少税收
            double loyaltyFactor = Math.Max(0, 1 - (Math.Abs(loyalty) / 10) * 0.02);
            finalTax = (int)(baseTax * loyaltyFactor);
        }

        // 确保税收数额非负
        finalTax = Math.Max(finalTax, 0);

        // 更新金币存储
        Capital.data.storage.change("gold", finalTax);
        data.storage.change("gold", -finalTax);
        gold_pay_tax = -finalTax;
        capital_tax_income = finalTax;
        //要添加事件文本
    }

    public int GetTaxToltal()
    {
        int num = 0;
        foreach (City c in kingdom.cities)
        {
            AW_City city = c as AW_City;
            num += city.capital_tax_income;
        }

        return num;
    }

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
        #region 原版代码

        Kingdom pKingdom2 = this.kingdom;
        this.removeFromCurrentKingdom();
        this.setKingdom(pKingdom, true);
        this.switchedKingdom();
        pKingdom.capturedFrom(pKingdom2);

        #endregion
    }

    [MethodReplace(nameof(City.updateAge))]
    internal new void updateAge()
    {
        this.gold_in_tax = this.getPopulationTotal(true) / 2;
        this.gold_out_homeless = this.getPopulationTotal(true) - this.status.housingTotal;
        if (this.gold_out_homeless < 0)
        {
            this.gold_out_homeless = 0;
        }

        this.gold_out_army = this.countProfession(UnitProfession.Warrior) / 2;
        this.gold_out_buildings = this.buildings.Count                    / 2;
        this.gold_change = this.gold_in_tax - this.gold_out_army - this.gold_out_buildings - this.gold_out_homeless;
        int num = this.gold_change;
        if (num < 0)
        {
            num = 0;
        }

        this.data.storage.change("gold", num);
        this.updatePopPoints();
        PayTax();                                               //交税
        CityPopRecordManager.NewCityPopCompositionRecord(this); //记录人口构成
    }
}