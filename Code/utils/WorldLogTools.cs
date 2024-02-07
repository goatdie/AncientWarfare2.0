namespace Figurebox.utils;

public class WorldLogTools
{
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

        public static void LogKingIntegration(Actor King, Kingdom Main, Kingdom Inherited)
        {
            WorldLogMessage warreclaim =
                new WorldLogMessage("KingIntegrationMessage", King.getName(), Main.name, Inherited.name);
            //ColorAsset colorAsset = Reflection.GetField(typeof(Kingdom), pKingdom, "colorAsset") as ColorAsset;
            warreclaim.color_special2 = King.kingdom.kingdomColor.getColorText();
            warreclaim.color_special1 = Main.kingdomColor.getColorText();
            warreclaim.color_special3 = Inherited.kingdomColor.getColorText();
            //mandateofheaven.color_special1 = new Color32(255, 217, 134, 255);
            //mandateofheaven.unit = pAttacker.king;
            warreclaim.location = King.currentPosition;
            warreclaim.kingdom = Main;
            warreclaim.add();
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
}