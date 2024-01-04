using System.Collections.Generic;
using NCMS.Utils;
using UnityEngine;

namespace Figurebox
{
    class WindowManager
    {
        public static Dictionary<string, GameObject> windowContents = new Dictionary<string, GameObject>();
        public static Dictionary<string, ScrollWindow> createdWindows = new Dictionary<string, ScrollWindow>();


        public static void init()
        {
            newWindow("tianmingBoardWindow", "Mandate of Heaven Board");
            newWindow("cityHistoryWindow", "City History");
            newWindow("kingdomHistoryWindow", "Kingdom History");
            newWindow("kingdomHistoryListWindow", "Kingdom History List");
            newWindow("warinfoWindow", "War Info");
            TianmingBoardWindow.init();
            KingdomHistoryListWindow.Init();

            KingdomPolicyGraphWindow.CreateAndInit(nameof(KingdomPolicyGraphWindow));
        }

        private static void newWindow(string id, string title)
        {
            ScrollWindow window;
            GameObject content;
            window = Windows.CreateNewWindow(id, title);
            createdWindows.Add(id, window);

            GameObject scrollView =
                GameObject.Find(
                    $"/Canvas Container Main/Canvas - Windows/windows/{window.name}/Background/Scroll View");
            scrollView.gameObject.SetActive(true);

            content = GameObject.Find(
                $"/Canvas Container Main/Canvas - Windows/windows/{window.name}/Background/Scroll View/Viewport/Content");
            if (content != null)
            {
                windowContents.Add(id, content);
            }
        }

        public static void updateScrollRect(GameObject content, int count, int size)
        {
            var scrollRect = content.GetComponent<RectTransform>();
            scrollRect.sizeDelta = new Vector2(0, count * size);
        }
    }
}