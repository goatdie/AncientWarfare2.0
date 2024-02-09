using System;
using System.Collections.Generic;
using System.IO;
using Figurebox.ai;
using Figurebox.constants;
using Figurebox.content;
using Figurebox.core;
using Figurebox.patch;
using Figurebox.patch.MoH;
using Figurebox.patch.policies;
using Figurebox.ui.patches;
using Figurebox.utils;
using Figurebox.utils.instpredictors;
using HarmonyLib;
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
        public static Transform  prefabs_library;

        public static Main instance;

        private static readonly HashSet<string>         _logged_warnings = new();
        private static readonly HashSet<string>         _logged_infos    = new();
        public                  content.BuildingLibrary buildingLibrary;

        public content.ProfessionLibrary professions;

        private int race_count = 4;

        private int skip_frame = 60;

        // public const string mainPath = "worldbox_Data/StreamingAssets/Mods/NCMS/Core/Temp/Mods/AncientWarfare2.0";
        public static string mainPath => Mod.Info.Path; // 这种方式更鲁棒, 可以适配不同的模组文件夹位置

        public static ModDeclare mod_declare { get; private set; }

        void Awake()
        {
            LM.AddToCurrentLocale("", "");
#if 一米_中文名
            print("词库加载!"                                          + mainPath + "/name_generators/Xia");
            CN_NameGeneratorLibrary.SubmitDirectoryToLoad(mainPath + "/name_generators/Xia");
            WordLibraryManager.SubmitDirectoryToLoad(mainPath      + "/name_generators/lib");
            NameGeneratorInitialzier.init();
            LM.AddToCurrentLocale("familyname", "姓");
            LM.AddToCurrentLocale("clanname",   "氏");
#endif
            LM.ApplyLocale(); //之前是只加载了, 忘记应用了
            NewUI.PatchResources();
            _ = new KingdomTaskLibrary();
            _ = new ActorTaskLibrary();
            _ = new CityTaskLibrary();
            _ = new ai.ActorJobLibrary();
            _ = new ai.CitizenJobs();
            _ = new ai.CityJobLibrary();
            professions = new content.ProfessionLibrary();
            _ = new TraitLibrary();
            _ = new content.WarTypeLibrary();
            _ = new content.LoyaltyLibrary();
            _ = new GodPowerLibrary();
            _ = new content.PlotsLibrary();
            _ = new StatusEffectLibrary();
            _ = new content.ItemLibrary();
            _ = new content.ProjectileLibrary();
            _ = new content.TooltipLibrary();
            _ = new TraitGroupLibrary();
            _ = new RacesLibrary();
            _ = new content.ActorAssetLibrary();
            _ = new KingdomAssetLibrary();
            content.BuildingLibrary.init();
            _ = new BuildingAssetLibrary();
            AW_WarManager.init();
            AW_CitiesManager.init();
            AW_KingdomManager.init();
            AW_AllianceManager.init();
            AW_UnitGroupManager.init();

            AssetManager.instance.add(UnitGroupTypeLibrary.Instance,      "unit_group_types");
            AssetManager.instance.add(CityTechLibrary.Instance,           "city_techs");
            AssetManager.instance.add(KingdomPolicyLibrary.Instance,      "kingdom_policies");
            AssetManager.instance.add(KingdomPolicyStateLibrary.Instance, "kingdom_policy_states");

            KingdomPolicyLibrary.Instance.post_init();
            KingdomPolicyStateLibrary.Instance.post_init();
            //loadBanners("Xia");
            // NewUI.init();
            NameGeneratorAssets.init();

            TabManager.init();
            WindowManager.init();
            MapModeManager.CreateMapLayer();
            //NewUI.CreateAndPatchCharIcons();
            print("Translation loaded");
            ResourceAsset resourceAsset = AssetManager.resources.get(SR.gold);
            resourceAsset.maximum = 99999999;
            AssetManager.clan_level_bonus_library.t.base_stats[S.clan_members] = 200f;
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
            NameGeneratorAssets.init();
            BannerGenerator.loadTexturesFromResources("Xia");

            MapModeManager.StartUpdate();
            patchHarmony();
        }

        private void Update()
        {
            checkRaceAdded();
            // AW_Kingdom.kingsSetThisFrame.Clear();
            if (skip_frame-- > 0) return;

            _ = EventsManager.Instance;
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
            instance = this;

            Configure();

            prefabs_library = new GameObject("Prefabs").transform;
            prefabs_library.SetParent(transform);
            _ = new AWS();
            _ = new AWDataS();
            _ = new PolicyState();
            _ = new PolicyStateType();
            _ = new TechType();

            BaseInstPredictor.init();
            HarmonyTools.ReplaceMethods();
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
                    DebugConfig.setOption(DebugOption.CitizenJobAttacker,  true);
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
            _ = new MapIconPatch();
            Harmony.CreateAndPatchAll(typeof(PathFinderPatch));
            Harmony.CreateAndPatchAll(typeof(ClanManagerPatch));
            print("Create and patch all:CTraits");
        }

        public static void LogWarning(string pMessage, bool pShowStackTrace = false, bool pLogOnlyOnce = false)
        {
            if (pLogOnlyOnce)
                if (!_logged_warnings.Add(pMessage))
                    return;
            LogService.LogWarning($"[AW2.0]: {pMessage}");
            if (pShowStackTrace)
            {
                LogService.LogStackTraceAsWarning();
            }
        }

        public static void LogInfo(string pMessage, bool pShowStackTrace = false, bool pLogOnlyOnce = false)
        {
            if (pLogOnlyOnce)
                if (!_logged_infos.Add(pMessage))
                    return;
            LogService.LogInfo($"[AW2.0]: {pMessage}");
            if (pShowStackTrace) LogService.LogStackTraceAsInfo();
        }

        public static void LogDebug(string pMessage, bool pShowStackTrace = false, bool pLogOnlyOnce = false)
        {
            if (!DebugConst.IS_DEVELOPER) return;
            if (pLogOnlyOnce)
                if (!_logged_infos.Add(pMessage))
                    return;
            LogService.LogInfo($"[AW2.0]: {pMessage}");
            if (pShowStackTrace)
            {
                LogService.LogStackTraceAsInfo();
            }
        }
    }
}