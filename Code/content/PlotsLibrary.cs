using System.Collections.Generic;
using System.Linq;
using Figurebox.abstracts;
using Figurebox.constants;
using Figurebox.core;
using Figurebox.core.events;
using Figurebox.utils;
using Figurebox.utils.extensions;
using Figurebox.utils.MoH;
using ReflectionUtility;
using UnityEngine;

namespace Figurebox.content
{
    public class PlotsLibrary : ExtendedLibrary<PlotAsset>
    {
        // public static PlotAsset reclaim_war = null;

        private static double last_Timer = 0.0f;
        private static double interval   = 1000.0f;

        public static PlotAsset active_vassal;

        protected override void init()
        {
            replace(new AW_PlotAsset(get("new_war"))
            {
                plot_type = PlotType.war
            });
            // "Reclaim War" PlotAsset Initialization

            #region reclaimWarPlotAsset

            var reclaim = new AW_PlotAsset();
            reclaim.id = "reclaim_war";
            reclaim.plot_type = PlotType.war;
            reclaim.path_icon = "ui/plots/plot_reclaim";
            reclaim.translation_key = "plot_reclaim_war";
            reclaim.description = "plot_description_reclaim_war";
            reclaim.check_initiator_actor = true;
            reclaim.check_initiator_city = true;
            reclaim.check_initiator_kingdom = true;
            reclaim.check_target_kingdom = true;
            AssetManager.plots_library.add(reclaim);
            reclaim.check_supporters = global::PlotsLibrary.checkMembersToRemoveDefault;
            reclaim.check_should_continue = ((Plot pPlot) =>
                    pPlot.initiator_actor.isAlive()                                          &&
                    pPlot.initiator_kingdom.getArmy() + 20 >= pPlot.target_kingdom.getArmy() &&
                    !pPlot.initiator_actor.kingdom.asset.mad                                 &&
                    !pPlot.initiator_kingdom.hasEnemies()                                    &&
                    (pPlot.initiator_kingdom.getAlliance() == null ||
                     !pPlot.initiator_kingdom.getAlliance().kingdoms_hashset.Contains(pPlot.target_kingdom))
                );


            reclaim.check_launch = delegate(Actor pActor, Kingdom pKingdom)
            {
                // 检查是否失去了领土
                if (!DiplomacyHelpers.isWarNeeded(pKingdom))
                {
                    return false;
                }

                if (pKingdom.AW().IsVassal())
                {
                    return false;
                }
                /*if (!HasLostTerritories(pKingdom))
                {
                     Debug.Log("没有可收回领土"+pKingdom.data.name);

                    return false;
                }*/

                // 检查是否已经有正在进行的收复战
                if (!pKingdom.hasAlliance()) return true;
                foreach (Kingdom kingdom in pKingdom.getAlliance().kingdoms_hashset)
                {
                    if (kingdom == pKingdom || kingdom.king == null) continue;
                    List<Plot> plotsFor = World.world.plots.getPlotsFor(kingdom.king, true);
                    if (plotsFor == null) continue;
                    if (plotsFor.Any(plot => plot.isSameType(AssetManager.plots_library.get("reclaim_war"))))
                    {
                        return false;
                    }
                }

                return true;
            };


            reclaim.action = delegate(Plot pPlot)
            {
                World.world.diplomacy.startWar(pPlot.initiator_kingdom, pPlot.target_kingdom,
                                               AssetManager.war_types_library.get("reclaim"));
                return true;
            };

            reclaim.plot_power = ((Actor pActor) => (int)pActor.stats[S.warfare]);
            reclaim.cost = 1000;
            //reclaim_war = this.add(reclaim);

            #endregion

            #region historicalCharacterUsurpationPlotAsset

            PlotAsset usurpation = new PlotAsset();
            usurpation.id = "historical_character_usurpation";
            usurpation.path_icon = "ui/plots/plot_usurpation"; // 你需要提供一个图标的路径
            usurpation.translation_key = "plot_historical_character_usurpation";
            usurpation.description = "plot_description_historical_character_usurpation";
            usurpation.check_initiator_actor = true;
            usurpation.check_initiator_city = false;
            usurpation.check_initiator_kingdom = true;
            usurpation.check_target_kingdom = false;
            usurpation.check_other_plots = delegate(Actor pActor, Plot pPlot)
            {
                Kingdom kingdom = pActor.kingdom;
                using (HashSet<Actor>.Enumerator enumerator = pPlot.supporters.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        if (DiplomacyHelpers.areKingdomsClose(enumerator.Current.kingdom, kingdom))
                        {
                            return true;
                        }
                    }
                }

                return false;
            };
            AssetManager.plots_library.add(usurpation);
            usurpation.plot_power = ((Actor pActor) => (int)pActor.stats[S.diplomacy]);
            usurpation.check_supporters = delegate(Actor pActor, Plot pPlot)
            {
                if (global::PlotsLibrary.checkMembersToRemoveDefault(pActor, pPlot))
                {
                    return true;
                }

                Kingdom kingdom = pActor.kingdom;
                return !kingdom.isAlive() || kingdom.hasEnemies() || kingdom.cities.Count == 0 || kingdom.hasAlliance();
            };
            usurpation.check_launch = (PlotCheckerLaunch)((pActor, pKingdom) =>
            {
                if (pKingdom.hasEnemies() || pActor == pKingdom.king)
                {
                    //Debug.Log("不是老资历之不乱");
                    return false;
                }

                if (pKingdom.king                != null && pActor.isCityLeader() &&
                    pKingdom.king.getInfluence() > pActor.getInfluence())
                {
                    return false;
                }


                if (pActor.getAge() <= 17 && pKingdom == null && pActor.city == null)
                {
                    Debug.Log("不是老资历之很多null");
                    return false;
                }

                return true;
            });
            usurpation.check_should_continue = delegate(Plot pPlot)
            {
                if (!pPlot.initiator_actor.isAlive() && pPlot.initiator_actor.kingdom.asset.mad)
                {
                    return false;
                }

                foreach (Actor actor in pPlot.supporters)
                {
                    Kingdom kingdom = actor.kingdom;
                    if (!kingdom.isAlive())
                    {
                        return false;
                    }

                    if (kingdom.hasEnemies())
                    {
                        return false;
                    }
                }

                return true;
            };
            usurpation.action = (PlotAction)(pPlot =>
            {
                // Update the year of the last usurpation for the initiator of the plot
                string initiatorId = pPlot.initiator_actor.data.id;
                TraitLibrary.LastUsurpationYears[initiatorId] = World.world.mapStats.getCurrentYear();

                Actor pActor = pPlot.initiator_actor;
                Kingdom pKingdom = pPlot.initiator_kingdom;
                Actor oldTarget;

                if (IsLeaderOfCity(pActor))
                {
                    // Usurp the kingdom
                    oldTarget = pKingdom.king;
                    PerformKingdomUsurpation(pActor, pKingdom);
                }
                else
                {
                    // Usurp the city
                    oldTarget = pActor.city.leader;
                    PerformCityUsurpation(pActor);
                }

                return true;
            });


