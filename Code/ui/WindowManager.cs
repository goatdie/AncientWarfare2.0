using System.Collections.Generic;
using Figurebox.ui.windows;
using NCMS.Utils;
using UnityEngine;

namespace Figurebox
{
    class WindowManager
    {
        public static Dictionary<string, GameObject> windowContents = new Dictionary<string, GameObject>();

        public static void init()
        {
            KingdomPolicyGraphWindow.CreateAndInit(nameof(KingdomPolicyGraphWindow));
            KingdomHistoryWindow.CreateAndInit(nameof(KingdomHistoryWindow));
        }
    }
}