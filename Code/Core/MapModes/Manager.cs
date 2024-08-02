using System;
using System.Threading;
using AncientWarfare.Abstracts;
using AncientWarfare.Const;
using UnityEngine;

namespace AncientWarfare.Core.MapModes
{
    internal class Manager : IManager
    {
        public static CustomMapLayer map_layer { get; private set; }

        public static CustomMapMode map_mode
        {
            get
            {
                if (PlayerConfig.optionBoolEnabled(ToggleNames.map_tribe_zones))
                    return CustomMapMode.Tribe;
                return CustomMapMode.Hidden;
            }
        }

        public void Initialize()
        {
            GameObject custom_map_layer_obj =
                new("[layer]Ancient Warfare Layer", typeof(CustomMapLayer), typeof(SpriteRenderer));
            custom_map_layer_obj.transform.SetParent(World.world.transform);
            custom_map_layer_obj.transform.localPosition = Vector3.zero;
            custom_map_layer_obj.transform.localScale = Vector3.one;
            custom_map_layer_obj.GetComponent<SpriteRenderer>().sortingOrder = 1;
            map_layer = custom_map_layer_obj.GetComponent<CustomMapLayer>();
            World.world.mapLayers.Add(map_layer);

            StartUpdate();
        }

        internal void StartUpdate()
        {
            new Thread(() =>
            {
                while (true)
                    try
                    {
                        Thread.Sleep((int)(500 / Math.Max(Config.timeScale, 1)));
                        map_layer.PreparePixels();
                    }
                    catch (Exception e)
                    {
                        //is_running = false;
                        Main.LogDebug("游戏时间倍率过高", pLogOnlyOnce: true, pConcurrent: true,
                                      pLevel: DebugMsgLevel.Warning);
                        Main.LogDebug(e.Message, pLogOnlyOnce: true, pConcurrent: true, pLevel: DebugMsgLevel.Warning);
                        //LogService.LogErrorConcurrent(e.StackTrace);
                    }
            }).Start();
        }

        public static void SetAllDirty()
        {
            map_layer.SetAllDirty();
        }
    }
}