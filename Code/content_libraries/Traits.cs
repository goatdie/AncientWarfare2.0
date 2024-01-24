using System;
using System.Collections.Generic;
using System.Linq;
using Figurebox.constants;
using Figurebox.core;
using Figurebox.Utils;
using Figurebox.Utils.MoH;

namespace Figurebox
{
    class Traits
    {
        public static Dictionary<string, int> LastUsurpationYears = new Dictionary<string, int>();

        private readonly HashSet<ActorTrait> social_identity_traits = new();

        internal void init()
        {
            /*BaseStatAsset mod_health =  AssetManager.base_stats_library.get("mod_health");
              BaseStatAsset stewardship =  AssetManager.base_stats_library.get("stewardship");
              BaseStatAsset diplomacy =  AssetManager.base_stats_library.get("diplomacy");
              BaseStatAsset warfare =  AssetManager.base_stats_library.get("warfare");
              BaseStatAsset intelligence =  AssetManager.base_stats_library.get("intelligence");*/
            add_social_identities();

            void add_and_unlock_trait(ActorTrait trait)
            {
                AssetManager.traits.add(trait);
                PlayerConfig.unlockTrait(trait.id);
            }

            ActorTrait Figure = new ActorTrait
            {
                id = "figure",
                path_icon = "ui/Icons/traits/iconfigure",
                birth = 0f,
                inherit = 0f,
                group_id = TraitGroups.aw2
            };
            BaseStatAsset mod_health = AssetManager.base_stats_library.get("mod_health");
            BaseStatAsset stewardship = AssetManager.base_stats_library.get("stewardship");
            add_and_unlock_trait(Figure);
            Figure.base_stats[S.mod_health] = +15f;
            Figure.base_stats[S.stewardship] = +10;

            ActorTrait zhuhou = new ActorTrait
            {
                id = "zhuhou",
                path_icon = "ui/Icons/traits/iconzhuhou",
                birth = 0f,
                inherit = 0f,
                group_id = TraitGroups.social_identity_group
            };

            add_and_unlock_trait(zhuhou);
            zhuhou.base_stats[S.mod_health] = +5f;
            zhuhou.base_stats[S.stewardship] = +5;
            //zhuhou.action_special_effect = (WorldAction)Delegate.Combine(zhuhou.action_special_effect, new WorldAction(Actionlib.checktianming));
            zhuhou.action_special_effect = (WorldAction)Delegate.Combine(zhuhou.action_special_effect,
                new WorldAction(Actionlib.checkkingleft));
            ActorTrait tianming = new ActorTrait
            {
                id = "天命",
                path_icon = "ui/Icons/traits/iconTianming",
                inherit = 0f,
                birth = 0f,
                group_id = TraitGroups.aw2
                //opposite = "shunming,yuming",
            };

            tianming.base_stats[S.stewardship] += 150;
            tianming.base_stats[S.diplomacy] += 15;
            tianming.base_stats[S.warfare] += 14;
            tianming.base_stats[S.intelligence] += 14;
            tianming.action_special_effect =
                (WorldAction)Delegate.Combine(tianming.action_special_effect, new WorldAction(Actionlib.checkP));
            add_and_unlock_trait(tianming);
            ActorTrait first = new ActorTrait
            {
                id = "first",
                path_icon = "ui/Icons/traits/iconfirst",
                inherit = 0f,
                birth = 0f,
                group_id = TraitGroups.aw2
                //opposite = "shunming,yuming",
            };
            first.action_special_effect =
                (WorldAction)Delegate.Combine(first.action_special_effect, new WorldAction(tianmingP));
            first.base_stats[S.diplomacy] += 15;
            first.base_stats[S.warfare] += 14;
            first.base_stats[S.intelligence] += 14;
            add_and_unlock_trait(first);
            ActorTrait formerking = new ActorTrait
            {
                id = "formerking",
                path_icon = "ui/Icons/traits/iconformerking",
                birth = 0f,
                inherit = 0f,
                group_id = TraitGroups.aw2
            };
            add_and_unlock_trait(formerking);
            formerking.action_special_effect = (WorldAction)Delegate.Combine(formerking.action_special_effect,
                new WorldAction(Actionlib.former));
            //formerking.baseStats.mod_health = +5f;
            // formerking.baseStats.stewardship = +1;
            ActorTrait jinwei = new ActorTrait
            {
                id = "禁卫军",
                path_icon = "ui/Icons/traits/iconjinwei",
                // opposite = "将领",
                inherit = 0f,
                birth = 0f,
                group_id = TraitGroups.aw2
            };
            jinwei.base_stats[S.scale] += 0.03f;
            jinwei.base_stats[S.mod_health] = +2f;
            jinwei.base_stats[S.damage] += 25;
            jinwei.base_stats[S.speed] += 30f;
            //jinwei.baseStats.attackSpeed = +5;
            jinwei.base_stats[S.knockback_reduction] += 100f;
            //jinwei.action_special_effect = (WorldAction)Delegate.Combine(jinwei.action_special_effect, new WorldAction(jinweiP));
            add_and_unlock_trait(jinwei);
            ActorTrait rebel = new ActorTrait
            {
                id = "rebel",
                path_icon = "ui/Icons/traits/iconrebel",
                birth = 0f,
                inherit = 0f,
                sameTraitMod = 20,
                group_id = TraitGroups.aw2
            };
            add_and_unlock_trait(rebel);
            rebel.base_stats[S.mod_health] = +2f;
            rebel.base_stats[S.diplomacy] += 35;
            rebel.base_stats[S.stewardship] += 35;
            rebel.base_stats[S.warfare] += 4;
            rebel.action_special_effect =
                (WorldAction)Delegate.Combine(rebel.action_special_effect, new WorldAction(Actionlib.rebelkingdom));
        }

