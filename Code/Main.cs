using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AncientWarfare.Abstracts;
using AncientWarfare.Attributes;
using AncientWarfare.Const;
using AncientWarfare.Core.Extensions;
using AncientWarfare.Utils;
using NCMS.Extensions;
using NeoModLoader.api;
using NeoModLoader.api.attributes;
using NeoModLoader.General;
using NeoModLoader.General.Game.extensions;
using NeoModLoader.services;
using UnityEngine;

namespace AncientWarfare
{
    internal enum DebugMsgLevel
    {
        Info,
        Warning,
        Error
    }

    internal class Main : BasicMod<Main>, IReloadable
    {
        public static Transform prefabs_library;

        public static Main instance;

        private static readonly HashSet<string> _logged_warnings = new();
        private static readonly HashSet<string> _logged_infos    = new();

        public static string mainPath => mod_declare.FolderPath; // 这种方式更鲁棒, 可以适配不同的模组文件夹位置
        public static ModDeclare mod_declare { get; private set; }

        void Awake()
        {
            LM.AddToCurrentLocale("", "");
            ResourceAsset resourceAsset = AssetManager.resources.get(SR.gold);
            resourceAsset.maximum = 99999999;
            AssetManager.clan_level_bonus_library.t.base_stats[S.clan_members] = 200f;
        }


        void Start()
        {
            BannerGenerator.loadTexturesFromResources("Xia");
        }

        private void Update()
        {
            var elapsed = World.world.getCurElapsed();

            TribePlaceFinder.I.Update(elapsed);
        }

        [Hotfixable]
        public void Reload()
        {
            return;
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
            /*
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
            */
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

            LogInfo($"IS_DEVELOPER: {DebugConst.IS_DEVELOPER}, EDITOR_INMNY: {DebugConst.EDITOR_INMNY}");
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

        public static void LogDebug(string        pMessage, bool pShowStackTrace = false, bool pLogOnlyOnce = false,
                                    DebugMsgLevel pLevel = DebugMsgLevel.Info, bool pConcurrent = false)
        {
            if (!DebugConst.IS_DEVELOPER) return;
            if (pLogOnlyOnce)
                if (!_logged_infos.Add(pMessage))
                    return;
            var info_to_log = $"[AW2.0]: {pMessage}";
            switch (pLevel, pConcurrent)
            {
                case (DebugMsgLevel.Info, false):
                    LogService.LogInfo(info_to_log);
                    break;
                case (DebugMsgLevel.Info, true):
                    LogService.LogInfoConcurrent(info_to_log);
                    break;
                case (DebugMsgLevel.Warning, false):
                    LogService.LogWarning(info_to_log);
                    break;
                case (DebugMsgLevel.Warning, true):
                    LogService.LogWarningConcurrent(info_to_log);
                    break;
                case (DebugMsgLevel.Error, false):
                    LogService.LogError(info_to_log);
                    break;
                case (DebugMsgLevel.Error, true):
                    LogService.LogErrorConcurrent(info_to_log);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(pLevel), pLevel, null);
            }

            if (pShowStackTrace)
            {
                switch (pLevel)
                {
                    case DebugMsgLevel.Info:
                        LogService.LogStackTraceAsInfo();
                        break;
                    case DebugMsgLevel.Warning:
                        LogService.LogStackTraceAsWarning();
                        break;
                    case DebugMsgLevel.Error:
                        LogService.LogStackTraceAsError();
                        break;
                }
            }
        }

        protected override void OnModLoad()
        {
            Configure();
            mod_declare = GetDeclaration();

            prefabs_library = new GameObject("Prefabs").transform;
            prefabs_library.SetParent(transform);

            HarmonyTools.PatchAll();
            HarmonyTools.ReplaceMethods();


            var types = Assembly.GetExecutingAssembly().GetTypes();
            types.Where(t => t.IsSubclassOf(typeof(StringLibrary)) && !t.IsAbstract).ForEach(t =>
            {
                _ = Activator.CreateInstance(t);
                LogDebug($"Initialize string library: {t.Name}");
            });

            var manager_types = types
                                .Where(t => typeof(IManager).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                                .ToList();
            SortManagerTypes(manager_types);
            foreach (var t in manager_types)
            {
                try
                {
                    var manager = (IManager)Activator.CreateInstance(t, true);
                    manager?.Initialize();
                    LogDebug($"Initialize manager: {t.FullName}(null?{manager == null})");
                }
                catch (Exception e)
                {
                    LogError($"Failed to initialize manager: {t.FullName}");
                    LogError(e.Message);
                    LogError(e.StackTrace);
                }
            }

            AssetManager.buildings.ForEach<BuildingAsset, BuildingLibrary>(x =>
            {
                x.cityBuilding = false;
                if (x.type == SB.type_hall)
                {
                    x.storage = true;
                    x.cost.gold = 0;
                    x.cost.common_metals = 0;
                    x.cost.stone = 0;
                    x.GetAdditionAsset().storage_size = 20 * (int)Math.Pow(2, x.upgradeLevel);
                }
            });
        }

        private static void SortManagerTypes(List<Type> manager_types)
        {
            for (int i = 0; i < manager_types.Count; i++)
            {
                var i_attr = manager_types[i].GetCustomAttribute<ManagerInitializeAfterAttribute>();
                if (i_attr == null) continue;
                for (int j = i + 1; j < manager_types.Count; j++)
                {
                    if (i_attr.after_types.Contains(manager_types[j]))
                    {
                        manager_types.Swap(i, j);
                        break;
                    }
                }
            }
        }
    }
}