            usurpation.cost = 1200;


            // 你需要提供这个情节的其他属性和方法，例如check_supporters, check_should_continue, check_launch, action, plot_power, cost等

            #endregion

            #region VassalWarPlotAsset

            var vassalWar = new AW_PlotAsset();
            vassalWar.id = "vassal_war";
            vassalWar.plot_type = PlotType.war;
            vassalWar.path_icon = "ui/plots/plot_vassal_war";
            vassalWar.translation_key = "plot_vassal_war";
            vassalWar.description = "plot_description_vassal_war";
            vassalWar.check_initiator_actor = true;
            vassalWar.check_initiator_city = true;
            vassalWar.check_initiator_kingdom = true;
            vassalWar.check_target_kingdom = true;
            vassalWar.check_other_plots = delegate(Actor pActor, Plot pPlot)
            {
                return pPlot.supporters.Any(
                    actor => DiplomacyHelpers.areKingdomsClose(actor.kingdom, pActor.kingdom));
            };
            AssetManager.plots_library.add(vassalWar);
            vassalWar.check_supporters = global::PlotsLibrary.checkMembersToRemoveDefault;
            vassalWar.check_should_continue = ((Plot pPlot) =>
                    pPlot.initiator_actor.isAlive()                                     &&
                    !pPlot.initiator_actor.kingdom.asset.mad                            &&
                    pPlot.initiator_kingdom.getArmy() >= pPlot.target_kingdom.getArmy() &&
                    !pPlot.initiator_kingdom.hasEnemies()                               &&
                    (!pPlot.initiator_kingdom.hasAlliance() ||
                     pPlot.initiator_kingdom.getAlliance() != pPlot.target_kingdom.getAlliance()) &&
                    !pPlot.target_kingdom.AW().IsVassal()                                         &&
                    !pPlot.target_kingdom.AW().IsSuzerain()
                );


