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
    class CityHistoryWindow : MonoBehaviour
    {
        private static GameObject contents;
        private static GameObject scrollView;
        public static GameObject content;
        private static Vector2 originalSize;
        public static CityHistoryWindow instance;
        private static bool UIReadyToLoad = false;
        private static bool loading = false;
        public static City currentCity;
        public static Button Hisotrybutton;
        private static GameObject cityAndKingdomTextObject;



        public static void init()
        {

            scrollView = GameObject.Find($"Canvas Container Main/Canvas - Windows/windows/cityHistoryWindow/Background/Scroll View");


            contents = WindowManager.windowContents["cityHistoryWindow"];
            instance = new GameObject("CityHistoryWindowInstance").AddComponent<CityHistoryWindow>();
            originalSize = contents.GetComponent<RectTransform>().sizeDelta;
            contents.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 1500);

            Hisotrybutton = NewUI.createBGWindowButton(
              GameObject.Find($"Canvas Container Main/Canvas - Windows/windows/village"),
              -25,
              "iconworldlaw",
              "History",
             "City History",
              "Shows a city's history",
              openWindow
          );
        }
        public static void openWindow()
        {
            Windows.ShowWindow("cityHistoryWindow");
            AddCityAndKingdomToWindow(currentCity);
            // DisplayOwnerHistory();

            // Hisotrybutton.onClick.AddListener(() =>AddCityAndKingdomToWindow(currentCity));

        }


        public static void AddCityAndKingdomToWindow(City city)
        {
            // 删除之前的内容
            foreach (Transform child in contents.transform)
            {
                Destroy(child.gameObject);
            }

            if (city == null)
            {
                Debug.LogWarning("No city selected.");
                return;
            }

            Debug.Log("city selected." + city.data.name);
            string cityId = city.data.id;

            // 获取该城市历史归属的记录
            // 按年份升序排列
            var cityHistory = FunctionHelper.CityYearData[cityId].ToList();

            // 设置文本的初始位置
            int posY = 0;

            // 按行显示城市及其历史归属
            foreach (var cityKingdomEntry in cityHistory)
            {
                string[] keyParts = cityKingdomEntry.Key.Split('-');
                string kingdomId = keyParts[0];
                int recaptureCount = int.Parse(keyParts[1]);
                int year = cityKingdomEntry.Value.Item2;

                // 检查键是否存在于字典中
                if (FunctionHelper.CityYearData.ContainsKey(cityId) && FunctionHelper.CityYearData[cityId].ContainsKey(cityKingdomEntry.Key))
                {
                    // 创建一个新的Text组件，用于显示城市及其新的王国
                    Text cityAndKingdomText = new GameObject("CityAndKingdomText").AddComponent<Text>();

                    // 设置字体，字体大小和颜色
                    cityAndKingdomText.font = (Font)Resources.Load("Fonts/Roboto-Bold", typeof(Font));
                    cityAndKingdomText.fontSize = 8;
                    cityAndKingdomText.color = Color.white;

                    // 设置要显示的文本
                    if (FunctionHelper.kingdomCityNameyData.ContainsKey(kingdomId))
                    {
                        cityAndKingdomText.text = $"Kingdom: {FunctionHelper.kingdomCityNameyData[kingdomId]} Year: {year} Recapture Count: {recaptureCount}";
                    }
                    else
                    {
                        Debug.LogWarning($"Kingdom ID {kingdomId} not found in kingdomCityNameyData.");
                        cityAndKingdomText.text = $"Kingdom ID {kingdomId} not found. Year: {year} Recapture Count: {recaptureCount}";
                    }

                    // 创建一个RectTransform组件，用于调整文本的大小和位置
                    RectTransform cityAndKingdomTextRect = cityAndKingdomText.GetComponent<RectTransform>();

                    // 将RectTransform组件设为其他GameObject的子对象，例如将其添加到Canvas上
                    cityAndKingdomTextRect.SetParent(contents.transform, false);

                    // 调整文本的位置和大小
                    cityAndKingdomTextRect.anchoredPosition = new Vector2(-100, posY);
                    cityAndKingdomTextRect.sizeDelta = new Vector2(300, 1500);

                    // 更新下一行文本的位置
                    posY -= 10;
                }
                else
                {
                    Debug.LogWarning("Key not found in CityYearData.");
                }
            }
        }









































    }
}