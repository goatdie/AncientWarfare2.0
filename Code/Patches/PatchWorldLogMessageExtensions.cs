using AncientWarfare.Core;
using AncientWarfare.LocaleKeys;
using HarmonyLib;
using NeoModLoader.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace AncientWarfare.Patches
{
    internal static class PatchWorldLogMessageExtensions
    {
        [HarmonyPostfix, HarmonyPatch(typeof(WorldLogMessageExtensions), nameof(WorldLogMessageExtensions.getFormatedText))]
        private static void Postfix_getFormatedText(ref string __result, ref WorldLogMessage pMessage, Text pTextField, bool pColorField, bool pColorTags)
        {
            string text = LM.Get(pMessage.text);
            Color color = Toolbox.color_log_neutral;
            if (pMessage.text == WorldLogKeys.new_tribe)
            {
                color = Toolbox.color_log_neutral;
                text = text.Replace("$name$", pMessage.coloredText(pMessage.special1, pColorTags, 1));
                pMessage.icon = "iconCitySelect";
            }
            else
            {
                return;
            }
            if (pColorField)
                pTextField.color = color;

            __result = text;
        }
    }
}
