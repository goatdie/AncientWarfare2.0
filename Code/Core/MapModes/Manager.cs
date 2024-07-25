using AncientWarfare.Abstracts;
using NeoModLoader.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace AncientWarfare.Core.MapModes
{
    internal class Manager : IManager
    {
        private static bool is_running;
        public static CustomMapLayer map_layer { get; private set; }

        public static CustomMapMode map_mode
        {
            get
            {
                if (true || PlayerConfig.optionBoolEnabled("map_tribe_zones"))
                    return CustomMapMode.Tribe;
                return CustomMapMode.Hidden;
            }
        }
        internal void StartUpdate()
        {
            new Thread(() =>
            {
                try
                {
                    is_running = true;
                    while (true)
                    {
                        map_layer.PreparePixels();
                        Thread.Sleep((int)(500 / Math.Max(Config.timeScale, 1)));
                    }
                }
                catch (Exception e)
                {
                    is_running = false;
                    LogService.LogErrorConcurrent($"Error when updating map mode: {map_mode}. It is now disabled.");
                    LogService.LogErrorConcurrent(e.Message);
                    LogService.LogErrorConcurrent(e.StackTrace);
                }
            }).Start();
        }

        public static void SetAllDirty()
        {
            map_layer.SetAllDirty();
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
    }
}
