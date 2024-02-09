using Figurebox.abstracts;
using Figurebox.core;

namespace Figurebox.content
{
    class GodPowerLibrary : ExtendedLibrary<GodPower>
    {
        private static Kingdom SelectKingdom1 = null;
        private static Kingdom SelectKingdom2 = null;

        protected override void init()
        {
            GodPower xiapower = clone("spawn_xia", "_spawnActor");
            xiapower.name = "spawn_xia";
            xiapower.actor_asset_id = "unit_Xia";
            xiapower.click_action = AssetManager.powers.spawnUnit;


            GodPower vassal = new GodPower();
            vassal.id = "vassal";
            vassal.force_map_text = MapMode.Kingdoms;
            vassal.name = "vassal";
            vassal.unselectWhenWindow = true;
            vassal.path_icon = "ui/wars/war_vassal";
            vassal.click_special_action = new PowerActionWithID(vassal_click);
            add(vassal);
            GodPower vassal_remove = new GodPower();
            vassal_remove.id = "vassal_remove";
            vassal_remove.force_map_text = MapMode.Kingdoms;
            vassal_remove.name = "vassal_remove";
            vassal_remove.unselectWhenWindow = true;
            vassal_remove.path_icon = "ui/wars/war_independent";
            vassal_remove.click_special_action = new PowerActionWithID(vassal_remove_click);
            add(vassal_remove);

            add_map_mode_powers();
        }

        private void add_map_mode_powers()
        {
            add(new GodPower
            {
                id = "vassal_zones",
                name = "Vassal Layer",
                unselectWhenWindow = true,
                map_modes_switch = true,
                toggle_name = "map_vassal_zones",
                toggle_action = _ => MapModeManager.SetAllDirty()
            });
        }

        private static bool vassal_click(WorldTile pTile, string pPowerID)
        {
            if (pTile.zone.city == null)
            {
                return false;
            }

            if (pTile.zone.city.kingdom == null)
            {
                return false;
            }

            AW_Kingdom tilekingdom = pTile.zone.city.kingdom as AW_Kingdom;

            if (tilekingdom.IsVassal())
            {
                WorldTip.showNow($"{pTile.zone.city.kingdom.data.name} is Vassal", pTranslate: false, "top", 6f);
                return false;
            }

            if (SelectKingdom1 is null)
            {
                if (tilekingdom.IsSuzerain())
                {
                    WorldTip.showNow($"{pTile.zone.city.kingdom.data.name} is Lord", pTranslate: false, "top", 6f);
                    return false;
                }

                SelectKingdom1 = pTile.zone.city.kingdom;
                WorldTip.showNow($"Let {SelectKingdom1.data.name} to be who's vassal?", pTranslate: false, "top", 6f);
                return false;
            }

            if (SelectKingdom2 is null)
            {
                if (pTile.zone.city.kingdom == SelectKingdom1)
                {
                    WorldTip.showNow($"Don't choose the same country", pTranslate: false, "top", 6f);
                    SelectKingdom1 = null;
                    return false;
                }

                SelectKingdom2 = pTile.zone.city.kingdom;
                AW_Kingdom Select2 = SelectKingdom2 as AW_Kingdom;
                AW_Kingdom Select1 = SelectKingdom1 as AW_Kingdom;
                WorldTip.showNow($"{SelectKingdom2.data.name}!", pTranslate: false, "top", 6f);
                Select1.SetVassal(Select2);
                SelectKingdom1 = null;
                SelectKingdom2 = null;
                return true;
            }

            return true;
        }

        private static bool vassal_remove_click(WorldTile pTile, string pPowerID)
        {
            AW_Kingdom tilekingdom = pTile.zone.city.kingdom as AW_Kingdom;

            if (pTile.zone.city == null)
            {
                return false;
            }

            if (pTile.zone.city.kingdom == null)
            {
                return false;
            }

            if (tilekingdom.IsVassal())
            {
                // 这个王国是附庸，所以我们可以移除它的附庸状态
                tilekingdom.RemoveSuzerain();
                WorldTip.showNow($"The vassal status of {pTile.zone.city.kingdom.data.name} has been removed", false,
                                 "top", 6f);
                return true;
            }

            // 这个王国不是附庸，所以我们不能移除它的附庸状态
            WorldTip.showNow($"{pTile.zone.city.kingdom.data.name} is not a vassal", false, "top", 6f);
            return false;
        }
    }
}