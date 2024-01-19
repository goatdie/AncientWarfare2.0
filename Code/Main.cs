using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Figurebox.ai;
using Figurebox.constants;
using Figurebox.core;
using Figurebox.patch;
using Figurebox.patch.MoH;
using Figurebox.patch.policies;
using Figurebox.ui.patches;
using Figurebox.ui.windows;
using Figurebox.Utils;
using HarmonyLib;
using ModDeclaration;
using NCMS;
using NeoModLoader.api;
using NeoModLoader.api.attributes;
using NeoModLoader.General;
using NeoModLoader.services;
using NeoModLoader.utils;
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
        public static Transform prefabs_library;

        public List<string> addRaces = new List<string>()
        {
            "Xia"
        };

        public BuildingLibrary buildingLibrary = new BuildingLibrary();

        public MoreBuildings moreBuildings = new MoreBuildings();

        private int race_count = 4;

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
            NewUI.PatchResources();
            KingdomBehLibrary.init();
            _ = new ActorTaskLibrary();
            _ = new CityTaskLibrary();
            _ = new ai.ActorJobLibrary();
            _ = new ai.CitizenJobs();
            _ = new ai.CityJobLibrary();
            professions = new content.ProfessionLibrary();
            Traits.init();
            AW_WarManager.init();
            AW_CitiesManager.init();
            AW_KingdomManager.init();
            AW_AllianceManager.init();
            KingdomPolicyLibrary.Instance.init();
            KingdomPolicyStateLibrary.Instance.init();

            KingdomPolicyLibrary.Instance.post_init();
            KingdomPolicyStateLibrary.Instance.post_init();
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
            Tooltips.init();
            TraitGroups.init();
            TianmingGroup.init();
            RacesLibrary.init();
            moreActors.init();
            moreBuildings.init();
            KingdomVassals.init();

            moreKingdoms.init();
            BuildingLibrary.init();
            MapModeManager.CreateMapLayer();
            //NewUI.CreateAndPatchCharIcons();
            instance = this;
            EventsManager.Instance.CreateDataBase();
            print("Translation loaded");
            ResourceAsset resourceAsset = AssetManager.resources.get(SR.gold);
            resourceAsset.maximum = 99999999;
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

        private void Update()
        {
            if (instance == null) return;
            checkRaceAdded();
            // AW_Kingdom.kingsSetThisFrame.Clear();
        }

        private void OnApplicationQuit()
        {
          // EventsManager.Instance.CleanTempDataBase();
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

            prefabs_library = new GameObject("Prefabs").transform;
            prefabs_library.SetParent(transform);
            _ = new AWS();
            _ = new AWDataS();
            _ = new PolicyState();
            _ = new PolicyStateType();
            AutoMethodReplaceTool.ReplaceMethods();
        }

        [Hotfixable]
        public void Reload()
        {
            var locale_dir = GetLocaleFilesDirectory(GetDeclaration());
            foreach (var file in Directory.GetFiles(locale_dir))
            {
                if (file.EndsWith(".json"))
                {
                    LM.LoadLocale(Path.GetFileNameWithoutExtension(file), file);
                }
                else if (file.EndsWith(".csv"))
                {
                    LM.LoadLocales(file);
                }
            }

            LM.ApplyLocale();

            Dictionary<int, (Kingdom, Kingdom)> hash_dict = new();
            foreach (Kingdom kingdom1 in World.world.kingdoms.list)
            {
                foreach (Kingdom kingdom2 in World.world.kingdoms.list)
                {
                    int hash = Kingdom.cache_enemy_check.getHash(kingdom1, kingdom2);
                    if (hash_dict.TryGetValue(hash, out (Kingdom, Kingdom) collision))
                    {
                        (Kingdom collision_k1, Kingdom collision_k2) = collision;
                        if ((collision_k1.id != kingdom1.id || collision_k2.id != kingdom2.id) &&
                            (collision_k1.id != kingdom2.id || collision_k2.id != kingdom1.id))
                            LogInfo(
                                $"Hash collision: ({kingdom1.id} {kingdom2.id}) with ({collision_k1.id} {collision_k2.id})");
                    }
                    else
                    {
                        hash_dict.Add(hash, (kingdom1, kingdom2));
                    }
                }
            }
        }

        /// <summary>
        ///     检查是否有新的种族，为新的种族补充贴图之类的
        /// </summary>
        [Hotfixable]
        private void checkRaceAdded()
        {
            if (AssetManager.raceLibrary.list.Count == race_count) return;

            race_count = AssetManager.raceLibrary.list.Count;

            foreach (var added_prof in professions.added_assets)
            {
                if (string.IsNullOrEmpty(added_prof.special_skin_path)) continue;

                var xia_prof_sprites_path = "actors/" + AssetManager.raceLibrary.get("Xia").main_texture_path +
                                            added_prof.special_skin_path;
                var all_sprites = SpriteTextureLoader.getSpriteList(xia_prof_sprites_path);

                foreach (var race in AssetManager.raceLibrary.list)
                {
                    if (!race.civilization) continue;
                    var sprites_path = "actors/" + race.main_texture_path + added_prof.special_skin_path;
                    var sprites = Resources.LoadAll<Sprite>(sprites_path);
                    if (sprites is { Length: > 0 }) continue;

                    foreach (var sprite in all_sprites)
                        ResourcesPatch.PatchResource(sprites_path + "/" + sprite.name, sprite);
                }
            }
        }

        private void Configure()
        {
            if (Environment.UserName == "Inmny" || Environment.UserName == "1")
            {
                Config.isEditor = true;
                DebugConst.IS_DEVELOPER = true;
                if (Environment.UserName == "Inmny") DebugConst.EDITOR_INMNY = true;

                if (DebugConst.EDITOR_INMNY)
                {
                    DebugConfig.setOption(DebugOption.DrawCitizenJobIcons, true);
                    DebugConfig.setOption(DebugOption.CitizenJobAttacker, true);
                }
            }
        }

        void patchHarmony()
        {
            Harmony.CreateAndPatchAll(typeof(RacesLibrary));
            Harmony.CreateAndPatchAll(typeof(MoHCorePatch));
            Harmony.CreateAndPatchAll(typeof(NamePatch));
            Harmony.CreateAndPatchAll(typeof(KingdomHeirPatch));
            Harmony.CreateAndPatchAll(typeof(KingdomYearName));
            Harmony.CreateAndPatchAll(typeof(KingdomWindowPatch));
            Harmony.CreateAndPatchAll(typeof(WorldLogPatch));
            Harmony.CreateAndPatchAll(typeof(SpecialFigure));
            _ = new SlavesPatch();
            _ = new CitiesManagerPatch();
            _ = new KingdomManagerPatch();
            /*
                        Harmony.CreateAndPatchAll(typeof(FunctionHelper));

                        Harmony.CreateAndPatchAll(typeof(TianmingGroup));
                        Harmony.CreateAndPatchAll(typeof(ClansManager));
                        Harmony.CreateAndPatchAll(typeof(KingdomVassals));

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
            */
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
        public content.ProfessionLibrary professions;

        #endregion
    }
}