            vassalWar.check_launch = delegate(Actor pActor, Kingdom pKingdom)
            {
                // 检查是否已经是附庸国
                if (pKingdom.AW().IsVassal())
                {
                    return false;
                }

                if (pKingdom.AW().addition_data.Title == AW_KingdomDataAddition.KingdomTitle.Baron)
                {
                    return false;
                }

                if (!DiplomacyHelpers.isWarNeeded(pKingdom))
                {
                    return false;
                }

                // 检查是否已经有正在进行的附庸战
                if (!pKingdom.hasAlliance()) return true;
                foreach (Kingdom kingdom in pKingdom.getAlliance().kingdoms_hashset)
                {
                    if (kingdom == pKingdom || kingdom.king == null) continue;
                    List<Plot> plotsFor = World.world.plots.getPlotsFor(kingdom.king, true);
                    if (plotsFor == null) continue;
                    if (plotsFor.Any(plot => plot.isSameType(AssetManager.plots_library.get("vassal_war"))))
                    {
                        return false;
                    }
                }

                return true;
            };

            vassalWar.action = delegate(Plot pPlot)
            {
                World.world.diplomacy.startWar(pPlot.initiator_kingdom, pPlot.target_kingdom,
                                               AssetManager.war_types_library.get("vassal_war"));
                WorldLogTools.LogVassalWarStart(pPlot.target_kingdom, pPlot.initiator_kingdom);

                return true;
            };

            vassalWar.plot_power = ((Actor pActor) => (int)pActor.stats[S.warfare]);
            vassalWar.cost = 1000;

            #endregion


            #region 主动附庸

            active_vassal = new PlotAsset();
            active_vassal.id = "active_vassal";
            active_vassal.path_icon = "ui/plots/plot_vassal_war";
            active_vassal.translation_key = "plot_active_vassal";
            active_vassal.description = "plot_description_active_vassal";
            active_vassal.check_initiator_actor = true;
            active_vassal.check_initiator_kingdom = true;
            active_vassal.check_target_kingdom = true;
            active_vassal.check_launch = (pActor, pKingdom) =>
            {
                // 仅在战争期间或即将被宣战时允许发动
                return World.world.plots.list.Any(plot => plot.getAsset().AW()?.plot_type == PlotType.war &&
                                                          plot.target_kingdom             == pKingdom)
                       || World.world.wars.hasWars(pKingdom);
            };
            active_vassal.check_should_continue = ActiveVassalCheckContinue;
            active_vassal.action = pPlot =>
            {
                pPlot.initiator_kingdom.AW()?.SetVassal(pPlot.target_kingdom.AW());
                return true;
            };
            active_vassal.check_supporters = (pActor, pPlot) =>
                pActor.isUnitOk() && !pActor.kingdom.isEnemy(pPlot.initiator_kingdom) &&
                !pActor.kingdom.isEnemy(pPlot.target_kingdom);

            #endregion

            #region absorbvassal

            PlotAsset absorbvassal = new PlotAsset();
            absorbvassal.id = "absorb_vassal";
            absorbvassal.path_icon = "ui/plots/plot_absorb_vassal";
            absorbvassal.translation_key = "plot_absorb_vassal";
            absorbvassal.description = "plot_description_absorb_vassal";
            absorbvassal.check_initiator_actor = true;
            absorbvassal.check_initiator_kingdom = true;
            absorbvassal.check_target_kingdom = true;
            absorbvassal.plot_power = ((Actor pActor) => (int)pActor.stats[S.diplomacy]); //改为比对附庸国军事实力

            AssetManager.plots_library.add(absorbvassal);

            absorbvassal.check_supporters = global::PlotsLibrary.checkMembersToRemoveDefault;
            absorbvassal.check_should_continue = (Plot pPlot) =>
            {
                // Ensure the target kingdom is a vassal of the initiator kingdom
                // and the initiator kingdom does not have any enemies
                // and the initiator kingdom's army size is not smaller than the target kingdom's
                return pPlot.initiator_kingdom.getArmy() >= pPlot.target_kingdom.getArmy()
                       && pPlot.target_kingdom.AW().IsVassal()
                       && !pPlot.initiator_kingdom.hasEnemies();
            };


            absorbvassal.check_launch = delegate(Actor pActor, Kingdom pKingdom)
            {
                // Ensure the target kingdom is a vassal of the initiator kingdom
                if (pKingdom.AW().IsVassal() || !pKingdom.AW().IsSuzerain() || pActor.kingdom.hasEnemies())
                {
                    return false;
                }


                // Check if the vassal relation has been established for at least 50 years

                return true;
            };