        private void add_social_identities()
        {
            var slave = new ActorTrait();
            slave.id = nameof(slave);
            slave.path_icon = "ui/policy/start_slaves";
            slave.birth = 0;
            slave.inherit = 100;
            slave.special_effect_interval = 3;
            slave.action_special_effect = (pTarget, pTile) =>
            {
                pTarget.a.setProfession(AWUnitProfession.Slave.C());
                return true;
            };
            add_social_identity_trait(slave);
        }

        private void add_social_identity_trait(ActorTrait pActorTrait)
        {
            pActorTrait.needs_to_be_explored = false;
            pActorTrait.unlocked_with_achievement = false;
            pActorTrait.group_id = TraitGroups.social_identity_group;
            AssetManager.traits.add(pActorTrait);
            var opposite_traits = pActorTrait.oppositeArr == null
                ? new List<string>()
                : new List<string>(pActorTrait.oppositeArr);

            foreach (var trait in social_identity_traits)
            {
                if (trait.oppositeArr != null && trait.oppositeArr.Contains(pActorTrait.id)) continue;
                switch (trait.oppositeArr)
                {
                    case null when string.IsNullOrEmpty(trait.opposite):
                        trait.oppositeArr = new[] { pActorTrait.id };
                        continue;
                    case null:
                        trait.oppositeArr = trait.opposite.Split(',');
                        break;
                }

                var new_list = new List<string>(trait.oppositeArr) { pActorTrait.id };
                trait.oppositeArr = new_list.ToArray();
            }

            pActorTrait.oppositeArr = opposite_traits.ToArray();
            social_identity_traits.Add(pActorTrait);
        }

        public static bool HasEnoughTimePassedSinceLastUsurpation(Actor pActor)
        {
            int currentYear = World.world.mapStats.getCurrentYear();
            string kingdomId = pActor.data.id;

            // 如果这个王国从未发生过篡位事件，那么可以发生篡位
            if (!LastUsurpationYears.ContainsKey(kingdomId))
            {
                return true;
            }

            int lastUsurpationYear = LastUsurpationYears[kingdomId];
            return currentYear - lastUsurpationYear >= 10; // 假设篡位事件的冷却期为10年
        }

