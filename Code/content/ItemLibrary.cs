using System;
using System.Collections.Generic;
using Figurebox.abstracts;
using Figurebox.Utils;
using HarmonyLib;
using ReflectionUtility;
using UnityEngine;

namespace Figurebox.content
{
    class ItemLibrary : ExtendedLibrary<ItemAsset>
    {

        protected override void init()
        {
            weapon_info();
            let_the_actor_use_weapon();
            displaying_weapon_on_map();
        }
        private void weapon_info()
        {
            ItemAsset ji = clone("ji", "sword");

            ji.name_templates = Toolbox.splitStringIntoList(new string[]
            {
                "sword_name#30", "sword_name_king#3", "weapon_name_city", "weapon_name_kingdom", "weapon_name_culture", "weapon_name_enemy_king", "weapon_name_enemy_kingdom"

            });
            ji.path_icon = "ui/Icons/items/icon_ji";
            ji.id = "ji";
            add(ji);
            ji.materials = List.Of<string>(new string[]
            {
                "bronze", "copper"
            });
            ji.base_stats[S.damage] = 10;
            ji.base_stats[S.attack_speed] = -1;
            ji.equipment_value = 800;
            ji.base_stats[S.knockback_reduction] += 0.1f;
            ji.base_stats[S.critical_chance] = 0.03f;

            //ji.base_stats[S.damage]CritMod = 1f;
            ji.base_stats[S.targets] = 1;
            ji.path_slash_animation = "qing";
            ji.equipmentType = EquipmentType.Weapon;
            ji.name_class = "item_class_weapon";
            ji.action_attack_target = (AttackAction)Delegate.Combine(ji.action_attack_target, new AttackAction(qingAttack));
            ItemAsset ge = clone("ge", "sword");

            ji.name_templates = Toolbox.splitStringIntoList(new string[]
            {
                "sword_name#30", "sword_name_king#3", "weapon_name_city", "weapon_name_kingdom", "weapon_name_culture", "weapon_name_enemy_king", "weapon_name_enemy_kingdom"

            });
            ge.path_icon = "ui/Icons/items/icon_ge";
            ge.id = "ge";
            add(ge);
            ge.materials = List.Of<string>(new string[]
            {
                "bronze", "copper"
            });
            ge.base_stats[S.damage] = 10;
            ge.base_stats[S.attack_speed] = -1;
            ge.base_stats[S.knockback_reduction] += 0.1f;
            ge.equipment_value = 800;
            ge.base_stats[S.critical_chance] = 0.06f;
            ge.base_stats[S.targets] = 1;
            ge.path_slash_animation = "qing";
            ge.equipmentType = EquipmentType.Weapon;
            ge.name_class = "item_class_weapon";
            ge.action_attack_target = (AttackAction)Delegate.Combine(ge.action_attack_target, new AttackAction(qingAttack));

            ItemAsset binfa = clone("binfa", "_accessory");
            binfa.path_icon = "ui/Icons/items/icon_binfa";
            binfa.name_class = "item_class_accessory";
            binfa.id = "binfa";
            binfa.name_templates = List.Of<string>(new string[]
            {
                "amulet_name"
            });
            binfa.quality = ItemQuality.Legendary;
            binfa.equipmentType = EquipmentType.Amulet;
            binfa.materials = List.Of<string>(new string[]
            {
                "bronze"
            });
            binfa.base_stats[S.warfare] = 20;
            binfa.base_stats[S.critical_chance] += 3f;
            binfa.equipment_value = 2000;
            add(binfa);


        }
        private void let_the_actor_use_weapon()
        {
            Race human = AssetManager.raceLibrary.get("human");
            Race orc = AssetManager.raceLibrary.get("orc");
            Race dwarf = AssetManager.raceLibrary.get("dwarf");
            Race elf = AssetManager.raceLibrary.get("elf");

            for (int i = 0; i < 7; i++)
            {
                orc.preferred_weapons.Add("ji");
                human.preferred_weapons.Add("ji");
                dwarf.preferred_weapons.Add("ji");
                elf.preferred_weapons.Add("ji");
                orc.preferred_weapons.Add("ge");
                human.preferred_weapons.Add("ge");
                dwarf.preferred_weapons.Add("ge");
                elf.preferred_weapons.Add("ge");
                orc.preferred_weapons.Add("binfa");
                human.preferred_weapons.Add("binfa");
                dwarf.preferred_weapons.Add("binfa");
                elf.preferred_weapons.Add("binfa");



            }
            ResourceAsset gold = AssetManager.resources.get("gold");
            gold.maximum = 50000;
        }
        public static bool qingAttack(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null)
        {
            if (pTarget != null)
            {
                Actor a = Reflection.GetField(pTarget.GetType(), pTarget, "a") as Actor;
                if (a == null)
                {
                    return false;
                }
                a.addStatusEffect("qing", 1f);
            }
            return true;
        }
        private void displaying_weapon_on_map()
        {
            ActorAnimationLoader aal = new ActorAnimationLoader();
            Dictionary<string, Sprite> dictItems = Traverse.Create(aal).Field("dictItems").GetValue() as Dictionary<string, Sprite>;
            Sprite[] addSprites = ResourcesHelper.loadAllSprite("items/", 0.8f, 0.3f, true);
            foreach (Sprite sprite in addSprites)
            {
                dictItems.Add(sprite.name, sprite);
            }
            Traverse.Create(aal).Field("dictItems").SetValue(dictItems);
        }
    }
}