            absorbvassal.action = delegate(Plot pPlot)
            {
                AW_Kingdom lordKingdom = pPlot.initiator_kingdom as AW_Kingdom;
                AW_Kingdom vassalKingdom = pPlot.target_kingdom as AW_Kingdom;

                // Create a copy of the vassal kingdom's cities list
                List<City> vassalCities = new List<City>(vassalKingdom.cities);
                EventsManager.Instance.ENDVassal(vassalKingdom, lordKingdom, true);
                lordKingdom.vassals.Remove(vassalKingdom);
                vassalKingdom.ClearSuzerain();
                // Transfer all the cities of the vassal kingdom to the lord kingdom
                foreach (City vassalCity in vassalCities)
                {
                    vassalCity.joinAnotherKingdom(lordKingdom);
                }

                lordKingdom.absorb_timestamp = World.world.getCreationTime();
                return true;
            };

            #endregion

            #region IndependencePlot

            PlotAsset IndependenceWar = new()
            {
                // 设置 IndependenceWar 的各个属性
                id = "Independence_War",                           // 情节的唯一标识符
                path_icon = "ui/plots/plot_Independence_War",      // 情节在界面上的图标路径
                translation_key = "plot_Independence_War",         // 情节的翻译键值
                description = "plot_description_Independence_War", // 情节的描述
                check_initiator_actor = true,                      // 是否检查发起者的角色
                check_initiator_kingdom = true,                    // 是否检查发起者的王国
                check_target_kingdom = true,                       // 是否检查目标王国
                cost = 1200,
                // 设置情节强度的计算方式
                plot_power = (Actor pActor) => (int)((pActor.stats[S.warfare] + pActor.stats[S.intelligence]) * 0.5f),
            };

            // 将 IndependenceWar 添加到 AssetManager 的 plots_library 列表中
            AssetManager.plots_library.add(IndependenceWar);


            IndependenceWar.check_supporters = global::PlotsLibrary.checkMembersToRemoveDefault;

            IndependenceWar.check_launch = delegate(Actor pActor, Kingdom pKingdom)
            {
                AW_Kingdom lord = pKingdom.AW().suzerain;
                if (lord != null && (World.world.diplomacy.getOpinion(pKingdom, lord).total - 950) >= 0)
                {
                    return false;
                }

                if (pActor.kingdom.hasEnemies() || pKingdom.AW().IsSuzerain() || !pKingdom.AW().IsVassal())
                {
                    return false;
                }

                //Debug.Log("关系"+(World.world.diplomacy.getOpinion(pKingdom, lord).total - 1000));
                return true;
            };
            IndependenceWar.check_should_continue = (Plot pPlot) =>
            {
                if (pPlot.supporters.Count < 2)
                {
                    return false;
                }

                int army = 0;
                foreach (Actor actor in pPlot.supporters)
                {
                    Kingdom kingdom = actor.kingdom;
                    if (!kingdom.isAlive())
                    {
                        return false;
                    }

                    if (kingdom.hasEnemies())
                    {
                        return false;
                    }

                    army += kingdom.getArmy();
                }

                if (army < pPlot.target_kingdom.getArmy())
                {
                    return false;
                }


                return !pPlot.target_kingdom.AW().IsVassal()
                       && pPlot.target_kingdom.AW().IsSuzerain()
                       && !pPlot.initiator_kingdom.hasEnemies() && pPlot.initiator_kingdom.data.royal_clan_id !=
                       pPlot.target_kingdom.data.royal_clan_id;
            };
            // 定义情节发生时要执行的操作
            IndependenceWar.action = delegate(Plot pPlot)
            {
                AW_Kingdom vassal = pPlot.initiator_kingdom as AW_Kingdom;
                AW_Kingdom lord = pPlot.target_kingdom as AW_Kingdom;

                // 调用新的函数来移除附庸关系

                War war = World.world.diplomacy.startWar(vassal, lord,
                                                         AssetManager.war_types_library.get("independence_war"));
                vassal.RemoveSuzerain();

                foreach (Actor actor in pPlot.supporters)
                {
                    Kingdom kingdom = actor.kingdom;

                    if (kingdom.isAlive() && kingdom.countCities() != 0 && !kingdom.hasEnemies() &&
                        kingdom.king                               != null)
                    {
                        // if kingdom is a vassal of the same suzerain as the initiator kingdom
                        if (kingdom.AW().IsVassal() && kingdom.AW().suzerain == pPlot.target_kingdom.AW())
                        {
                            // make kingdom independent

                            kingdom.AW().RemoveSuzerain();
                            // join war
                            war.joinAttackers(kingdom);
                        }
                    }
                }


                return true;
            };

