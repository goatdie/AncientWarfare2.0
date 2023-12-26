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

namespace Figurebox
{
    class TianmingBoardWindow : MonoBehaviour
    {
        public bool is_chinese = false;
        private static GameObject contents;
        private static GameObject scrollView;
        public static GameObject content;
        private static Vector2 originalSize;
        public static TianmingBoardWindow instance;
        private static List<Actor> filteredActors = new List<Actor>();
        private static bool UIReadyToLoad = false;
        private static bool loading = false;
        private static GameObject progressBar;
        private Text kingYearDataText;
        private Text YearDataText;
        private Text currentTianmingText;
        private GameObject infoHolder;

        // private ScrollRect scrollView;

        public static void init()
        {


            contents = WindowManager.windowContents["tianmingBoardWindow"];
            instance = new GameObject("TianmingBoardWindowInstance").AddComponent<TianmingBoardWindow>();
            originalSize = contents.GetComponent<RectTransform>().sizeDelta;
            contents.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 15000);
            GameObject fixedTextContainer = new GameObject("FixedTextContainer");
            fixedTextContainer.transform.SetParent(contents.transform, false);
            fixedTextContainer.AddComponent<RectTransform>();
            fixedTextContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 15000);
            // 创建 Text 组件
            instance.currentTianmingText = new GameObject("CurrentTianmingText").AddComponent<Text>();
            instance.currentTianmingText.font = (Font)Resources.Load("Fonts/Roboto-Bold", typeof(Font));
            instance.currentTianmingText.fontSize = 24;
            instance.currentTianmingText.color = Color.yellow;
            //currentTianmingText.text = $"当前天命值: {FunctionHelper.tianmingvalue}";

            // 创建 infoHolder
            instance.infoHolder = NewUI.createSubWindow(contents, new Vector3(120, -20, 0), new Vector2(350, 100), new Vector2(50, 50));

            // 将 currentTianmingText 放入 infoHolder
            RectTransform currentTianmingTextRect = instance.currentTianmingText.GetComponent<RectTransform>();
            currentTianmingTextRect.SetParent(instance.infoHolder.GetComponent<RectTransform>(), false);



            // 设置文本的位置和大小
            currentTianmingTextRect.anchoredPosition = new Vector2(-40, -30); // 根据需要调整位置
            currentTianmingTextRect.sizeDelta = new Vector2(250, 150); // 根据需要调整大小

            //scrollView = GameObject.Find($"/Canvas Container Main/Canvas - Windows/windows/tianmingBoardWindow/Background/Scroll View");

            // progressBar = NewUI.createProgressBar(scrollView, new Vector3(0, 0, 0));
            // progressBar.SetActive(false);
            /*Button filterButton = NewUI.createBGWindowButton(
                 scrollView, 
                 50, 
                 "iconSettings", 
                 "FiltersBGButton", 
                 "Filters", 
                 "Add Filters To The LeaderBoard",
                 FilterWindow.openWindow
             );*/
            // 创建并设置kingYearDataText

            //...
            // 创建并设置kingYearDataText
            instance.kingYearDataText = new GameObject("KingYearDataText").AddComponent<Text>();
            instance.kingYearDataText.font = (Font)Resources.Load("Fonts/Roboto-Bold", typeof(Font));
            instance.kingYearDataText.fontSize = 8;
            Color goldOrange = new Color(255f / 255f, 215f / 255f, 0f / 255f, 1f);
            instance.kingYearDataText.color = goldOrange;

            // 添加 ContentSizeFitter 组件
            ContentSizeFitter kingYearDataTextFitter = instance.kingYearDataText.gameObject.AddComponent<ContentSizeFitter>();
            kingYearDataTextFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            kingYearDataTextFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            RectTransform kingYearDataTextRect = instance.kingYearDataText.GetComponent<RectTransform>();
            kingYearDataTextRect.anchoredPosition = new Vector2(-200, 7415);
            kingYearDataTextRect.sizeDelta = new Vector2(200, 1100);
            kingYearDataTextRect.anchorMin = new Vector2(0.5f, 1);
            kingYearDataTextRect.anchorMax = new Vector2(0.5f, 1);
            kingYearDataTextRect.pivot = new Vector2(0.5f, 1);

