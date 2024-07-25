using AncientWarfare.Core.Force;
using AncientWarfare.LocaleKeys;
using HarmonyLib;
using NeoModLoader.api.attributes;
using NeoModLoader.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace AncientWarfare.Utils
{
    public static class WorldLogHelper
    {
        public static void LogNewTribe(Tribe tribe)
        {
            WorldLogMessage msg = new WorldLogMessage(WorldLogKeys.new_tribe, tribe.GetName());
            msg.add();
        }
    }
}