        private static bool basePlotChecks(Actor pActor, PlotAsset pPlotAsset)
        {
            if (pActor == null || pPlotAsset == null) return false;

            if (!World.world.worldLaws.world_law_rebellions.boolVal || !(pPlotAsset.checkInitiatorPossible(pActor) &&
                                                                         pPlotAsset.check_launch(pActor,
                                                                             pActor.kingdom)))
            {
                return false;
            }

            return true;
        }

        public static bool tryPlotusurpation(Actor pActor, PlotAsset pPlotAsset)
        {
            if (!basePlotChecks(pActor, pPlotAsset))
                //Debug.Log("发动篡位失败出检查不过");
                return false;

            if (!HasEnoughTimePassedSinceLastUsurpation(pActor)) return false;

            Plot plot = World.world.plots.newPlot(pActor, pPlotAsset);
            plot.rememberInitiators(pActor);
            // Debug.Log("发动篡位");
            /* if (plot!=null&&!plot.checkInitiatorAndTargets())
             {
                 Debug.Log("usurpation is missing start requirements");
                 return false;
             }
 */
            return true;
        }

        public static bool tianmingP(BaseSimObject pTarget, WorldTile pTile = null)
        {
            if (pTarget == null || pTarget.a == null || !pTarget.isAlive()) return false;
            Actor a = pTarget.a;
            int tianmingCtr = 0;
            Race race = AssetManager.raceLibrary.get(a.asset.race);
            ActorContainer actorContainer = race.units;
            var Units = actorContainer.getSimpleList();
            AW_Kingdom awKingdom = a.kingdom as AW_Kingdom;

            if (awKingdom.FomerMoh)
            {
                a.removeTrait("first");
                return false;
            }
            //Debug.Log("已经有天命了");
            if (MoHTools.IsMoHKingdom(awKingdom))
            {
                return false;
            }

            if (a.getAge() > 17 && a.kingdom != null && a.city != null)
            {
                //Debug.Log("执行");
                if (a.kingdom.king == null && a.getAge() > 15)
                {
                    awKingdom.clearKingData();
                    awKingdom.setKing(a);
                    WorldLog.logNewKing(awKingdom);

                    return false;
                }

                if (a.kingdom.king != null && a.kingdom.king != a && a.hasTrait("zhuhou"))
                {
                    a.removeTrait("first");
                }
                else if (a.kingdom.king != null && a.kingdom.king != a && !a.hasPlots())
                {
                    tryPlotusurpation(a, AssetManager.plots_library.get("historical_character_usurpation"));
                    //  WorldLog.logKingLeft(a.kingdom, a.kingdom.king);
                    //a.kingdom.CallMethod("removeKing");
                    //  a.kingdom.king.city.removeCitizen(a.kingdom.king, false, AttackType.Other);
                    //a.kingdom.clearKingData();
                    // a.kingdom.setKing(a);
                    //  WorldLog.logNewKing(a.kingdom);
                }


                if (a.kingdom.king != null && a.kingdom.king != a && a.hasPlots())
                {
                    foreach (City city in a.kingdom.cities)
                    {
                        if (!city.isHappy() && !(city.leader == null) && city.leader.isAlive())
                        {
                            List<Plot> actorPlots = World.world.plots.getPlotsFor(a, true);
                            foreach (Plot plot in actorPlots)
                            {
                                plot.addSupporter(city.leader);
                            }
                        }
                    }
                }
            }

            foreach (var unit in Units)
            {
                if (unit.hasTrait("first"))
                {
                    tianmingCtr++;
                }

                if (tianmingCtr > 1)
                {
                    unit.removeTrait("first");
                }
            }


            return true;
        }
    }
}