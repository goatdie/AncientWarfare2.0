using System;
using System.Collections.Generic;
using Figurebox.abstracts;
using NCMS.Utils;
using ReflectionUtility;
using UnityEngine;

namespace Figurebox.content
{
    internal class ActorAssetLibrary : ExtendedLibrary<ActorAsset>
    {
        protected override void init()
        {
            ColorSetAsset Xia_default = new ColorSetAsset();
            Xia_default.id = "xia_default";
            Xia_default.shades_from = "#FFC984";
            Xia_default.shades_to = "#543E2C";
            Xia_default.is_default = true;
            AssetManager.skin_color_set_library.add(Xia_default);

            var XiaAsset = clone("unit_Xia", "unit_human");
            XiaAsset.nameLocale = "Xias";
            XiaAsset.body_separate_part_head = true;
            XiaAsset.heads = 11;
            //XiaAssetexture_heads = "t_Xia_heads";
            XiaAsset.oceanCreature = false;
            //XiaAsset.procreate = true;
            XiaAsset.nameTemplate = "Xia_name";
            XiaAsset.race = "Xia";
            XiaAsset.fmod_spawn = "event:/SFX/UNITS/Human/HumanSpawn";
            XiaAsset.fmod_attack = "event:/SFX/UNITS/Human/HumanAttack";
            XiaAsset.fmod_idle = "event:/SFX/UNITS/Human/HumanIdle";
            XiaAsset.fmod_death = "event:/SFX/UNITS/Human/HumanDeath";
            //XiaAssetexture_path = "t_Xia";
            //XiaAssetexture_heads = string.Empty;
            XiaAsset.base_stats[S.max_age] = 90f;
            XiaAsset.base_stats[S.max_children] = 6f;
            XiaAsset.icon = "iconXias";
            XiaAsset.color = Toolbox.makeColor("#33724D");
            XiaAsset.setBaseStats(100, 15, 45, 5, 90, 5);
            XiaAsset.base_stats[S.attack_speed] = 60f;
            XiaAsset.disableJumpAnimation = true;
            AssetManager.actor_library.addColorSet("xia_default");
            AssetManager.actor_library.loadShadow(XiaAsset);
            //XiaAssetraits.Add("zho");

            var babbyXia = clone("baby_Xia", "unit_Xia");
            babbyXia.body_separate_part_head = false;
            babbyXia.body_separate_part_hands = false;
            babbyXia.take_items = false;
            babbyXia.base_stats[S.speed] = 10f;
            // babbyXia.timeToGrow = 60;
            babbyXia.baby = true;
            babbyXia.animation_idle = "walk_3";
            babbyXia.growIntoID = "unit_Xia";
            AssetManager.actor_library.addTrait("peaceful");
            babbyXia.disableJumpAnimation = true;
            babbyXia.color_sets = XiaAsset.color_sets;
            AssetManager.actor_library.loadShadow(babbyXia);


        }

    }

}