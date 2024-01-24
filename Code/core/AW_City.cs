using System;
using System.Collections.Generic;
using System.Linq;
using Figurebox.attributes;
using Figurebox.constants;
using Figurebox.core.dbs;
using Figurebox.Utils;
using Figurebox.Utils.MoH;

namespace Figurebox.core;

public class AW_City : City
{
    /// <summary>
    ///     拓展后的单位身份
    /// </summary>
    private static readonly AWUnitProfession[] ExtendUnitProfessions =
        (AWUnitProfession[])Enum.GetValues(typeof(AWUnitProfession));

    public int capital_tax_income;

    public int food_count_for_slaves_this_year;

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

    [MethodReplace(nameof(City.makeOwnKingdom))]
    public new Kingdom makeOwnKingdom()
    {
        Kingdom old_kingdom = kingdom;
        removeFromCurrentKingdom();
        Kingdom new_kingdom = World.world.kingdoms.makeNewCivKingdom(this);
        switchedKingdom();
        ((AW_Kingdom)kingdom).InheritPolicyFrom(old_kingdom as AW_Kingdom);
        return new_kingdom;
    }

    [MethodReplace(nameof(City.finishCapture))]
    public new void finishCapture(Kingdom pKingdom)
    {
        #region 原版代码

        this.clearCapture();
        this.recalculateNeighbourCities();
        using (ListPool<War> pWars = pKingdom.getWars())
        {
            Kingdom kingdom = this.findKingdomToJoinAfterCapture(pKingdom, pWars);
            if (!this.checkRebelWar(kingdom, pWars))
            {
                kingdom.data.timestamp_new_conquest = World.world.getCurWorldTime();
            }


            // 新增代码
            AW_War mohWar = null;
            foreach (War war in pWars)
            {
                if (war._asset == AssetManager.war_types_library.get("tianming"))
                {
                    mohWar = war as AW_War;
                    break; // 找到天命战争后，跳出循环
                }

                if (war._asset == AssetManager.war_types_library.get("conquest"))
                {
                    if (MoHTools.IsMoHKingdom(MoHTools.ConvertKtoAW(war.main_attacker)) &&
                        this == mohWar._attackerCapital)
                    {
                        // 天命国被反推丢失天命
                        MoHTools.Clear_MoHKingdom();
                        MoHTools.SetMoHKingdom(MoHTools.ConvertKtoAW(war.main_defender));
                        if (war.main_defender.king != null)
                        {
                            war.main_defender.king.addTrait("first");
                        }
                    }
                }
            }

            // 特定情况的操作

            // 继续原版逻辑
            this.joinAnotherKingdom(kingdom);
            this.removeSoldiers();

            // 天命战争的解决
            if (mohWar != null)
            {
                if (this == mohWar._attackerCapital || this == mohWar._defenderCapital)
                {
                    // 如果当前城市是攻击方或防守方的首都
                    mohWar.ResolveTianmingWar();
                }
            }
        }

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
        this.gold_out_buildings = this.buildings.Count / 2;
        this.gold_change = this.gold_in_tax - this.gold_out_army - this.gold_out_buildings - this.gold_out_homeless;
        int num = this.gold_change;
        if (num < 0)
        {
            num = 0;
        }

        this.data.storage.change("gold", num);
        this.updatePopPoints();
        if (professionsDict.Count == 0) updateCitizens();
        PayTax();                                               //交税
        CityPopRecordManager.NewCityPopCompositionRecord(this); //记录人口构成

        // 奴隶食物供给
        food_count_for_slaves_this_year = 0;
        if (professionsDict != null && professionsDict[AWUnitProfession.Slave.C()].Count > 0)
        {
            var total_food = data.storage.listFood.Sum(food => food.amount);

            food_count_for_slaves_this_year = (int)(total_food * 0.1f);
        }
    }

    public override void Dispose()
    {

        if (MoHTools._moh_cities.Contains(this))
        {
            MoHTools._moh_cities.Remove(this);
        }


        base.Dispose();
    }

}