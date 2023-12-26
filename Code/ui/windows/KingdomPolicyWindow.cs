using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NCMS;
using NCMS.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ReflectionUtility;
using Figurebox;
using NeoModLoader.General;




namespace Figurebox
{
    class KingdomPolicyWindow : NeoModLoader.General.UI.Window.AutoLayoutWindow<KingdomPolicyWindow>
    {
        protected override void Init()
        {
            // 在这里实现窗口内容的初始化
        }
        public static KingdomPolicyWindow instance;
        private static bool UIReadyToLoad = false;
        private static bool Loading = false;
        public static Kingdom currentKingdom;
        private static Button KingdomPolicyWindowEntry
        private static GameObject kingdomAndKingTextObject;

        // 1层封装
        public static void init()
        {
            instance = KingdomPolicyWindow.CreateWindow("KingdomPolicyWindow", "KingdomPolicyWindow Title");


            KingdomPolicyWindowEntry= NewUI.createBGWindowButton(
                GameObject.Find("Canvas Container Main/Canvas - Windows/windows/kingdom"),
                -60,
                "iconworldlaw",
                "KingdomPolicy",
                "Kingdom Policy",
                "Shows a kingdom's policy",
                openWindow
            );

        }
        public static void openWindow()
        {
            Windows.ShowWindow("KingdomPolicyWindow");

        }
    }

}