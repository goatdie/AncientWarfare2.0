using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using Figurebox.core;
using Figurebox.core.table_items;
using Figurebox.ui.prefabs;
using NeoModLoader.api;
using NeoModLoader.api.attributes;
using NeoModLoader.General.UI.Prefabs;
using UnityEngine;
using UnityEngine.UI;

namespace Figurebox.ui.windows;

public partial class NewKingdomHistoryWindow : AbstractWideWindow<NewKingdomHistoryWindow>
{
    /// <summary>
    ///     临时的按照时间顺序排列的君主统治列表
    /// </summary>
    private readonly Queue<KingRuleTableItem> _tmp_ruleQueue = new();

    private readonly Dictionary<HistoryType, RectTransform> HistoryTabs = new();

    /// <summary>
    ///     所选国家所有君主
    /// </summary>
    private readonly Dictionary<string, ActorTableItem> kings = new();

    /// <summary>
    ///     按照时间顺序排列的君主统治列表
    /// </summary>
    private readonly Queue<KingRuleTableItem> ruleQueue = new();

    private HistoryType _historyType = HistoryType.Population;

    private KingdomTableItem _kingdom;

    private ObjectPoolGenericMono<KingRuleHistoryItem> _rule_historyItemPool;

    private string _selectedKing = "";
    private string _selectedKingdom = "";
    private KingRuleTableItem _selectedRule;
    private RectTransform HistorySelectContentTransform;
    private RectTransform KingSelectContentTransform;

    private void Update()
    {
        if (!Initialized || !IsOpened) return;
        if (_tmp_ruleQueue.Count > 0)
        {
            KingRuleTableItem rule = _tmp_ruleQueue.Dequeue();
            var rule_item = _rule_historyItemPool.getNext(0);
            rule_item.Setup(kings[rule.aid], rule, _kingdom);
        }
    }

    protected override void Init()
    {
        var king_select_view = BackgroundTransform.Find("Scroll View").gameObject;
        king_select_view.name = "King Select Scroll View";
        var rect_transform = king_select_view.GetComponent<RectTransform>();
        rect_transform.sizeDelta = new Vector2(108, 255);
        rect_transform.localPosition = new Vector3(-232, 0, 0);
        rect_transform.localScale = Vector3.one;
        var scroll_rect = king_select_view.GetComponent<ScrollRect>();
        scroll_rect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;
        scroll_rect.verticalScrollbar.GetComponent<RectTransform>().sizeDelta = new Vector2(10, 0);
        var scroll_area_bg = king_select_view.GetComponent<Image>();
        scroll_area_bg.sprite = SpriteTextureLoader.getSprite("ui/special/windowEmptyFrame");
        scroll_area_bg.type = Image.Type.Sliced;
        scroll_area_bg.color = Color.white;

        KingSelectContentTransform = king_select_view.transform.Find("Viewport/Content").GetComponent<RectTransform>();
        var vert_layout = KingSelectContentTransform.gameObject.AddComponent<VerticalLayoutGroup>();
        vert_layout.childControlHeight = false;
        vert_layout.childControlWidth = false;
        vert_layout.childForceExpandHeight = false;
        vert_layout.childForceExpandWidth = false;
        vert_layout.childAlignment = TextAnchor.UpperCenter;
        vert_layout.spacing = 4;
        vert_layout.padding = new RectOffset(0, 0, 12, 12);

        var fitter = KingSelectContentTransform.gameObject.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

        var history_select_view = Instantiate(king_select_view, BackgroundTransform);
        history_select_view.name = "History Select Scroll View";
        rect_transform = history_select_view.GetComponent<RectTransform>();
        rect_transform.sizeDelta = new Vector2(48, 255);
        rect_transform.localPosition = new Vector3(263, 0, 0);
        rect_transform.localScale = Vector3.one;

        HistorySelectContentTransform =
            history_select_view.transform.Find("Viewport/Content").GetComponent<RectTransform>();

        void CreateHistoryTab(HistoryType pType, string pIcon)
        {
            var tab_entry = Instantiate(SimpleButton.Prefab, HistorySelectContentTransform);
            tab_entry.name = pType.ToString();
            tab_entry.Setup(() =>
            {
                _historyType = pType;
                UpdatePage();
            }, SpriteTextureLoader.getSprite(pIcon), pTipType: "normal", pTipData: new TooltipData
            {
                tip_name = "AW_History " + pType,
                tip_description = "AW_History " + pType + " Description"
            });
            tab_entry.Background.enabled = false;

            GameObject tab = Instantiate(king_select_view, BackgroundTransform);
            tab.name = pType.ToString();
            tab.transform.SetParent(BackgroundTransform);
            tab.transform.localScale = Vector3.one;
            tab.transform.localPosition = new Vector3(30, 0, 0);
            tab.GetComponent<RectTransform>().sizeDelta = new Vector2(400, 255);
            HistoryTabs[pType] = tab.transform.Find("Viewport/Content") as RectTransform;
        }

        CreateHistoryTab(HistoryType.Population, "ui/icons/iconPopulation");
        CreateHistoryTab(HistoryType.Territory, "ui/icons/iconKingdomZones");
        CreateHistoryTab(HistoryType.War, "ui/icons/iconWarsList");
        CreateHistoryTab(HistoryType.Policy, "ui/icons/iconPlotsList");
        CreateHistoryTab(HistoryType.Review, "ui/icons/iconDocument");


        InitReviewTab();

        _rule_historyItemPool =
            new ObjectPoolGenericMono<KingRuleHistoryItem>(KingRuleHistoryItem.Prefab, KingSelectContentTransform);
    }

