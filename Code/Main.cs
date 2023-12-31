using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Figurebox.constants;
using Figurebox.core;
using Figurebox.patch;
using Figurebox.Save;
using Figurebox.test;
using Figurebox.UI.Patches;
using HarmonyLib;
using ModDeclaration;
using NCMS;
using NeoModLoader.api;
using NeoModLoader.api.attributes;
using NeoModLoader.General;
using NeoModLoader.services;
using UnityEngine;
#if 一米_中文名
using Chinese_Name;
#endif

namespace Figurebox
{
    [ModEntry]
    internal class Main : MonoBehaviour, ILocalizable, IMod, IReloadable
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

        public static ModDeclare mod_declare { get; private set; }
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
            KingdomPolicyLibrary.Instance.init();
            KingdomPolicyStateLibrary.Instance.init();
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
        public ModDeclare GetDeclaration()
        {
            return mod_declare;
        }
        public GameObject GetGameObject()
        {
            return gameObject;
        }
        public string GetUrl()
        {
            return mod_declare.RepoUrl;
        }
        public void OnLoad(ModDeclare pModDecl, GameObject pGameObject)
        {
            mod_declare = pModDecl;

            Mod.Info = typeof(Info)
                .GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[]
                    {
                        typeof(NCMod)
                    },
                    null)
                ?.Invoke(new object[]
                {
                    new NCMod
                    {
                        name = pModDecl.Name,
                        author = pModDecl.Author,
                        description = pModDecl.Description,
                        path = pModDecl.FolderPath,
                        version = pModDecl.Version,
                        iconPath = pModDecl.IconPath,
                        targetGameBuild = pModDecl.TargetGameBuild
                    }
                }) as Info;

            Configure();
        }
        [Hotfixable]
        public void Reload()
        {
            foreach (City city in World.world.cities.list)
            {
                int count = 0;
                foreach (Actor actor in city.units)
                {
                    if (actor == null)
                    {
                        count++;
                    }
                }
                if (count == 0) continue;
                LogWarning($"There are {count} actors be null in {city.data.id}({city.getCityName()})'s units. Fix it temporarily.");
                city.units.Remove(null);
                city.units.checkAddRemove();
            }
        }
        private void Configure()
        {
            if (Environment.UserName == "Inmny" || Environment.UserName == "1")
            {
                Config.isEditor = true;
            }
        }

        void patchHarmony()
        {
            Harmony.CreateAndPatchAll(typeof(RacesLibrary));

            Harmony.CreateAndPatchAll(typeof(FunctionHelper));
            Harmony.CreateAndPatchAll(typeof(SpecialFigure));
            Harmony.CreateAndPatchAll(typeof(TianmingGroup));
            Harmony.CreateAndPatchAll(typeof(ClansManager));
            Harmony.CreateAndPatchAll(typeof(KingdomVassals));

            Harmony.CreateAndPatchAll(typeof(KingdomWindowPatch));
            Harmony.CreateAndPatchAll(typeof(CityWindowPatch));

            Harmony.CreateAndPatchAll(typeof(KingdomPatch));
            Harmony.CreateAndPatchAll(typeof(KingActorPatch));
            Harmony.CreateAndPatchAll(typeof(WarPatch));

            Harmony.CreateAndPatchAll(typeof(CustomSaveManager));

            if (DebugConst.ACTOR_TEST)
            {
                Harmony.CreateAndPatchAll(typeof(ActorTest));
                BatchTest<Actor>.SelfPatch();
            }
            if (DebugConst.CITY_TEST) Harmony.CreateAndPatchAll(typeof(CityTest));

            print("Create and patch all:CTraits");
        }

        public static void LogWarning(string pMessage, bool pShowStackTrace = false)
        {
            LogService.LogWarning($"[AW2.0]: {pMessage}");
            if (pShowStackTrace)
            {
                LogService.LogStackTraceAsWarning();
            }
        }
        public static void LogInfo(string pMessage, bool pShowStackTrace = false)
        {
            LogService.LogInfo($"[AW2.0]: {pMessage}");
            if (pShowStackTrace)
            {
                LogService.LogStackTraceAsInfo();
            }
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