using System.Linq;

namespace Figurebox.Utils
{
    public static class CityTools
    {
        /// <summary>
        ///     从城市中移出单位
        /// </summary>
        /// <param name="city">城市</param>
        /// <param name="pActor">单位</param>
        /// <param name="pUnsetKingdom">是否从其王国中移出</param>
        public static void removeUnit(this City city, Actor pActor, bool pUnsetKingdom = true)
        {
            // 如果单位是船，从船只集合中移除
            city._dirty_units = true;
            if (pActor.asset.isBoat)
            {
                city.boats.Remove(pActor);
            }
            else
            {
                // 否则，从普通单位集合中移除
                city.units.Remove(pActor);
                if (pActor == city.leader) city.removeLeader();
            }

            // 更新状态
            city.setStatusDirty();

            // 如果指定，从其王国中移除单位
            if (pUnsetKingdom) pActor.setKingdom(null);

            // 这里可以添加任何其他需要在移除单位时进行的操作
        }

        public static void logUnite(Kingdom pKingdom)
        {
            WorldLogMessage mandateofheaven =
                new WorldLogMessage("mandateofheavenMessage", pKingdom.name, pKingdom.king.getName(), null);
            //ColorAsset colorAsset = Reflection.GetField(typeof(Kingdom), pKingdom, "colorAsset") as ColorAsset;
            mandateofheaven.color_special1 = pKingdom.kingdomColor.getColorText();
            //mandateofheaven.color_special1 = new Color32(255, 217, 134, 255);
            mandateofheaven.unit = pKingdom.king;
            mandateofheaven.location = pKingdom.king.currentPosition;
            mandateofheaven.kingdom = pKingdom;
            mandateofheaven.add();
        }

        public static void loglose(Kingdom pKingdom)
        {
            WorldLogMessage mandateofheaven = new WorldLogMessage("losemandateofheavenMessage", pKingdom.name,
                pKingdom.king.getName(), null);
            //ColorAsset colorAsset = Reflection.GetField(typeof(Kingdom), pKingdom, "colorAsset") as ColorAsset;
            mandateofheaven.color_special1 = pKingdom.kingdomColor.getColorText();
            //mandateofheaven.color_special1 = new Color32(255, 217, 134, 255);
            if (pKingdom.king != null)
            {
                mandateofheaven.unit = pKingdom.king;
            }

            mandateofheaven.location = pKingdom.king.currentPosition;
            mandateofheaven.kingdom = pKingdom;
            mandateofheaven.add();
        }

        public static void loglosekingdom(Kingdom pKingdom)
        {
            WorldLogMessage mandateofheavenlose =
                new WorldLogMessage("losekingdommandateofheavenMessage", pKingdom.name, null);
            //ColorAsset colorAsset = Reflection.GetField(typeof(Kingdom), pKingdom, "colorAsset") as ColorAsset;
            mandateofheavenlose.color_special1 = pKingdom.kingdomColor.getColorText();
            //mandateofheaven.color_special1 = new Color32(255, 217, 134, 255);
            if (pKingdom.king != null)
            {
                mandateofheavenlose.unit = pKingdom.king;
            }

            mandateofheavenlose.location = pKingdom.location;
            mandateofheavenlose.kingdom = pKingdom;
            mandateofheavenlose.add();
        }

        public static void logtianmingwar(Kingdom pAttacker, Kingdom pDefender)
        {
            WorldLogMessage warmandateofheaven =
                new WorldLogMessage("warmandateofheavenMessage", pAttacker.name, pDefender.name, null);
            //ColorAsset colorAsset = Reflection.GetField(typeof(Kingdom), pKingdom, "colorAsset") as ColorAsset;
            warmandateofheaven.color_special2 = pAttacker.kingdomColor.getColorText();
            warmandateofheaven.color_special1 = pDefender.kingdomColor.getColorText();
            //mandateofheaven.color_special1 = new Color32(255, 217, 134, 255);
            //mandateofheaven.unit = pAttacker.king;
            warmandateofheaven.location = pAttacker.location;
            warmandateofheaven.kingdom = pAttacker;
            warmandateofheaven.add();
        }

        public static void logjoinanotherkingdom(Kingdom pMain, Kingdom pTarget)
        {
            WorldLogMessage joinAnotherKingdom =
                new WorldLogMessage("joinanotherkingdomMessage", pMain.name, pTarget.name, null);
            //ColorAsset colorAsset = Reflection.GetField(typeof(Kingdom), pKingdom, "colorAsset") as ColorAsset;
            joinAnotherKingdom.color_special2 = pMain.kingdomColor.getColorText();
            joinAnotherKingdom.color_special1 = pTarget.kingdomColor.getColorText();
            //mandateofheaven.color_special1 = new Color32(255, 217, 134, 255);
            //mandateofheaven.unit = pAttacker.king;
            joinAnotherKingdom.location = pMain.location;
            joinAnotherKingdom.kingdom = pMain;
            joinAnotherKingdom.add();
        }

        public static void logreclaim(Kingdom pAttacker, Kingdom pDefender, Kingdom Winner)
        {
            WorldLogMessage warreclaim =
                new WorldLogMessage("reclaimwarendMessage", pAttacker.name, pDefender.name, Winner.name);
            //ColorAsset colorAsset = Reflection.GetField(typeof(Kingdom), pKingdom, "colorAsset") as ColorAsset;
            warreclaim.color_special2 = pAttacker.kingdomColor.getColorText();
            warreclaim.color_special1 = pDefender.kingdomColor.getColorText();
            warreclaim.color_special3 = Winner.kingdomColor.getColorText();
            //mandateofheaven.color_special1 = new Color32(255, 217, 134, 255);
            //mandateofheaven.unit = pAttacker.king;
            warreclaim.location = Winner.location;
            warreclaim.kingdom = Winner;
            warreclaim.add();
        }

