using AncientWarfare.Abstracts;
using AncientWarfare.Attributes;
using AncientWarfare.Core.Content;
using AncientWarfare.LocaleKeys;
using NeoModLoader.General;
using NeoModLoader.General.UI.Tab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AncientWarfare.UI
{
    [ManagerInitializeAfter(typeof(GodPowerExtendLibrary))]
    internal class Manager : IManager
    {
        public void Initialize()
        {
            create_tab();
        }

        private void create_tab()
        {
            var tab = TabManager.CreateTab("Ancient Warfare", CommonKeys.ancient_warfare, UIKeys.ancient_warfare_tab_desc, SpriteTextureLoader.getSprite("ui/icons/iconAncientWarfare"));

            const string INFO = "info";
            const string MAP = "map";

            tab.SetLayout(new List<string> { INFO, MAP });
            tab.AddPowerButton(MAP, PowerButtonCreator.CreateToggleButton(nameof(GodPowerExtendLibrary.tribe_zones), SpriteTextureLoader.getSprite("ui/icons/iconCities")));

            tab.UpdateLayout();
        }
    }
}
