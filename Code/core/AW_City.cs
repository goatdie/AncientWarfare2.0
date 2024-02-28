using System;
using System.Collections.Generic;
using System.Linq;
using Figurebox.attributes;
using Figurebox.constants;
using Figurebox.core.dbs;
using Figurebox.utils;
using Figurebox.utils.extensions;
using Figurebox.utils.MoH;
using UnityEngine;

namespace Figurebox.core;

public partial class AW_City : City
{
    /// <summary>
    ///     拓展后的单位身份
    /// </summary>
    private static readonly AWUnitProfession[] ExtendUnitProfessions =
        (AWUnitProfession[])Enum.GetValues(typeof(AWUnitProfession));

    public AW_CityDataAddition addition_data = new();
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
                AW_War AW_war = war as AW_War;
                if (war._asset == AssetManager.war_types_library.get("tianming"))
                {
                    mohWar = war as AW_War;
                    break; // 找到天命战争后，跳出循环
                }

                if (war._asset == AssetManager.war_types_library.get("reclaim"))
                {
                    AW_War recliamwar = war as AW_War;

                    if (HasKingdomRecoveredAllCities(recliamwar))
                    {
                        World.world.wars.endWar(war);
                    }
                }
                if (war._asset == AssetManager.war_types_library.get("vassal_war"))
                {
                    AW_War vassalwar = war as AW_War;
                    if (this == vassalwar._defenderCapital && kingdom == vassalwar.main_attacker)
                    {
                        AW_City cap = vassalwar._defenderCapital as AW_City;
                        TransferCities(vassalwar.main_attacker, vassalwar.main_defender, cap);
                        SetLoserAsVassal(vassalwar.main_defender, vassalwar.main_attacker);
                        World.world.wars.endWar(war);
                        return;
                    }
                    else if (this == vassalwar._attackerCapital && kingdom == vassalwar.main_defender)
                    {
                        AW_City cap = vassalwar._attackerCapital as AW_City;
                        TransferCities(vassalwar.main_defender, vassalwar.main_attacker, cap);
                        SetLoserAsVassal(vassalwar.main_attacker, vassalwar.main_defender);
                        World.world.wars.endWar(war);
                        return;
                    }


                }



                if (MoHTools.IsMoHKingdom(war.main_attacker.AW()) &&
                    this == AW_war?._attackerCapital)
                {
                    // 天命国被反推丢失天命
                    MoHTools.Clear_MoHKingdom();
                    MoHTools.SetMoHKingdom(war.main_defender.AW());
                    if (war.main_defender.king != null)
                    {
                        war.main_defender.king.addTrait("first");
                    }
                }
                else if (MoHTools.IsMoHKingdom(war.main_defender.AW()) &&
                     this == AW_war?._defenderCapital)
                {
                    MoHTools._moh_cities.Clear();
                    MoHTools._moh_cities.AddRange(MoHTools.MoHKingdom.cities);
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

    [MethodReplace(nameof(City.joinAnotherKingdom))]
    public new void joinAnotherKingdom(Kingdom pKingdom)
    {
        RecordOccupation(kingdom.AW()); //记录脱离母国统治
        this.removeFromCurrentKingdom();
        this.setKingdom(pKingdom, true);
        this.switchedKingdom();
        pKingdom.capturedFrom(kingdom.AW());
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
        updateTechResearch();
    }

    [MethodReplace(nameof(City.updateCapture))]
    private new void updateCapture(float pElapsed)
    {
        if (this.last_ticks == 0 && !this.isGettingCaptured())
        {
            return;
        }

        if ((int)this._capture_ticks > this.last_ticks)
        {
            this.last_ticks++;
        }
        else if ((int)this._capture_ticks < this.last_ticks)
        {
            this.last_ticks--;
        }
        last_ticks = Mathf.Clamp(last_ticks, 0, 100);

        if (this.captureTimer > 0f)
        {
            this.captureTimer -= pElapsed;
            return;
        }

        this.captureTimer = 0.1f;
        Kingdom kingdom = null;
        foreach (Kingdom key in this._capturing_units.Keys)
        {
            if (kingdom == null)
            {
                kingdom = key;
            }
            else if (this._capturing_units[key] > this._capturing_units[kingdom])
            {
                kingdom = key;
            }
        }

        if (kingdom == null)
        {
            this._capture_ticks -= 0.5f;
            if (this._capture_ticks <= 0f)
            {
                this.clearCapture();
            }

            return;
        }

        bool flag = false;
        if (this._capturing_units.ContainsKey(this.kingdom) && this._capturing_units[this.kingdom] > 0 &&
            this.getArmy() > 0)
        {
            flag = true;
        }

        if (this.being_captured_by != null && !this.being_captured_by.isAlive())
        {
            this.being_captured_by = null;
        }

        bool flag2 = false;
        if (this.kingdom == kingdom)
        {
            flag2 = true;
        }

        if (flag && this._capturing_units.Count == 1)
        {
            flag2 = true;
        }

        // 新增逻辑：判断内战情况下的加速占领
        bool isRebelCivilWar = IsRebelCivilWar(kingdom.AW());
        float captureIncrement = isRebelCivilWar ? 8.0f : 1.0f;

        if (flag2)
        {
            this._capture_ticks -= captureIncrement;
            if (this._capture_ticks <= 0f)
            {
                this.clearCapture();
            }

            return;
        }

        if (kingdom.isEnemy(this.kingdom) && (!flag || this._capture_ticks < 5f))
        {
            if (this.being_captured_by == null || this.being_captured_by == kingdom)
            {
                this._capture_ticks += captureIncrement + pElapsed;
                this.being_captured_by = kingdom;
                if (this._capture_ticks >= 100f)
                {
                    this.finishCapture(kingdom);
                    return;
                }
            }
            else if (kingdom.isEnemy(this.being_captured_by))
            {
                this._capture_ticks -= 0.5f * captureIncrement;
                if (this._capture_ticks <= 0f)
                {
                    this.clearCapture();
                    return;
                }
            }
            else
            {
                this._capture_ticks += captureIncrement + pElapsed;
                if (this._capture_ticks >= 100f)
                {
                    this.finishCapture(this.being_captured_by);
                }
            }
        }
    }

    private bool IsRebelCivilWar(AW_Kingdom attacker)
    {
        return kingdom != null && kingdom.AW().Rebel && attacker.Rebel;
    }

    private void updateCaptureForRebelCivilWar(float pElapsed)
    {
        // 特定于Rebel之间内战情况下的占领逻辑
        // 比如加速占领速度
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