            // 创建并设置YearDataText
            instance.YearDataText = new GameObject("YearDataText").AddComponent<Text>();
            instance.YearDataText.font = (Font)Resources.Load("Fonts/Roboto-Bold", typeof(Font));
            instance.YearDataText.fontSize = 8;
            instance.YearDataText.color = goldOrange;

            // 添加 ContentSizeFitter 组件
            ContentSizeFitter YearDataTextFitter = instance.YearDataText.gameObject.AddComponent<ContentSizeFitter>();
            YearDataTextFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            YearDataTextFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            RectTransform YearDataTextRect = instance.YearDataText.GetComponent<RectTransform>();
            YearDataTextRect.anchoredPosition = new Vector2(kingYearDataTextRect.anchoredPosition.x + 130, kingYearDataTextRect.anchoredPosition.y);
            YearDataTextRect.anchorMin = new Vector2(0.5f, 1);
            YearDataTextRect.anchorMax = new Vector2(0.5f, 1);
            YearDataTextRect.pivot = new Vector2(0.5f, 1);
            YearDataTextRect.sizeDelta = new Vector2(200, 11000);






            // 创建并设置currentTianmingText



            instance.CreateScrollView();
            instance.kingYearDataText.transform.SetParent(scrollView.transform, false);
            instance.YearDataText.transform.SetParent(scrollView.transform, false);



        }





        public static void openWindow()
        {
            // instance.startFilterCoroutine();
            Windows.ShowWindow("tianmingBoardWindow");
            instance.UpdateKingYearDataText();
            instance.UpdateYearDataText();
            instance.UpdateCurrentTianmingText();
        }

        private void CreateScrollView()
        {
            GameObject scrollviewObj = new GameObject("ScrollView");
            RectTransform scrollviewRect = scrollviewObj.AddComponent<RectTransform>();
            scrollviewRect.SetParent(contents.transform, false); // Set scrollView as a child of contents

            scrollviewObj.AddComponent<CanvasRenderer>();
            scrollviewObj.AddComponent<Image>();


            scrollView = scrollviewObj;
            scrollviewObj.AddComponent<ScrollRect>();
            ScrollRect scrollRectComponent = scrollView.GetComponent<ScrollRect>();
            scrollRectComponent.movementType = ScrollRect.MovementType.Elastic;
            scrollRectComponent.scrollSensitivity = 10f;
            scrollRectComponent.inertia = true;
            scrollRectComponent.decelerationRate = 0.135f;
            scrollRectComponent.elasticity = 0.1f;


            // Create a viewport
            GameObject viewport = new GameObject("Viewport");
            RectTransform viewportRect = viewport.AddComponent<RectTransform>();
            viewportRect.SetParent(scrollviewRect, false);
            viewport.AddComponent<CanvasRenderer>();
            viewport.AddComponent<Image>();
            Mask viewportMask = viewport.AddComponent<Mask>();
            viewportMask.showMaskGraphic = false;
            scrollView.GetComponent<ScrollRect>().viewport = viewportRect;
            scrollView.GetComponent<ScrollRect>().horizontal = false;


            // Create content
            content = new GameObject("Content");
            RectTransform contentRect = content.AddComponent<RectTransform>();
            contentRect.SetParent(viewportRect, false);
            content.AddComponent<CanvasRenderer>();
            content.AddComponent<Image>();
            content.AddComponent<VerticalLayoutGroup>();

            // Set up the content as a child of the viewport

            content.transform.SetParent(viewport.transform, false);
            scrollView.GetComponent<ScrollRect>().content = contentRect;




            // Add a vertical scrollbar
            GameObject scrollbarVertical = new GameObject("Scrollbar Vertical");
            RectTransform scrollbarVerticalRect = scrollbarVertical.AddComponent<RectTransform>();
            scrollbarVertical.AddComponent<CanvasRenderer>();
            scrollbarVertical.AddComponent<Image>();
            Scrollbar scrollbarVerticalScrollbar = scrollbarVertical.AddComponent<Scrollbar>();
            scrollbarVerticalScrollbar.direction = Scrollbar.Direction.BottomToTop;
            scrollView.GetComponent<ScrollRect>().verticalScrollbar = scrollbarVerticalScrollbar;
            GameObject existingScrollbar = GameObject.Find("/Canvas Container Main/Canvas - Windows/windows/tianmingBoardWindow/Background/Scroll View");
            Sprite existingSprite = existingScrollbar.GetComponent<Image>().sprite;
            scrollbarVertical.GetComponent<Image>().sprite = existingSprite;



            // Set up the scrollbar as a child of the ScrollView
            scrollbarVertical.transform.SetParent(scrollviewRect, false);
        }
        void AddItemToScrollView(GameObject newItem)
        {
            // Add the new item as a child of the content GameObject
            newItem.transform.SetParent(content.transform, false);

            // Calculate the new height of the content, based on the preferred height of all children
            float newHeight = 0;
            foreach (RectTransform child in content.transform)
            {
                newHeight += LayoutUtility.GetPreferredSize(child, 1); // 1 is the vertical axis
            }

            // Add some spacing between the items, if needed
            newHeight += content.GetComponent<VerticalLayoutGroup>().spacing * (content.transform.childCount - 1);

            // Update the content's RectTransform sizeDelta to accommodate the new height
            RectTransform contentRect = content.GetComponent<RectTransform>();
            contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, newHeight);

            // Force an immediate update of the layout
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentRect);
        }


        private static List<Actor> getActorList()
        {
            List<Actor> actorList = new List<Actor>();

            return actorList;
        }


        private void UpdateKingYearDataText()
        {


            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, string> entry in FunctionHelper.TmkingData)
            {
                sb.AppendLine($"King:{entry.Key}:{entry.Value}");
            }
            kingYearDataText.text = sb.ToString();
        }

        private void UpdateCurrentTianmingText()
        {
            is_chinese = LocalizedTextManager.instance.language == "cz" || LocalizedTextManager.instance.language == "ch";
            if (is_chinese)
            {
                currentTianmingText.text = $"当前天命: {FunctionHelper.tianmingvalue}";
            }
            else currentTianmingText.text = $"Current Dynasty Value: {FunctionHelper.tianmingvalue}";

            // 添加所有需要处理的种族名称
            string[] racesToProcess = { "human", "elf", "orc", "dwarf", "Xia" };

            // 遍历种族名称数组
            foreach (string raceName in racesToProcess)
            {
                filteredActors.Clear();
                Race race = AssetManager.raceLibrary.get(raceName);
                ActorContainer actorContainer = (ActorContainer)Reflection.GetField(typeof(Race), race, "units");
                filteredActors.AddRange(actorContainer.getSimpleList());

                foreach (Actor pActor in filteredActors)
                {
                    if (pActor.hasTrait("天命"))
                    {
                        NewUI.createActorUI(pActor, infoHolder, new Vector3(0, 0, 0));
                        Debug.Log(pActor.data.name);
                    }
                }
            }
        }


        private void UpdateYearDataText()
        {


            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, int> entry in FunctionHelper.YearData)
            {
                sb.AppendLine($"Year:{entry.Value}");
            }
            YearDataText.text = sb.ToString();
        }
        private static void setProgressBar(float pVal, float pMax, string pEnding, bool pReset = true, bool pFloat = false, bool pUpdateText = true, bool pWithoutTween = false)
        {
            StatBar statBar = progressBar.GetComponent<StatBar>();
            statBar.setBar(pVal, pMax, pEnding, pReset, pFloat, pUpdateText, pWithoutTween);
        }

    }
}