using NCMS.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Figurebox
{
    public class KingdomHistoryListWindow : MonoBehaviour
    {
        //通过initialized来判断是否已经初始化, 防止未初始化就调用的情况
        private static bool initialized;
        public GameObject ButtonPrefab;
        public Transform ButtonParent;

        private void OnEnable()
        {
            if (!initialized) return;
            // 创建按钮
            CreatekingdomButtons();
        }

        public static void Init()
        {
            // 获取Scroll View对象
            var scrollView =
                GameObject.Find(
                    $"/Canvas Container Main/Canvas - Windows/windows/kingdomHistoryListWindow/Background/Scroll View");

            // 获取Content对象，这是所有按钮的父对象
            var content = scrollView.transform.Find("Viewport/Content");

            // 添加VerticalLayoutGroup组件来自动排列按钮
            var layout = content.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childControlHeight = true;
            layout.childControlWidth = true;

            // 添加ContentSizeFitter组件使Content可以自动调整大小
            var contentSizeFitter = content.gameObject.AddComponent<ContentSizeFitter>();
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            KingdomHistoryListWindow historyList = scrollView.AddComponent<KingdomHistoryListWindow>();

            //contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained; 
            //contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            initialized = true;
        }

        public static void openWindow()
        {
            // 显示窗口
            Windows.ShowWindow("kingdomHistoryListWindow");
        }

        public static void CreatekingdomButtons()
        {
            // 从KingdomManager中获取所有的kingdom
            var kingdoms = FunctionHelper.kingdomids;

            // 获取Scroll View对象
            var scrollView =
                GameObject.Find(
                    "/Canvas Container Main/Canvas - Windows/windows/kingdomHistoryListWindow/Background/Scroll View");

            // 获取Content对象，这是所有按钮的父对象
            var content = scrollView.transform.Find("Viewport/Content");

            // 清除所有现有的按钮
            foreach (Transform child in content)
            {
                Destroy(child.gameObject);
            }

            // 为每一个kingdom创建一个按钮
            // 为每一个kingdom创建一个按钮
            foreach (var kingdomId in kingdoms)
            {
                // 使用kingdomId获取Kingdom实例
                Kingdom kingdom = BehaviourActionBase<Kingdom>.world.kingdoms.get(kingdomId);


                // 如果获取不到Kingdom实例，跳过


                if (FunctionHelper.kingdomCityNameyData.ContainsKey(kingdomId))
                {
                    var buttonWrapper =
                        new GameObject($"Kingdom_{FunctionHelper.kingdomCityNameyData[kingdomId]}_Button");
                    buttonWrapper.transform.SetParent(content, false);

                    var layoutElement = buttonWrapper.AddComponent<LayoutElement>();
                    layoutElement.preferredHeight = 30; // 设置你想要的大小

                    // 创建旗帜，并将其放在按钮的左侧60的位置
                    if (kingdom != null)
                    {
                        GameObject banner = NewUI.createKingdomBanner(buttonWrapper, kingdom, new Vector3(-60, 0, 0));
                    }


                    // 创建按钮
                    NewUI.createTextButtonWSize(
                        $"Kingdom_{FunctionHelper.kingdomCityNameyData[kingdomId]}_TextButton",
                        $"Kingdom {FunctionHelper.kingdomCityNameyData[kingdomId]}",
                        new Vector2(0, 0), // Position will be set by VerticalLayoutGroup
                        Color.white, // Set appropriate color
                        buttonWrapper.transform, // Parent to ButtonWrapper object instead of Content
                        () => OpenKingdomHistory(kingdomId),
                        new Vector2(0, 0) // Position and size will be set by ButtonWrapper
                    );
                }
                else
                {
                    Debug.Log($"Kingdom {kingdomId} not found in kingdomCityNameyData");
                }
            }
        }

        public static void OpenKingdomHistory(string kingdomId)
        {
            Windows.ShowWindow("kingdomHistoryWindow");
            KingdomHistoryWindow.AddKingdomAndKingToWindow(kingdomId);

            // 在这里添加打开具有相应王国历史信息的窗口的代码
        }
    }
}