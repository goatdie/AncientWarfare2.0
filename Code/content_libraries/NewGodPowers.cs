using Figurebox.core;

namespace Figurebox
{
    class NewGodPowers
    {
        public static Kingdom SelectKingdom1 = null;
        public static Kingdom SelectKingdom2 = null;
        public static void init()
        {
            initPowers();
            // initStats();
        }



        public static Actor spawnUnit(WorldTile pTile, string pPowerID)
        {
            GodPower godPower = AssetManager.powers.get(pPowerID);
            MusicBox.playSound("event:/SFX/UNIQUE/SpawnWhoosh", (float)pTile.pos.x, (float)pTile.pos.y, false, false);
            if (godPower.id == SA.sheep && pTile.Type.lava)
            {
                AchievementLibrary.achievementSacrifice.check(null, null, null);
            }
            EffectsLibrary.spawn("fx_spawn", pTile, null, null, 0f, -1f, -1f);
            string text;
            if (godPower.actor_asset_ids.Count > 0)
            {
                text = godPower.actor_asset_ids.GetRandom<string>();
            }
            else
            {
                text = godPower.actor_asset_id;
            }
            Actor actor = World.world.units.spawnNewUnit(text, pTile, true, godPower.actorSpawnHeight);
            actor.addTrait("miracle_born", false);
            actor.data.age_overgrowth = 18;
            actor.data.had_child_timeout = 8f;
            return actor;
        }


        public static void initPowers()
        {


            GodPower xiapower = AssetManager.powers.clone("spawn_xia", "_spawnActor");
            xiapower.name = "spawn_xia";
            xiapower.actor_asset_id = "unit_Xia";
            xiapower.click_action = action_spawn_xia;
            /*xiapower.click_action = new PowerActionWithID((WorldTile pTile, string pPower)
                                =>
            {
                return (bool)AssetManager.powers.CallMethod("spawnUnit", pTile, pPower);
            });*/
            AssetManager.powers.add(xiapower);

            //GodPower power1 = new GodPower();
            //power1.id = "Toggle";
            // power1.name = "Toggle";
            // power1.rank = PowerRank.Rank0_free;
            //  power1.click_action = new PowerActionWithID((WorldTile pTile, string pPower));
            //  AssetManager.powers.add(xiapower);

            GodPower vassal = new GodPower();
            vassal.id = "vassal";
            vassal.force_map_text = MapMode.Kingdoms;
            vassal.name = "vassal";
            vassal.unselectWhenWindow = true;
            vassal.path_icon = "ui/wars/war_vassal";
            vassal.click_special_action = new PowerActionWithID(vassal_click);
            AssetManager.powers.add(vassal);
            GodPower vassal_remove = new GodPower();
            vassal_remove.id = "vassal_remove";
            vassal_remove.force_map_text = MapMode.Kingdoms;
            vassal_remove.name = "vassal_remove";
            vassal_remove.unselectWhenWindow = true;
            vassal_remove.path_icon = "ui/wars/war_independent";
            vassal_remove.click_special_action = new PowerActionWithID(vassal_remove_click);
            AssetManager.powers.add(vassal_remove);


        }
        public static bool action_spawn_xia(WorldTile pTile, string pPowerID)
        {

            Actor actor = spawnUnit(pTile, pPowerID);
            if (actor == null)
            {
                return false;
            }
            return true;
        }
        public static bool vassal_click(WorldTile pTile, string pPowerID)
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
                Select2.SetVassal(Select1);
                SelectKingdom1 = null;
                SelectKingdom2 = null;
                return true;
            }

            return true;
        }
        public static bool vassal_remove_click(WorldTile pTile, string pPowerID)
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
                WorldTip.showNow($"The vassal status of {pTile.zone.city.kingdom.data.name} has been removed", false, "top", 6f);
                return true;
            }
            // 这个王国不是附庸，所以我们不能移除它的附庸状态
            WorldTip.showNow($"{pTile.zone.city.kingdom.data.name} is not a vassal", false, "top", 6f);
            return false;
        }
    }
}