        public static void logUsurpation(Actor pActor, Kingdom pTarget)
        {
            if (pActor == null)
            {
                return;
            }

            WorldLogMessage usurpation = new WorldLogMessage("usurpationMessage", pActor.getName(), pTarget.name, null);
            usurpation.color_special1 = pTarget.kingdomColor.getColorText();
            usurpation.color_special2 = pTarget.kingdomColor.getColorText();
            usurpation.location = pActor.currentPosition;
            usurpation.kingdom = pTarget;
            if (pActor != null)
            {
                usurpation.unit = pActor;
            }

            usurpation.add();
        }

        public static void LogVassalWarStart(Kingdom pAttacker, Kingdom pDefender)
        {
            WorldLogMessage vassalWarStart =
                new WorldLogMessage("vassalWarStartMessage", pAttacker.name, pDefender.name, null);
            vassalWarStart.color_special2 = pAttacker.kingdomColor.getColorText();
            vassalWarStart.color_special1 = pDefender.kingdomColor.getColorText();
            vassalWarStart.location = pAttacker.location;
            vassalWarStart.kingdom = pAttacker;
            vassalWarStart.add();
        }

        // 在结束附庸战争时添加记录
        public static void LogVassalWarEnd(Kingdom pAttacker, Kingdom pDefender)
        {
            WorldLogMessage vassalWarEnd =
                new WorldLogMessage("vassalWarEndMessage", pAttacker.name, pDefender.name, null);
            vassalWarEnd.color_special2 = pAttacker.kingdomColor.getColorText();
            vassalWarEnd.color_special1 = pDefender.kingdomColor.getColorText();
            vassalWarEnd.location = pAttacker.location;
            vassalWarEnd.kingdom = pAttacker;
            vassalWarEnd.add();
        }

        public static void LogIndependenceWarMessage(Kingdom pAttacker, Kingdom pDefender)
        {
            WorldLogMessage IndependenceWarEndMessage =
                new WorldLogMessage("IndependenceWarMessage", pAttacker.name, pDefender.name, null);
            IndependenceWarEndMessage.color_special2 = pAttacker.kingdomColor.getColorText();
            IndependenceWarEndMessage.color_special1 = pDefender.kingdomColor.getColorText();
            IndependenceWarEndMessage.location = pAttacker.location;
            IndependenceWarEndMessage.kingdom = pAttacker;
            IndependenceWarEndMessage.add();
        }

        public static void logFigure(Actor pActor)
        {
            if (pActor == null)
            {
                return;
            }

            WorldLogMessage historicalfigure = new WorldLogMessage("historicalMessage", pActor.getName(), null);
            historicalfigure.color_special1 = pActor.kingdom.kingdomColor.getColorText();
            historicalfigure.location = pActor.currentPosition;
            if (pActor != null)
            {
                historicalfigure.unit = pActor;
                //historicalfigure.special1 = pActor.getName();
            }

            historicalfigure.add();
        }

        public static bool WasOccupiedByKingdomInLast5To100Years(City city, Kingdom kingdom)
        {
            string cityId = city.data.id;
            string kingdomId = kingdom.data.id;
            int currentYear = World.world.mapStats.getCurrentYear();
            string currentKingdomId = city.kingdom.data.id;
            int startYear = currentYear - 100;


            if (FunctionHelper.CityYearData.ContainsKey(cityId))
            {
                var entries = FunctionHelper.CityYearData[cityId].OrderByDescending(e => e.Value.Item2).ToList();
                //Debug.Log($"Entries for city {city.data.name}:");
                foreach (var entry in entries)
                {
                    //Debug.Log($"Kingdom: {entry.Key.Split('-')[0]}, Year: {entry.Value.Item2}");
                }

                for (int i = 1; i < entries.Count; i++)
                {
                    // 如果在过去100年内，有其他王国占领了这个城市
                    if (entries[i - 1].Value.Item2 >= startYear && entries[i - 1].Key.Split('-')[0] != kingdomId)
                    {
                        // 如果在这之前，我们关心的王国是这个城市的统治者，并且这个城市在被其他王国占领之前已经在我们关心的王国的统治下度过了至少400年
                        if (entries[i].Key.Split('-')[0] == kingdomId &&
                            entries[i - 1].Value.Item2 - entries[i].Value.Item2 >= 60)
                        {
                            // Debug.Log($"Kingdom {kingdom.data.name} can reclaim city {city.data.name}.");
                            return true;
                        }
                    }
                }

                foreach (var entry in FunctionHelper.CityYearData[cityId])
                {
                    if (entry.Key.StartsWith(kingdomId + "-"))
                    {
                        int yearsSinceCapture = currentYear - entry.Value.Item2;
                        if (yearsSinceCapture >= 5 && yearsSinceCapture <= 100)
                        {
                            // 如果当前城市不属于该王国，且在过去的5到100年内被该王国占领过
                            if (currentKingdomId != kingdomId)
                            {
                                //Debug.Log($"100 5 Kingdom {kingdom.data.name} can reclaim city {city.data.name}.");

                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }
    }
}