using NCMS.Utils;
using NeoModLoader.General.UI.Window;
using UnityEngine;
using UnityEngine.UI;

namespace Figurebox.ui.windows
{
    class KingdomPolicyWindow : AutoLayoutWindow<KingdomPolicyWindow>
    {
        public static KingdomPolicyWindow instance;
        private static bool UIReadyToLoad = false;
        private static bool Loading = false;
        public static Kingdom currentKingdom;
        private static Button KingdomPolicyWindowEntry;
        private static GameObject kingdomAndKingTextObject;

        protected override void Init()
        {
            // 在这里实现窗口内容的初始化
        }

        // 1层封装
        public static void init()
        {
            instance = CreateWindow("KingdomPolicyWindow", "KingdomPolicyWindow Title");


            KingdomPolicyWindowEntry = NewUI.createBGWindowButton(
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