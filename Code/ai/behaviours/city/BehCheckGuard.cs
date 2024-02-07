using System.Linq;
using ai.behaviours;
using Figurebox.constants;
using Figurebox.content;
using Figurebox.core;
using Figurebox.Utils;

namespace Figurebox.ai.behaviours.city;

/// <summary>
///     使用奴隶组织奴隶军
/// </summary>
public class BehCheckGuard : BehaviourActionCity
{
    public override BehResult execute(City pObject)
    {
        var city = (AW_City)pObject;
        if (city.groups.TryGetValue(UnitGroupTypeLibrary.guards.id, out AW_UnitGroup group))
        {
            findWarriorsForGroup(city, group);
            return BehResult.Continue;
        }

        if (city.kingdom.king != null && city.kingdom.king.unit_group == null)
        {
            //  Main.LogInfo("创建保镖" + "  " + city.name + "  " + city.kingdom.king.getName());
            city.CreateGroup(UnitGroupTypeLibrary.guards);
            findWarriorsForGroup(city, city.groups[UnitGroupTypeLibrary.guards.id]);

            return BehResult.Continue;
        }

        return base.execute(pObject);
    }

    private void findWarriorsForGroup(AW_City pCity, AW_UnitGroup pGroup)
    {
        var left_count = (int)(pGroup.asset.base_max_count * pCity.getArmyMaxTotalPercentage()) - pGroup.units.Count;
        if (left_count <= 0) return;
        if (pCity.isArmyFull())
        {
            return;
        }

        foreach (Actor unit in pCity.units)
        {
            if (unit.hasTrait("禁卫军"))
            {
                pGroup.addUnit(unit);
            }
        }

        foreach (Actor unit in pCity.professionsDict[AWUnitProfession.Unit.C()]
                                    .Where(unit => unit.unit_group == null)
                                    .Where(unit => unit.citizen_job == null)
                                    .Where(unit => unit.hasClan() && unit.getClan().data.id != pCity.kingdom.data.royal_clan_id))
        {
            pGroup.addUnit(unit);
            unit.addTrait("禁卫军");
            pCity.status.warriors_current++;
            unit.setProfession(UnitProfession.Warrior, false);
            unit.setCitizenJob(CitizenJobs.king_guard);
            ItemAsset wa = AssetManager.items.get("ji");
            ItemAsset aa = AssetManager.items.get("armor");
            string wm = "bronze";
            string am = "bronze";
            ItemData item = ItemGenerator.generateItem(wa, wm, World.world.mapStats.getCurrentYear(), unit.kingdom,
                                                       unit.getName(), 1, unit);
            item.modifiers.Clear();
            if (unit.equipment.getSlot(wa.equipmentType).data == null ||
                unit.equipment.getSlot(aa.equipmentType).data == null)
            {
                unit.equipment.getSlot(wa.equipmentType).setItem(item);
                item = ItemGenerator.generateItem(aa, am, World.world.mapStats.getCurrentYear(), unit.kingdom,
                                                  unit.getName(), 1, unit);
                item.modifiers.Clear();
                unit.equipment.getSlot(aa.equipmentType).setItem(item);
            }

            unit.dirty_sprite_item = true;
            unit.setStatsDirty();
            left_count--;
            if (left_count <= 0) break;
        }
    }
}