    public override void OnNormalEnable()
    {
        base.OnNormalEnable();

        kings.Clear();
        ruleQueue.Clear();
        _tmp_ruleQueue.Clear();

        _selectedKing = "";
        _selectedRule = null;

        RequestKings();

        ShowReviewTab();
    }

    public override void OnNormalDisable()
    {
        base.OnNormalDisable();

        kings.Clear();
        ruleQueue.Clear();
        _tmp_ruleQueue.Clear();

        _rule_historyItemPool.clear();
    }

    [Hotfixable]
    private void RequestKings()
    {
        using var cmd = new SQLiteCommand(EventsManager.Instance.OperatingDB);
        cmd.CommandText = "SELECT * FROM KingRule WHERE KID = @kingdom_id ORDER BY start_time ASC";
        cmd.Parameters.AddWithValue("@kingdom_id", _selectedKingdom);
        using (var king_rule_reader = cmd.ExecuteReader())
        {
            while (king_rule_reader.Read())
            {
                var king_rule = new KingRuleTableItem();
                king_rule.ReadFromReader(king_rule_reader);
                ruleQueue.Enqueue(king_rule);
                _tmp_ruleQueue.Enqueue(king_rule);
            }
        }

        cmd.CommandText = "SELECT * FROM Actor WHERE ID = @king_id";

        cmd.Parameters.Add("@king_id", DbType.String);

        foreach (var king_rule in ruleQueue.Where(king_rule => !kings.ContainsKey(king_rule.aid)))
        {
            cmd.Parameters["@king_id"].Value = king_rule.aid;

            using var king_reader = cmd.ExecuteReader();
            if (!king_reader.Read()) continue;

            var king = new ActorTableItem();
            king.ReadFromReader(king_reader);
            kings[king_rule.aid] = king;
        }

        cmd.CommandText = "SELECT * FROM Kingdom WHERE ID = @kingdom_id";
        using (var kingdom_reader = cmd.ExecuteReader())
        {
            if (kingdom_reader.Read())
            {
                _kingdom = new KingdomTableItem();
                _kingdom.ReadFromReader(kingdom_reader);
            }
        }
    }


    public void SelectKingRule(KingRuleTableItem pRule)
    {
        _selectedKing = pRule.aid;
        foreach (var rule_item in _rule_historyItemPool._elements_total) rule_item.MarkSelected(false);
        _selectedRule = pRule;
        _selectedKing = pRule.aid;
        UpdatePage();
    }

    public void UpdatePage()
    {
        foreach (var tab in HistoryTabs) tab.Value.gameObject.SetActive(tab.Key == _historyType);
        switch (_historyType)
        {
            case HistoryType.Population:
                break;
            case HistoryType.Territory:
                break;
            case HistoryType.War:
                break;
            case HistoryType.Policy:
                break;
            case HistoryType.Review:
                ShowReviewTab();
                break;
        }
    }

    public static void ShowWindow(string pKingdomID)
    {
        Instance._selectedKingdom = pKingdomID;
        Instance._selectedKing = "";
        Instance._historyType = HistoryType.Population;

        ScrollWindow.showWindow(WindowId);
    }

    private enum HistoryType
    {
        Population,
        Territory,
        War,
        Policy,
        Review
    }
}