using System;
using NeoModLoader.api;
using NeoModLoader.api.attributes;
using NCMS;
using NCMS.Utils;
using NeoModLoader.General;
using NeoModLoader.services;
using UnityEngine;
using ReflectionUtility;
using HarmonyLib;
using Figurebox;
using Figurebox.core;
using Figurebox.UI.Patches;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Config;
using System.Reflection;
using UnityEngine.Tilemaps;
using System.IO;
using Figurebox.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
#if 一米_中文名
using Chinese_Name;
#endif

namespace Figurebox
{
    [ModEntry]
    internal class Main : MonoBehaviour, ILocalizable
    {
        public static GameObject backgroundAvatar;
        public static GameObject citybg;

        public List<string> addRaces = new List<string>()
        {
            "Xia"
        };

        public BuildingLibrary buildingLibrary = new BuildingLibrary();

        public MoreBuildings moreBuildings = new MoreBuildings();

        // public const string mainPath = "worldbox_Data/StreamingAssets/Mods/NCMS/Core/Temp/Mods/AncientWarfare2.0";
        public static string mainPath => Mod.Info.Path; // 这种方式更鲁棒, 可以适配不同的模组文件夹位置

        void Awake()
        {
            LM.AddToCurrentLocale("", "");
#if 一米_中文名
            print("词库加载!" + mainPath + "/name_generators/Xia");
            CN_NameGeneratorLibrary.SubmitDirectoryToLoad(mainPath + "/name_generators/Xia");
            WordLibraryManager.SubmitDirectoryToLoad(mainPath + "/name_generators/lib");
            NameGeneratorInitialzier.init();
            LM.AddToCurrentLocale("familyname", "姓");
            LM.AddToCurrentLocale("clanname", "氏");
#endif
            LM.ApplyLocale(); //之前是只加载了, 忘记应用了
            Traits.init();
            AW_KingdomManager.init();
            //loadBanners("Xia");
            // NewUI.init();
            NameGeneratorAssets.init();
            warTypeLibrary.init();
            LoyaltyLibrary.init();
            NewGodPowers.init();
            TabManager.init();
            MorePlots.init();
            StatusEffectLib.init();
            WindowManager.init();
            moreWeapons.init();
            FunctionHelper.instance = functionHelper;
            trait_group.init();
            TianmingGroup.init();
            RacesLibrary.init();
            moreActors.init();
            moreBuildings.init();
            KingdomVassals.init();

            moreKingdoms.init();
            buildingLibrary.init();

            instance = this;
            print("Translation loaded");
        }


        void Start()
        {
            Dictionary<string, ScrollWindow> allWindows = ScrollWindow.allWindows;
            ScrollWindow.checkWindowExist("inspect_unit");
            allWindows["inspect_unit"].gameObject.SetActive(false);
            ScrollWindow.checkWindowExist("village");
            allWindows["village"].gameObject.SetActive(false);
            ScrollWindow.checkWindowExist("kingdom");
            allWindows["kingdom"].gameObject.SetActive(false);
            backgroundAvatar =
                GameObject.Find(
                    $"Canvas Container Main/Canvas - Windows/windows/inspect_unit/Background/Scroll View/Viewport/Content/Part 1/BackgroundAvatar");
            CityHistoryWindow.init();
            KingdomHistoryWindow.init();
            KingdomPolicyWindow.init();
            NameGeneratorAssets.init();
            BannerGenerator.loadTexturesFromResources("Xia");

            patchHarmony();
        }

        public string GetLocaleFilesDirectory(ModDeclare pModDeclare)
        {
            return Path.Combine(pModDeclare.FolderPath, "Locales");
        }

        void patchHarmony()
        {
            Harmony.CreateAndPatchAll(typeof(Traits));
            Harmony.CreateAndPatchAll(typeof(FunctionHelper));
            Harmony.CreateAndPatchAll(typeof(SpecialFigure));
            Harmony.CreateAndPatchAll(typeof(TianmingGroup));
            Harmony.CreateAndPatchAll(typeof(RacesLibrary));
            Harmony.CreateAndPatchAll(typeof(ClansManager));
            Harmony.CreateAndPatchAll(typeof(KingdomVassals));
            Harmony.CreateAndPatchAll(typeof(KingdomWindowPatch));
            print("Create and patch all:CTraits");
        }

        #region

        public static Main instance;
        public FunctionHelper functionHelper = new FunctionHelper();
        public MoreItems moreWeapons = new MoreItems();
        public MoreActors moreActors = new MoreActors();
        public RacesLibrary RacesLibrary = new RacesLibrary();
        public MorePlots MorePlots = new MorePlots();
        public MoreKingdoms moreKingdoms = new MoreKingdoms();
        public Traits Traits = new Traits();

        #endregion
    }
}