            #endregion

            #region 改变对天命国的战争类型

            var new_war = AssetManager.plots_library.get("new_war");
            new_war.action = delegate(Plot pPlot)
            {
                // 判断目标国家是否是天命国家
                AW_Kingdom target = pPlot.target_kingdom as AW_Kingdom;
                if (MoHTools.IsMoHKingdom(target))
                {
                    // 如果目标国家是天命国家，设置战争类型为 "tianming"
                    World.world.diplomacy.startWar(pPlot.initiator_kingdom, pPlot.target_kingdom,
                                                   AssetManager.war_types_library.get("tianming"), false);
                    WorldLogTools.logtianmingwar(pPlot.initiator_kingdom, pPlot.target_kingdom);
                }
                else
                {
                    // 否则使用普通战争类型
                    World.world.diplomacy.startWar(pPlot.initiator_kingdom, pPlot.target_kingdom,
                                                   global::WarTypeLibrary.normal);
                }

                return true;
            };
            new_war.check_launch = delegate(Actor pActor, Kingdom pKingdom)
            {
                // 直接允许Rebel发动行动，无视其他限制
                if (pKingdom.AW().Rebel)
                {
                    return true;
                }

                if (pKingdom.AW().IsVassal())
                {
                    return false;
                }

                if (!DiplomacyHelpers.isWarNeeded(pKingdom))
                {
                    return false;
                }

                if (pKingdom.hasAlliance())
                {
                    foreach (Kingdom item3 in pKingdom.getAlliance().kingdoms_hashset)
                    {
                        if (item3 != pKingdom && !(item3.king == null))
                        {
                            List<Plot> plotsFor = World.world.plots.getPlotsFor(item3.king, true);
                            if (plotsFor != null)
                            {
                                foreach (var plot in plotsFor)
                                {
                                    if (plot.isSameType(global::PlotsLibrary.new_war))
                                    {
                                        return false;
                                    }
                                }
                            }
                        }
                    }

                    return true;
                }

                return true;
            };

            new_war.check_should_continue = ((Plot pPlot) =>
            {
                // 如果是Rebel，直接继续行动，无视其他限制
                if (pPlot.initiator_kingdom.AW().Rebel &&
                    pPlot.initiator_kingdom.getAlliance() != pPlot.target_kingdom.getAlliance())
                {
                    return true;
                }

                return pPlot.initiator_actor.isUnitOk() &&
                       (!pPlot.initiator_kingdom.hasAlliance() ||
                        pPlot.initiator_kingdom.getAlliance() != pPlot.target_kingdom.getAlliance()) &&
                       DiplomacyHelpers.isWarNeeded(pPlot.initiator_kingdom);
            });

            #endregion

            #region 改变rebel

            var rebellion = AssetManager.plots_library.get("rebellion");
            rebellion.action = delegate(Plot pPlot)
            {
                if (MoHTools.IsMoHKingdom(pPlot.initiator_city.kingdom.AW()) ||
                    pPlot.initiator_city.kingdom.AW().Rebel || pPlot.initiator_city.kingdom.AW().FomerMoh)
                {
                    startTianmingRebellion(pPlot.initiator_city, pPlot.getLeader(), pPlot);
                }
                else
                {
                    DiplomacyHelpers.startRebellion(pPlot.initiator_city, pPlot.getLeader(), pPlot);
                }

                return true;
            };

            #endregion
        }

        private bool ActiveVassalCheckContinue(Plot pPlot)
        {
            return !pPlot.initiator_kingdom.isEnemy(pPlot.target_kingdom) &&
                   pPlot.initiator_kingdom.isOpinionTowardsKingdomGood(pPlot.target_kingdom);
        }

