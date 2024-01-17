using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NCMS.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Figurebox
{
    class KingdomHistoryWindow : MonoBehaviour
    {
        private static GameObject contents;
        private static GameObject scrollView;
        public static GameObject content;
        private static Vector2 originalSize;
        public static KingdomHistoryWindow instance;
        private static bool UIReadyToLoad = false;
        private static bool loading = false;
        public static Kingdom currentKingdom;
        private static Button KingdomHisotrybutton;
        private static GameObject kingdomAndKingTextObject;
        private static ContentSizeFitter contentSizeFitter;


        public static void init()
        {
            scrollView =
                GameObject.Find(
                    $"Canvas Container Main/Canvas - Windows/windows/kingdomHistoryWindow/Background/Scroll View");
            contents = WindowManager.windowContents["kingdomHistoryWindow"];
            instance = new GameObject("KingdomHistoryWindowInstance").AddComponent<KingdomHistoryWindow>();

            // 添加 Vertical Layout Group 和 Content Size Fitter 组件
            VerticalLayoutGroup verticalLayoutGroup = contents.AddComponent<VerticalLayoutGroup>();
            verticalLayoutGroup.childForceExpandHeight = false;
            verticalLayoutGroup.spacing = 15;

            ContentSizeFitter contentSizeFitter = contents.AddComponent<ContentSizeFitter>();
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            originalSize = contents.GetComponent<RectTransform>().sizeDelta;
            contents.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 1500);
/*
            KingdomHisotrybutton = NewUI.createBGWindowButton(
                GameObject.Find($"Canvas Container Main/Canvas - Windows/windows/kingdom"),
                -50,
                "iconworldlaw",
                "KingdomHistory",
                "Kingdom History",
                "Shows a kingdom's history",
                openWindow
            );
            */
        }


        public static void openWindow()
        {
            Windows.ShowWindow("kingdomHistoryWindow");
            AddKingdomAndKingToWindow(currentKingdom.data.id);
        }

        public static void AddKingdomAndKingToWindow(string kingdomId)
        {
            // 设置文本的初始位置
            int posY = -35;

            // 删除之前的内容
            foreach (Transform child in contents.transform)
            {
                Destroy(child.gameObject);
            }

            if (string.IsNullOrEmpty(kingdomId))
            {
                Debug.LogWarning("No kingdom selected.");
                return;
            }

            // 创建一个新的列表来保存所有的统治事件
            List<string> allRulingEvents = new List<string>();

            // 将每个统治事件添加到列表中
            foreach (var pair in FunctionHelper.KingStartYearInKingdom)
            {
                string[] parts = pair.Key.Split('-');
                if (parts[1] == kingdomId)
                {
                    allRulingEvents.Add(pair.Key);
                }
            }

            // 按照统治的开始年份对列表进行排序
            allRulingEvents.Sort((event1, event2) =>
                FunctionHelper.KingStartYearInKingdom[event1].CompareTo(FunctionHelper.KingStartYearInKingdom[event2]));


            // 按行显示王国及其历史纪录
            foreach (string rulingEvent in allRulingEvents)
            {
                string[] parts = rulingEvent.Split('-');
                string kingId = parts[0];
                string kingAndKingdomKey = rulingEvent;
                int kingEndYear = World.world.mapStats.getCurrentYear();
                int currentYear = World.world.mapStats.getCurrentYear();
                int warEndYear = World.world.mapStats.getCurrentYear();
                int warCount = 0;
                float yOffset = 1.0f;
                int offsetYAttacker = 10;
                int offsetYDefender;
                int year = FunctionHelper.KingStartYearInKingdom.ContainsKey(kingAndKingdomKey)
                    ? FunctionHelper.KingStartYearInKingdom[kingAndKingdomKey]
                    : 0;
                // 使用kingdomId获取Kingdom实例
                if (!FunctionHelper.KingEndYearInKingdom.TryGetValue(kingAndKingdomKey, out kingEndYear))
                {
                    kingEndYear = World.world.mapStats.getCurrentYear();
                }
                else
                {
                    kingEndYear = FunctionHelper.KingEndYearInKingdom[kingAndKingdomKey];
                }

                Kingdom kingdom = BehaviourActionBase<Kingdom>.world.kingdoms.get(kingdomId);


                Dictionary<string, Tuple<double, List<string>, List<string>, double, bool>> kingdomWarsInfo =
                    new Dictionary<string, Tuple<double, List<string>, List<string>, double, bool>>();

                foreach (var warId in FunctionHelper.WarStartDate.Keys)
                {
                    double warStartYear = FunctionHelper.WarStartDate[warId];

                    int kingStartYear = FunctionHelper.KingStartYearInKingdom[kingAndKingdomKey];
                    int warintstartyear = ExtractYear(warStartYear);


                    // 尝试获取字典中的值，如果键不存在，则返回当前年份


                    // Try to get the value from the dictionary, if the key does not exist, return the current year
                    if (!FunctionHelper.WarEndDate.TryGetValue(warId, out warEndYear))
                    {
                        warEndYear = World.world.mapStats.getCurrentYear();
                    }
                    else
                    {
                        warEndYear = FunctionHelper.WarEndDate[warId];
                    }


                    if ((FunctionHelper.Attackers[warId].Contains(kingdomId) ||
                         FunctionHelper.Defenders[warId].Contains(kingdomId)) &&
                        (warintstartyear + 10 >= kingStartYear && warEndYear - 10 <= kingEndYear))
                    {
                        double warEndYearFloat;
                        bool isWarOngoing = !FunctionHelper.WarEndDateFloat.TryGetValue(warId, out warEndYearFloat);

                        kingdomWarsInfo[warId] = new Tuple<double, List<string>, List<string>, double, bool>(
                            FunctionHelper.WarStartDate[warId],
                            FunctionHelper.Attackers[warId],
                            FunctionHelper.Defenders[warId],
                            warEndYearFloat,
                            isWarOngoing);
                    }
                }

                //待解决问题 同一个人在同一个王国统治第二次应该当新王一样加进来
                List<string> allVassalRelationships = new List<string>();

                // 将每个附庸关系添加到列表中
                foreach (var pair in FunctionHelper.kingdomVassalEstablishmentTime)
                {
                    string[] parts1 = pair.Key.Split('-');
                    if (parts1[0] == kingdomId || parts1[1] == kingdomId)
                    {
                        allVassalRelationships.Add(pair.Key);
                    }
                }

                // 按照附庸关系的建立时间对列表进行排序
                allVassalRelationships.Sort((relation1, relation2) =>
                    FunctionHelper.kingdomVassalEstablishmentTime[relation1]
                        .CompareTo(FunctionHelper.kingdomVassalEstablishmentTime[relation2]));

                string vassalLordInfo = "";

                foreach (string vassalRelation in allVassalRelationships)
                {
                    string[] parts1 = vassalRelation.Split(new char[] { '-' }, 3); // 分割成三个部分：宗主国id，附庸国id和计数器
                    string lordId = parts1[0];
                    string vassalId = parts1[1];

                    // 通过关系ID确定vassal和lord名称
                    string vassalName = FunctionHelper.kingdomCityNameyData.ContainsKey(vassalId)
                        ? FunctionHelper.kingdomCityNameyData[vassalId]
                        : "Unknown Kingdom";
                    string lordName = FunctionHelper.kingdomCityNameyData.ContainsKey(lordId)
                        ? FunctionHelper.kingdomCityNameyData[lordId]
                        : "Unknown Kingdom";

                    // 获取附庸关系的建立年份
                    int startYear = FunctionHelper.kingdomVassalEstablishmentTime[vassalRelation];
                    int kingStartYear = FunctionHelper.KingStartYearInKingdom[kingAndKingdomKey];

                    // 尝试获取附庸关系的结束年份，如果关系还在继续，就使用当前年份
                    int endYear = FunctionHelper.kingdomVassalEndTime.ContainsKey(vassalRelation)
                        ? FunctionHelper.kingdomVassalEndTime[vassalRelation]
                        : World.world.mapStats.getCurrentYear();

                    // 判断当前kingdom是否为vassal或lord，并将相关信息添加到UI中
                    if ((startYear <= kingEndYear && endYear >= kingStartYear))
                    {
                        // 判断当前kingdom是否为vassal或lord，并将相关信息添加到UI中
                        if (kingdomId == lordId)
                        {
                            string endInfo = endYear == currentYear ? "Ongoing" : endYear.ToString();
                            vassalLordInfo +=
                                string.Format("\n<b>Vassal:</b>\n{0}\nYear Established: {1}\nYear Ended: {2}\n",
                                    vassalName, startYear, endInfo);
                        }
                        else if (kingdomId == vassalId)
                        {
                            string endInfo = endYear == currentYear ? "Ongoing" : endYear.ToString();
                            vassalLordInfo +=
                                string.Format("\n<b>Lord:</b>\n{0}\nYear Established: {1}\nYear Ended: {2}\n", lordName,
                                    startYear, endInfo);
                        }
                    }
                }

                Text kingdomAndKingText = new GameObject("KingdomAndKingText").AddComponent<Text>();
                // 按照战争的创建时间对战争信息进行排序
                var sortedKingdomWarsInfo =
                    kingdomWarsInfo.OrderBy(warInfo => ExtractYear(warInfo.Value.Item1)).ToList();

                StringBuilder warInfo = new StringBuilder();

                foreach (var warInfoEntry in sortedKingdomWarsInfo)
                {
                    string warStartDate = World.world.mapStats.getDate(warInfoEntry.Value.Item1);


                    string warEndYearStr = warInfoEntry.Value.Item5
                        ? "Ongoing War"
                        : World.world.mapStats.getDate(warInfoEntry.Value.Item4);
                    GameObject warBannerContainer = new GameObject("WarBannerContainer");
                    warBannerContainer.transform.SetParent(kingdomAndKingText.transform, false);

                    StringBuilder attackerNames = new StringBuilder();
                    foreach (var attackerId in warInfoEntry.Value.Item2)
                    {
                        string kingdomName = FunctionHelper.kingdomCityNameyData.ContainsKey(attackerId)
                            ? FunctionHelper.kingdomCityNameyData[attackerId]
                            : "Unknown Kingdom";
                        attackerNames.AppendLine(kingdomName);

                        Kingdom dkingdom = BehaviourActionBase<Kingdom>.world.kingdoms.get(attackerId);
                        if (dkingdom != null)
                        {
                            GameObject banner = NewUI.createKingdomBanner(warBannerContainer, dkingdom,
                                new Vector3(-70, offsetYAttacker, 0));
                            banner.transform.localScale = new Vector3(0.2f, 0.1f);
                            offsetYAttacker -= 10;
                        }
                    }

                    offsetYDefender = offsetYAttacker - 20;
                    StringBuilder defenderNames = new StringBuilder();
                    foreach (var defenderId in warInfoEntry.Value.Item3)
                    {
                        string kingdomName = FunctionHelper.kingdomCityNameyData.ContainsKey(defenderId)
                            ? FunctionHelper.kingdomCityNameyData[defenderId]
                            : "Unknown Kingdom";
                        defenderNames.AppendLine(kingdomName);

                        Kingdom dkingdom = BehaviourActionBase<Kingdom>.world.kingdoms.get(defenderId);
                        if (dkingdom != null)
                        {
                            GameObject banner = NewUI.createKingdomBanner(warBannerContainer, dkingdom,
                                new Vector3(-70, offsetYDefender, 0));
                            banner.transform.localScale = new Vector2(0.2f, 0.1f);
                            offsetYDefender -= 10;
                        }
                    }

                    offsetYAttacker = offsetYDefender - 45; // Adjust for next war


                    string attackersStr = attackerNames.ToString();
                    string defendersStr = defenderNames.ToString();


                    warInfo.AppendLine(
                        $"War Start Year: {warStartDate}\nWar End Year: {warEndYearStr}\nWar Name: {FunctionHelper.warIdNameDict[warInfoEntry.Key]}\nAttackers:\n{attackersStr}\nDefenders:\n{defendersStr}");
                }


                // 创建一个新的Text组件，用于显示kingdom及其新的king 


                // 设置字体，字体大小和颜色
                kingdomAndKingText.font = (Font)Resources.Load("Fonts/Roboto-Bold", typeof(Font));
                kingdomAndKingText.fontSize = 8;
                kingdomAndKingText.color = Color.white;

                Debug.Log($"Creating text for king {kingId} at year {year}");

                // 设置要显示的文本
                if (FunctionHelper.KingName.ContainsKey(kingId) && FunctionHelper.KingKingdomName.ContainsKey(kingId))
                {
                    // 获取所有的王国名

                    List<string> allKingdomNames = FunctionHelper.KingKingdomName[kingId];

                    // Add a newline after each kingdom name
                    string allKingdomNamesStr = string.Join("\n", allKingdomNames);
                    // Now when displaying the text, each kingdom name will be on a new line
                    string kingEndYearStr = (kingEndYear == World.world.mapStats.getCurrentYear())
                        ? "Still in Power"
                        : kingEndYear.ToString();
                    kingdomAndKingText.text =
                        $"<i><b><color=#FFC535>King:</color></b> {FunctionHelper.KingName[kingId]}</i>  Started Rule: {year}\nEnd Rule: {kingEndYearStr}\nKingdoms:\n{allKingdomNamesStr}\nRuled for {FunctionHelper.kingYearData[kingId]} Years\nVassal-Lord Relationships:{vassalLordInfo}\nWars:\n{warInfo.ToString()}";
                }
                else
                {
                    Debug.LogWarning($"Kingdom ID {kingId} not found in KingName or KingKingdomName.");
                    kingdomAndKingText.text = $"Kingdom ID {kingId} not found. Year: {year}";
                }

                // 创建一个RectTransform组件，用于调整文本的大小和位置
                RectTransform kingdomAndKingTextRect = kingdomAndKingText.GetComponent<RectTransform>();

                // 将RectTransform组件设为其他GameObject的子对象，例如将其添加到Canvas上
                kingdomAndKingTextRect.SetParent(contents.transform, false);

                // 调整文本的位置和大小
                kingdomAndKingTextRect.anchoredPosition = new Vector2(-0, posY);
                kingdomAndKingTextRect.sizeDelta = new Vector2(280, 40); // 更改这个值以调整文本的高度

                // 添加ContentSizeFitter组件，使文本框的大小自动调整以适应其内容
                var textContentSizeFitter = kingdomAndKingText.gameObject.AddComponent<ContentSizeFitter>();
                textContentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                textContentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

                // 获取文本的高度
                float textHeight = kingdomAndKingText.preferredHeight;

                // 更新下一行文本的位置
                posY -= (int)textHeight + 20; // 在文本的高度和下一行之间保持10的间隔
            }
        }


        public static int ExtractYear(double time)
        {
            int year = (int)(time / 5f / 12f);
            year++; // 这是因为在你的getDate函数中增加了一年
            return year;
        }
    }
}