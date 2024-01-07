using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using NeoModLoader.General;
using UnityEngine;
using Random = System.Random;

namespace Figurebox
{

    class WorldLogPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(WorldLogMessageExtensions), "getFormatedText")]
        public static void getFormatedText(ref string __result, ref WorldLogMessage pMessage)
        {
            switch (pMessage.text)
            {
                case "baseLog":
                    __result = LM.Get(pMessage.text);
                    break;
                case "historicalMessage":
                    string text = LM.Get(pMessage.text);
                    if (pMessage.unit == null)
                    {
                        pMessage.icon = "iconDeathMark";
                        text = text.Replace("$ren$", string.Concat(new string[]
                        {
                            pMessage.special1
                        }));
                        __result = text;
                        return;
                    }

                    text = text.Replace("$ren$", string.Concat(new string[]
                    {
                        "<color=", Toolbox.colorToHex(pMessage.color_special1, true), ">", pMessage.unit.getName(),
                        "</color>"
                    }));
                    pMessage.icon = "iconDocument";
                    __result = text;
                    break;
                case "mandateofheavenMessage":
                    text = LM.Get(pMessage.text);
                    if (pMessage.unit == null)
                    {
                        pMessage.icon = "iconDead";
                        text = text.Replace("$king$", string.Concat(new string[]
                        {
                            pMessage.special2
                        }));
                        text = text.Replace("$kingdom$", string.Concat(new string[]
                        {
                            pMessage.special1
                        }));
                        __result = text;
                        return;
                    }

                    text = text.Replace("$king$", string.Concat(new string[]
                    {
                        "<color=", Toolbox.colorToHex(pMessage.color_special1), ">", pMessage.unit.data.name, "</color>"
                    }));
                    text = text.Replace("$kingdom$", string.Concat(new string[]
                    {
                        "<color=", Toolbox.colorToHex(pMessage.color_special1, true), ">", pMessage.kingdom.name,
                        "</color>"
                    }));
                    pMessage.icon = "iconKingdom";
                    __result = text;
                    break;
                case "losemandateofheavenMessage":
                    text = LM.Get(pMessage.text);
                    if (pMessage.unit == null)
                    {
                        pMessage.icon = "iconDead";
                        text = text.Replace("$king$", string.Concat(new string[]
                        {
                            pMessage.special2
                        }));
                        text = text.Replace("$kingdom$", string.Concat(new string[]
                        {
                            pMessage.special1
                        }));
                        __result = text;
                        return;
                    }

                    text = text.Replace("$king$", string.Concat(new string[]
                    {
                        "<color=", Toolbox.colorToHex(pMessage.color_special1), ">", pMessage.unit.data.name, "</color>"
                    }));
                    text = text.Replace("$kingdom$", string.Concat(new string[]
                    {
                        "<color=", Toolbox.colorToHex(pMessage.color_special1, true), ">", pMessage.kingdom.name,
                        "</color>"
                    }));
                    pMessage.icon = "iconDocument";
                    __result = text;
                    break;
                case "warmandateofheavenMessage":
                    text = LM.Get(pMessage.text);
                    text = text.Replace("$kingdom$", string.Concat(new string[]
                    {
                        "<color=", Toolbox.colorToHex(pMessage.color_special1, true), ">", pMessage.special2, "</color>"
                    }));
                    text = text.Replace("$kingdom2$", string.Concat(new string[]
                    {
                        "<color=", Toolbox.colorToHex(pMessage.color_special2, true), ">", pMessage.special1, "</color>"
                    }));
                    pMessage.icon = "iconDocument";
                    __result = text;
                    break;
                case "losekingdommandateofheavenMessage":
                    text = LM.Get(pMessage.text);
                    text = text.Replace("$kingdom$", string.Concat(new string[]
                    {
                        "<color=", Toolbox.colorToHex(pMessage.color_special1, true), ">", pMessage.special1, "</color>"
                    }));
                    pMessage.icon = "iconDocument";
                    __result = text;
                    break;
                case "joinanotherkingdomMessage":
                    text = LM.Get(pMessage.text);

                    text = text.Replace("$kingdom$", string.Concat(new string[]
                    {
                        "<color=", Toolbox.colorToHex(pMessage.color_special1, true), ">", pMessage.special2, "</color>"
                    }));
                    text = text.Replace("$kingdom2$", string.Concat(new string[]
                    {
                        "<color=", Toolbox.colorToHex(pMessage.color_special2, true), ">", pMessage.special1, "</color>"
                    }));
                    pMessage.icon = "iconDocument";
                    __result = text;
                    break;
                case "reclaimwarendMessage":
                    text = LM.Get(pMessage.text);
                    text = text.Replace("$kingdom$", string.Concat(new string[]
                    {
                        "<color=", Toolbox.colorToHex(pMessage.color_special1, true), ">", pMessage.special2, "</color>"
                    }));
                    text = text.Replace("$kingdom2$", string.Concat(new string[]
                    {
                        "<color=", Toolbox.colorToHex(pMessage.color_special2, true), ">", pMessage.special1, "</color>"
                    }));
                    text = text.Replace("$winner$", string.Concat(new string[]
                    {
                        "<color=", Toolbox.colorToHex(pMessage.color_special3, true), ">", pMessage.special3, "</color>"
                    }));
                    pMessage.icon = "iconDocument";
                    __result = text;
                    break;
                case "usurpationMessage":
                    text = LM.Get(pMessage.text);
                    text = text.Replace("$kingdom$", string.Concat(new string[]
                    {
                        "<color=", Toolbox.colorToHex(pMessage.color_special1, true), ">", pMessage.special2, "</color>"
                    }));
                    text = text.Replace("$first$", string.Concat(new string[]
                    {
                        "<color=", Toolbox.colorToHex(pMessage.color_special1, true), ">", pMessage.special1, "</color>"
                    }));
                    pMessage.icon = "iconDocument";
                    __result = text;
                    break;
                case "vassalWarStartMessage":
                    text = LM.Get(pMessage.text);
                    text = text.Replace("$first$", string.Concat(new string[]
                    {
                        "<color=", Toolbox.colorToHex(pMessage.color_special1, true), ">", pMessage.special2, "</color>"
                    }));
                    text = text.Replace("$kingdom2$", string.Concat(new string[]
                    {
                        "<color=", Toolbox.colorToHex(pMessage.color_special2, true), ">", pMessage.special1, "</color>"
                    }));
                    pMessage.icon = "iconDocument"; // 修改为你想要的图标
                    __result = text;
                    break;
                case "vassalWarEndMessage":
                    text = LM.Get(pMessage.text);
                    text = text.Replace("$first$", string.Concat(new string[]
                    {
                        "<color=", Toolbox.colorToHex(pMessage.color_special1, true), ">", pMessage.special2, "</color>"
                    }));
                    text = text.Replace("$kingdom2$", string.Concat(new string[]
                    {
                        "<color=", Toolbox.colorToHex(pMessage.color_special2, true), ">", pMessage.special1, "</color>"
                    }));
                    pMessage.icon = "iconDocument"; // 修改为你想要的图标
                    __result = text;
                    break;
                case "IndependenceWarMessage":
                    text = LM.Get(pMessage.text);
                    text = text.Replace("$kingdom$", string.Concat(new string[]
                    {
                        "<color=", Toolbox.colorToHex(pMessage.color_special1, true), ">", pMessage.special2, "</color>"
                    }));
                    text = text.Replace("$kingdom2$", string.Concat(new string[]
                    {
                        "<color=", Toolbox.colorToHex(pMessage.color_special2, true), ">", pMessage.special1, "</color>"
                    }));
                    pMessage.icon = "iconDocument";
                    __result = text;
                    break;
            }
        }
    }
}