        public static void startTianmingRebellion(City pCity, Actor pActor, Plot pPlot)
        {
            Kingdom originalKingdom = pCity.kingdom;
            Kingdom newKingdom = pCity.makeOwnKingdom();

            // 设置新王国为义军
            newKingdom.AW().Rebel = true;

            pCity.removeLeader();
            pCity.addNewUnit(pActor);
            City.makeLeader(pActor, pCity);
            War war = null;

            using ListPool<War> listPool = World.world.wars.getWars(originalKingdom);
            foreach (ref War item in listPool)
            {
                War current = item;
                if (current.main_attacker == originalKingdom &&
                    current.getAsset()    == AssetManager.war_types_library.get("tianmingrebel"))
                {
                    war = current;
                    war.joinDefenders(newKingdom);
                    break;
                }
            }

            if (war == null)
            {
                war = World.world.diplomacy.startWar(originalKingdom, newKingdom,
                                                     AssetManager.war_types_library.get("tianmingrebel"));
                if (originalKingdom.hasAlliance())
                {
                    foreach (Kingdom ally in originalKingdom.getAlliance().kingdoms_hashset)
                    {
                        if (ally != originalKingdom && ally.isOpinionTowardsKingdomGood(originalKingdom))
                        {
                            war.joinAttackers(ally);
                        }
                    }
                }
            }

            foreach (Actor supporter in pPlot.supporters)
            {
                City supporterCity = supporter.city;
                if (supporterCity         != null && supporterCity.kingdom != newKingdom &&
                    supporterCity.kingdom == originalKingdom)
                {
                    supporterCity.joinAnotherKingdom(newKingdom);
                }
            }

            int totalCities = originalKingdom.cities.Count;
            int maxCitiesToJoin = newKingdom.getMaxCities() - newKingdom.cities.Count;
            maxCitiesToJoin = maxCitiesToJoin < 0
                ? 0
                : (maxCitiesToJoin > totalCities / 3 ? totalCities / 3 : maxCitiesToJoin);

            for (int i = 0; i < maxCitiesToJoin; i++)
            {
                if (!DiplomacyHelpers.checkMoreAlignedCities(newKingdom, originalKingdom))
                {
                    break;
                }
            }
        }


        private bool IsLeaderOfCity(Actor actor)
        {
            return actor.city.leader == actor;
        }

        private void PerformKingdomUsurpation(Actor usurper, Kingdom kingdom)
        {
            usurper.city.removeLeader();

            if (kingdom.capital != null && usurper.city != kingdom.capital)
            {
                usurper.ChangeCity(kingdom.capital);
            }

            if (kingdom.king.unit_group != null)
            {
                kingdom.king.unit_group.disband();
            }

            // CityTools.logUsurpation(usurper, kingdom);
            WorldLog.logKingLeft(kingdom, kingdom.king);

            kingdom.CallMethod("removeKing");
            kingdom.clearKingData();

            kingdom.trySetRoyalClan();

            kingdom.setKing(usurper);
            WorldLog.logNewKing(kingdom);
        }

        private void PerformCityUsurpation(Actor usurper)
        {
            usurper.city.removeLeader();
            City.makeLeader(usurper, usurper.city);
        }

        /// <summary>
        ///     尝试发动主动附庸
        /// </summary>
        /// <param name="pActor"></param>
        /// <returns></returns>
        public static bool tryPlotActiveVassal(ClanManager pClanManager, Actor pActor)
        {
            if (pClanManager.basePlotChecks(pActor, active_vassal)) return false;

            AW_Kingdom kingdom = pActor.kingdom.AW();

            var enemies = new HashSet<Kingdom>(kingdom.getEnemiesKingdoms());

            var war_plots = World.world.plots.list
                                 .Where(plot => plot.getAsset().AW()?.plot_type == PlotType.war &&
                                                plot.target_kingdom             == kingdom)
                                 .ToList();

            if (!enemies.Any() && war_plots.Count == 0) return false;

            var need_active_vassal =
                enemies.Any(enemy => !(enemy.power <= kingdom.power * OtherConst.ActiveVassalPowerRate));

            if (!need_active_vassal) return false;

            AW_Kingdom suzerain = null;
            float opinion = 0;
            foreach (Kingdom possible_suzerain in World.world.kingdoms.list_civs)
            {
                if (enemies.Contains(possible_suzerain)) continue;

                if (possible_suzerain.power <= kingdom.power * OtherConst.ActiveVassalPowerRate) continue;

                var possible_opinion = World.world.diplomacy.getOpinion(possible_suzerain, kingdom).total;
                if (suzerain == null || possible_opinion > opinion || Toolbox.randomChance(0.5f))
                {
                    suzerain = possible_suzerain.AW();
                    opinion = possible_opinion;
                }
            }

            if (suzerain == null) return false;

            Plot plot = World.world.plots.newPlot(pActor, active_vassal);
            plot.rememberInitiators(pActor);
            plot.target_kingdom = suzerain;

            return true;
        }
    }
}