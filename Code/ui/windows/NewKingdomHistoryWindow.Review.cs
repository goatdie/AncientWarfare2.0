using System.Collections.Generic;
using System.Text;
using Figurebox.Utils;
using NeoModLoader.api.attributes;
using NeoModLoader.General;
using UnityEngine;
using UnityEngine.UI;
using Figurebox.core.table_items;
using System.Data.SQLite;
using Figurebox.core;
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
    private List<KingdomChangeYearTableItem> _changeyears = new();

    private void RequestChangeYearForCurrentRule()
    {
        using var cmd = new SQLiteCommand(EventsManager.Instance.OperatingDB);
        cmd.CommandText = "SELECT * FROM KingdomChangeYear WHERE id=@king_id";
        cmd.Parameters.AddWithValue("@king_id", _selectedRule.aid);

        using var result_reader = cmd.ExecuteReader();
        _changeyears.Clear();
        while (result_reader.Read())
        {
            var change_year_item = new KingdomChangeYearTableItem();
            change_year_item.ReadFromReader(result_reader);
            _changeyears.Add(change_year_item);
        }
    }
    private List<MOHTableItem> _moh = new();
    public void RequestMOH()
    {

        using (var cmd = new SQLiteCommand(EventsManager.Instance.OperatingDB))
        {
            cmd.CommandText = "SELECT * FROM MOH WHERE aid=@king_id";
            cmd.Parameters.AddWithValue("@king_id", _selectedRule.aid);
            using var result_reader = cmd.ExecuteReader();
            _moh.Clear();
            while (result_reader.Read())
            {
                var mohItem = new MOHTableItem();
                mohItem.ReadFromReader(result_reader);
                _moh.Add(mohItem);
            }
        }


    }
    private UsurpationTableItem _usurpation = new();
    public void RequestUsurpation()
    {
        using (var cmd = new SQLiteCommand(EventsManager.Instance.OperatingDB))
        {

            _usurpation = new UsurpationTableItem();

            cmd.CommandText = "SELECT * FROM USURPATION WHERE aid=@king_id";
            cmd.Parameters.AddWithValue("@king_id", _selectedRule.aid);
            using var result_reader = cmd.ExecuteReader();
            if (result_reader.Read())
            {
                _usurpation.ReadFromReader(result_reader);
            }
        }
    }
    private List<IntegrationTableItem> _integration = new();
    public void RequestIntegration()
    {
        using (var cmd = new SQLiteCommand(EventsManager.Instance.OperatingDB))
        {

            cmd.CommandText = "SELECT * FROM INTEGRATION WHERE aid=@king_id";
            cmd.Parameters.AddWithValue("@king_id", _selectedRule.aid);
            using var result_reader = cmd.ExecuteReader();
            _integration.Clear();
            while (result_reader.Read())
            {
                var IntegrationItem = new IntegrationTableItem();
                IntegrationItem.ReadFromReader(result_reader);
                _integration.Add(IntegrationItem);
            }
        }
    }
    private List<KingdomCapitalChangeTableItem> _capitalchange = new();
    public void RequestCapitalChange()
    {

        using (var cmd = new SQLiteCommand(EventsManager.Instance.OperatingDB))
        {
            cmd.CommandText = "SELECT * FROM CAPITALCHANGE WHERE aid=@king_id";
            cmd.Parameters.AddWithValue("@king_id", _selectedRule.aid);
            using var result_reader = cmd.ExecuteReader();
            _capitalchange.Clear();
            while (result_reader.Read())
            {
                var CapitalItem = new KingdomCapitalChangeTableItem();
                CapitalItem.ReadFromReader(result_reader);
                _capitalchange.Add(CapitalItem);
            }
        }


    }
    private List<VassalTableItem> _vassal = new();
    private List<VassalTableItem> _lord = new();

    public void RequestVassal()
    {

        using (var cmd = new SQLiteCommand(EventsManager.Instance.OperatingDB))
        {
            cmd.CommandText = "SELECT * FROM VASSAL WHERE vaid=@king_id";
            cmd.Parameters.AddWithValue("@king_id", _selectedRule.aid);
            using var result_reader = cmd.ExecuteReader();
            _vassal.Clear();
            while (result_reader.Read())
            {
                var Vassal = new VassalTableItem();

                ReadFromReader(result_reader, Vassal);
                _vassal.Add(Vassal);
            }
        }


    }
    public void RequestLord()
    {

        using (var cmd = new SQLiteCommand(EventsManager.Instance.OperatingDB))
        {
            cmd.CommandText = "SELECT * FROM VASSAL WHERE said=@king_id";
            cmd.Parameters.AddWithValue("@king_id", _selectedRule.aid);
            using var result_reader = cmd.ExecuteReader();
            _lord.Clear();
            while (result_reader.Read())
            {
                var Vassal = new VassalTableItem();


                ReadFromReader(result_reader, Vassal);
                // Vassal.absorb = result_reader.GetBoolean(result_reader.GetOrdinal("ABSORB"));
                _lord.Add(Vassal);
            }
        }


    }
    public void ReadFromReader(SQLiteDataReader reader, VassalTableItem bti)
    {


        bti.vaid = reader.GetString(reader.GetOrdinal("VAID"));
        bti.said = reader.GetString(reader.GetOrdinal("SAID"));
        bti.kid = reader.GetString(reader.GetOrdinal("KID"));
        bti.Skid = reader.GetString(reader.GetOrdinal("SKID"));
        bti.vassal_name = reader.GetString(reader.GetOrdinal("VASSAL_NAME"));
        bti.lord_name = reader.GetString(reader.GetOrdinal("LORD_NAME"));


        bti.start_time = reader.GetDouble(reader.GetOrdinal("START_TIME"));
        bti.end_time = reader.GetDouble(reader.GetOrdinal("END_TIME"));


        bti.absorb = reader.GetInt64(reader.GetOrdinal("ABSORB")) != 0;



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
        RequestChangeYearForCurrentRule();
        foreach (var change_year_event in _changeyears)
        {
            pItems.Add(new ReviewItem(change_year_event.timestamp, LM.Get("review_change_yearname"), change_year_event.new_name));
        }
        RequestMOH();
        foreach (var moh_event in _moh)
        {
            if (moh_event.start_time > 0)
                pItems.Add(new ReviewItem(moh_event.start_time, LM.Get("review_start_moh"), moh_event.kingdom_name));
            if (moh_event.end_time > 0)
                pItems.Add(new ReviewItem(moh_event.end_time, LM.Get("review_end_moh"), moh_event.kingdom_name));
        }
        RequestUsurpation();
        if (_usurpation.timestamp > 0)
            pItems.Add(new ReviewItem(_usurpation.timestamp, LM.Get("review_start_usurpation"), kings[_usurpation.aid].curr_name, _usurpation.kingdom_name));
        RequestIntegration();
        foreach (var integration_event in _integration)
        {
            pItems.Add(new ReviewItem(integration_event.timestamp, LM.Get("review_start_integration"), kings[integration_event.aid].curr_name, integration_event.old_kingdom_name, integration_event.new_kingdom_name));
        }
        RequestCapitalChange();
        foreach (var capital_event in _capitalchange)
        {
            pItems.Add(new ReviewItem(capital_event.timestamp, LM.Get("review_capitalchange"), capital_event.old_capital_name, capital_event.new_capital_name));
        }
        RequestVassal();
        foreach (var vassal_event in _vassal)
        {
            if (vassal_event.absorb && vassal_event.end_time > 0)
            {
                pItems.Add(new ReviewItem(vassal_event.end_time, LM.Get("review_absorbedvassal"), vassal_event.vassal_name, vassal_event.lord_name));
            }
            else
            {
                if (vassal_event.start_time > 0)
                    pItems.Add(new ReviewItem(vassal_event.start_time, LM.Get("review_setvassal"), vassal_event.vassal_name, vassal_event.lord_name));
                if (vassal_event.end_time > 0)
                    pItems.Add(new ReviewItem(vassal_event.end_time, LM.Get("review_removevassal"), vassal_event.vassal_name, vassal_event.lord_name));
            }

        }
        RequestLord();
        foreach (var vassal_event in _lord)
        {
            if (vassal_event.absorb && vassal_event.end_time > 0)
            {
                pItems.Add(new ReviewItem(vassal_event.end_time, LM.Get("review_absorbvassal"), vassal_event.vassal_name, vassal_event.lord_name));
            }
            else
            {
                if (vassal_event.start_time > 0)
                    pItems.Add(new ReviewItem(vassal_event.start_time, LM.Get("review_getvassal"), vassal_event.vassal_name, vassal_event.lord_name));
                if (vassal_event.end_time > 0)
                    pItems.Add(new ReviewItem(vassal_event.end_time, LM.Get("review_losevassal"), vassal_event.vassal_name, vassal_event.lord_name));
            }
        }
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