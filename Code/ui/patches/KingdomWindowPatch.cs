using Figurebox.core;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
namespace Figurebox.UI.Patches;

class KingdomWindowPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(KingdomWindow), "showInfo")]
    public static void Vassals_Post(KingdomWindow __instance)
    {
        var suzerain = KingdomVassals.GetSuzerain(__instance.kingdom);
        // 显示宗主国信息
        __instance.showStat("Suzerain", suzerain == __instance.kingdom ? "-" : suzerain.name);
    }
    [HarmonyPostfix]
    [HarmonyPatch(typeof(KingdomWindow), "showInfo")]
    public static void Heir_Post(KingdomWindow __instance)
    {
        AW_Kingdom awKingdom = __instance.kingdom as AW_Kingdom;
        if (awKingdom != null && awKingdom.hasHeir())
        {
            // 创建角色 UI
            GameObject parent = GameObject.Find("Canvas Container Main/Canvas - Windows/windows/kingdom");
            NewUI.createActorUI(awKingdom.heir, parent, new Vector3(-120, 0, 0));

            // 获取最近添加的 UI 元素
            Transform actorUITransform = parent.transform.GetChild(parent.transform.childCount - 1);
            GameObject actorUI = actorUITransform.gameObject;

            // 缩减大小
            RectTransform rectTransform = actorUI.GetComponent<RectTransform>();
            rectTransform.localScale = new Vector3(0.5f, 0.5f, 1); // 缩小到原来的一半

            // 添加继承人文本
            GameObject textGO = new GameObject("HeirText");
            textGO.transform.SetParent(actorUI.transform, false);

            Text heirText = textGO.AddComponent<Text>();
            heirText.text = "继承人";
            heirText.alignment = TextAnchor.UpperCenter;
            heirText.color = Color.black;
            heirText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");

            RectTransform textRect = textGO.GetComponent<RectTransform>();
            textRect.localPosition = new Vector3(0, 30, 0); // 调整文本位置
            textRect.sizeDelta = new Vector2(100, 20); // 设置文本大小
        }
    }
    [HarmonyPrefix]
    [HarmonyPatch(typeof(KingdomWindow), "OnEnable")]
    public static bool KingdomOnEnable_Prefix(KingdomWindow __instance)
    {
        KingdomHistoryWindow.currentKingdom = __instance.kingdom;
        if (__instance.GetComponent<KingdomWindowAdditionComponent>() == null)
        {
            __instance.gameObject.AddComponent<KingdomWindowAdditionComponent>().Initialize();
        }
        return true;
    }
}