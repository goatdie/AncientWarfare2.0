using System.Collections.Generic;
using NCMS.Utils;
using NeoModLoader.General.UI.Tab;
using UnityEngine;
namespace Figurebox
{
    internal class TabManager
    {
        private const string INFO = "info";
        private const string TOOL = "tool";
        private const string CREATURE = "creature";
        private const string OPTIONS = "options";
        private static PowersTab tab;
        public static void init()
        {
            tab = NeoModLoader.General.UI.Tab.TabManager.CreateTab("AW2Mod", "AW2", "AW2 Description", Sprites.LoadSprite($"{Mod.Info.Path}/icon.png"));
            tab.SetLayout(new List<string>
            {
                INFO,
                CREATURE,
                TOOL,
                OPTIONS
            });
            loadButtons();
            tab.UpdateLayout();
        }

        private static void loadButtons()
        {
            tab.AddPowerButton(TOOL,
                PowerButtons.CreateButton(
                    "vassal",
                    Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.icon_binfa.png"),
                    "make vassal",
                    "set vassal for kingdom",
                    Vector2.zero,
                    ButtonType.GodPower
                ));
            tab.AddPowerButton(TOOL,
                PowerButtons.CreateButton(
                    "vassal_remove",
                    Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.war_independent.png"),
                    "remove vassal",
                    "remove vassal for kingdom",
                    Vector2.zero,
                    ButtonType.GodPower
                ));
            /* PowerButtons.CreateButton(
                 "citizen_dej",
                 Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.civilian_icon2.png"),
                 "Citizen",
                 "Change Unit's Job To Citizen",
                 new Vector2(xPos + (index*gap), yPos),
                 ButtonType.GodPower,
                 collectionTab.transform,
                 null
             );
             index++;

             PowerButtons.CreateButton(
                 "king_dej",
                 Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.king_icon2.png"),
                 "King",
                 "Change Unit's Job To King",
                 new Vector2(xPos + (index*gap), yPos),
                 ButtonType.GodPower,
                 collectionTab.transform,
                 null
             );
             index++;

             PowerButtons.CreateButton(
                 "city_convert_dej",
                 Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.icon_ConvertCity.png"),
                 "Convert City",
                 "Change A City's Fealty To Another Kingdom",
                 new Vector2(xPos + (index*gap), yPos),
                 ButtonType.GodPower,
                 collectionTab.transform,
                 null
             );
             index++;

             PowerButtons.CreateButton(
                 "leader_board_dej",
                 Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.iconLeaderBoardCivs2.png"),
                 "Leader Board",
                 "View The Significant Units Within Your World",
                 new Vector2(xPos + (index*gap), yPos),
                 ButtonType.Click,
                 collectionTab.transform,
                 LeaderBoardWindow.openWindow
             );
             index += 2;

             PowerButtons.CreateButton(
                 "level_up_dej",
                 Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.iconLevels.png"),
                 "Level Up",
                 "Increases Unit's Level By Level Rate",
                 new Vector2(xPos + (index*gap), yPos),
                 ButtonType.GodPower,
                 collectionTab.transform,
                 null
             );
             createLevelInput(collectionTab.transform, new Vector3(xPos + 90 + ((index-2)*gap), 0, 0), Main.savedSettings.inputOptions["LevelRate"].value);
             index++;

             PowerButtons.CreateButton(
                 "specific_level_up_dej",
                 Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.iconLevels.png"),
                 "Specific Level Up",
                 "Changes Unit's Level To Level Rate And Not Add Upon It",
                 new Vector2(xPos + (index*gap), yPos),
                 ButtonType.GodPower,
                 collectionTab.transform,
                 null
             );
             index += 2;

             PowerButtons.CreateButton(
                 "heroic_titles_dej",
                 Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.icon_HeroicTitle.png"),
                 "Heroic Titles",
                 "Modify And Add Titles To Give To Units!",
                 new Vector2(xPos + (index*gap), yPos),
                 ButtonType.Click,
                 collectionTab.transform,
                 TitlesWindow.openWindow
             );
             index++;

             PowerButtons.CreateButton(
                 "alliance_dej",
                 Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.icon_AllianceWhisper.png"),
                 "Whisper Of Alliance",
                 "Create An Alliance Between Two Kingdoms",
                 new Vector2(xPos + (index*gap), yPos),
                 ButtonType.GodPower,
                 collectionTab.transform,
                 null
             );
             index++;

             PowerButtons.CreateButton(
                 "edit_cultures_dej",
                 Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.culture_background.png"),
                 "Edit Cultures",
                 "Add Or Remove Tech From Cultures",
                 new Vector2(xPos + (index*gap), yPos),
                 ButtonType.Click,
                 collectionTab.transform,
                 EditCultureWindow.openWindow
             );
             index = 0;
             yPos -= 36;

             PowerButtons.CreateButton(
                 "debug_dej",
                 Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.iconDebug.png"),
                 "Debug",
                 "Show The Debug Window",
                 new Vector2(xPos + (index*gap), yPos),
                 ButtonType.Click,
                 collectionTab.transform,
                 () => Windows.ShowWindow("debug")
             );
             index++;

             PowerButtons.CreateButton(
                 "holy_history_dej",
                 Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.iconWorldLaws.png"),
                 "Holy History Book (WIP)",
                 "View The History Of Your Favorite Culture",
                 new Vector2(xPos + (index*gap), yPos),
                 ButtonType.Click,
                 collectionTab.transform,
                 null
             );
             index++;

             PowerButtons.CreateButton(
                 "expand_zone_dej",
                 Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.expand_zone_icon.png"),
                 "Expansion Beam",
                 "Expand Zones Of A City",
                 new Vector2(xPos + (index*gap), yPos),
                 ButtonType.GodPower,
                 collectionTab.transform,
                 null
             );
             index++;

             PowerButtons.CreateButton(
                 "remove_zone_dej",
                 Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.remove_zone_icon.png"),
                 "Removal Beam",
                 "Remove Zones Of A City",
                 new Vector2(xPos + (index*gap), yPos),
                 ButtonType.GodPower,
                 collectionTab.transform,
                 null
             );
             index++;

             PowerButtons.CreateButton(
                 "stat_limiter_dej",
                 Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.icon_imperial_thinking.png"),
                 "Unit Stat Limiter",
                 "Set Up Your Own Limits To Stats For Units",
                 new Vector2(xPos + (index*gap), yPos),
                 ButtonType.Click,
                 collectionTab.transform,
                 StatLimiterWindow.openWindow
             );
             index+=5;

             PowerButtons.CreateButton(
                 "imperial_thinking_dej",
                 Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.icon_imperial_thinking.png"),
                 "Imperial Thinking",
                 "Village Of Different Races Can Be Taken Over Instead Of Killed",
                 new Vector2(xPos + (index*gap), yPos),
                 ButtonType.Toggle,
                 collectionTab.transform,
                 NewActions.turnOnImperialThinking
             );
             index++;

             PowerButtons.CreateButton(
                 "buildings_select_dej",
                 Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.icon_imperial_thinking.png"),
                 "Select Building",
                 "Select A Building To Be Dropped",
                 new Vector2(xPos + (index*gap), yPos),
                 ButtonType.Click,
                 collectionTab.transform,
                 BuildingsWindow.openWindow
             );
             index++;

             PowerButtons.CreateButton(
                 "building_drop_dej",
                 Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.icon_imperial_thinking.png"),
                 "Drop Building",
                 "Drop Selected Building",
                 new Vector2(xPos + (index*gap), yPos),
                 ButtonType.GodPower,
                 collectionTab.transform,
                 null
             );
             index++;*/
            tab.AddPowerButton(CREATURE,
                PowerButtons.CreateButton(
                    "spawn_xia",
                    Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.iconXias.png"),
                    "Xia",
                    "From East",
                    Vector2.zero,
                    ButtonType.GodPower
                ));

            tab.AddPowerButton(INFO,
                PowerButtons.CreateButton(
                    "TianMing_board",
                    Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.iconRebel.png"),
                    "Mandate of Heaven Board",
                    "View The Significant Units Within Your World",
                    Vector2.zero,
                    ButtonType.Click,
                    null,
                    TianmingBoardWindow.openWindow
                ));
            tab.AddPowerButton(INFO,
                PowerButtons.CreateButton(
                    "Kingdom_History",
                    Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.iconkingdomhistory.png"),
                    "Kingdom History",
                    "Window for all kingdoms' history",
                    Vector2.zero,
                    ButtonType.Click,
                    null,
                    KingdomHistoryListWindow.openWindow
                ));
            tab.AddPowerButton(OPTIONS,
                PowerButtons.CreateButton(
                    "historical_figure",
                    Sprites.LoadSprite($"{Mod.Info.Path}/icon.png"),
                    "Historical Figure",
                    "Disable Historical Figure(Enable this button to disable)",
                    Vector2.zero,
                    ButtonType.Toggle,
                    null,
                    SpecialFigure.togglefigure
                ));
        }
    }
}