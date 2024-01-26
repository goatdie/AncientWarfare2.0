using System.Collections.Generic;
using System.Text;
using Figurebox.Utils;
using NeoModLoader.api.attributes;
using NeoModLoader.General;
using UnityEngine;
using UnityEngine.UI;

namespace Figurebox.ui.windows;

public partial class NewKingdomHistoryWindow
{
    private Text _review_text;

    private void InitReviewTab()
    {
        RectTransform review_tab = HistoryTabs[HistoryType.Review];

        var layout_group = review_tab.GetComponent<VerticalLayoutGroup>();
        layout_group.childControlHeight = true;
        layout_group.childControlWidth = true;
        layout_group.childForceExpandHeight = true;
        layout_group.childForceExpandWidth = true;

        var fitter = review_tab.GetComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        var text_obj = new GameObject("Review Text", typeof(Text));
        text_obj.transform.SetParent(review_tab);
        text_obj.transform.localScale = Vector3.one;
        var text = text_obj.GetComponent<Text>();
        text.text = "Review";
        text.alignment = TextAnchor.UpperCenter;
        text.font = LocalizedTextManager.currentFont;
        text.fontSize = 10;
        text.supportRichText = true;

        _review_text = text;
    }

    [Hotfixable]
    private void ShowReviewTab()
    {
        List<ReviewItem> items = new();
        if (_selectedRule == null)
            PrepareGeneralReview(items);
        else
            PrepareRuleReview(items);
        Main.LogInfo("Review items count: " + items.Count);
        items.Sort((x, y) => x.time.CompareTo(y.time));

        var sb = new StringBuilder();

        foreach (ReviewItem item in items) sb.AppendLine(item.ToString());

        _review_text.text = sb.ToString();
    }

    private void PrepareRuleReview(List<ReviewItem> pItems)
    {
        pItems.Add(new ReviewItem(_selectedRule.start_time, LM.Get("review_rule_start"),
                                  kings[_selectedRule.aid].curr_name));

        // 其他信息, 不必按照时间顺序添加，会自动排序

        if (_selectedRule.end_time > 0)
            pItems.Add(new ReviewItem(_selectedRule.end_time, LM.Get("review_rule_end"),
                                      kings[_selectedRule.aid].curr_name,
                                      GeneralHelper.getYearsOn(_selectedRule.end_time -
                                                               _selectedRule.start_time)));
    }

    [Hotfixable]
    private void PrepareGeneralReview(List<ReviewItem> pItems)
    {
    }

    private struct ReviewItem
    {
        public readonly string text;
        public readonly double time;

        public ReviewItem(string pText, double pTime)
        {
            text = pText;
            time = pTime;
        }

        public ReviewItem(double pTime, string pTextFormat, params object[] pArgs)
        {
            text = string.Format(pTextFormat, pArgs);
            time = pTime;
        }

        public override string ToString()
        {
            return string.Format(LM.Get("review_item"), GeneralHelper.getDateOn(time), text);
        }
    }
}