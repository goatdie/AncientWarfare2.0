using System;
using Figurebox.abstracts;
using Figurebox.core;
using Figurebox.core.kingdom_policies;
using Figurebox.core.table_items;
using Figurebox.Utils;
using NeoModLoader.api.attributes;
using NeoModLoader.General;
using UnityEngine.UI;

namespace Figurebox.content;

internal class TooltipLibrary : ExtendedLibrary<TooltipAsset>
{
    protected override void init()
    {
        add(new TooltipAsset
        {
            id = "kingdom_policy",
            prefab_id = "tooltips/tooltip_policy",
            callback = showPolicy
        });
        add(new TooltipAsset
        {
            id = "kingdom_policy_state",
            prefab_id = "tooltips/tooltip_policy_state",
            callback = showPolicyState
        });
        add(new TooltipAsset
        {
            id = "actor_heir",
            prefab_id = "tooltips/tooltip_actor",
            callback = showHeir
        });
        add(new TooltipAsset
        {
            id = "history_king",
            callback = showHistoryKing
        });
        var resource_tooltip = AssetManager.tooltips.get(pID: "resource");
        resource_tooltip.callback = (TooltipShowAction)Delegate.Combine(
            a: resource_tooltip.callback,
            b: new TooltipShowAction(showTax));
    }

    private static void showHeir(Tooltip pTooltip, string pType, TooltipData pData)
    {
        AssetManager.tooltips.showActor("village_statistics_heir", pTooltip, pData);
        Image component = pTooltip.transform.FindRecursive("IconSpecial").GetComponent<Image>();
        component.sprite = SpriteTextureLoader.getSprite("ui/icons/iconCrown");
        component.gameObject.SetActive(true);
    }

    /// <summary>
    ///     显示政策
    /// </summary>
    /// <remarks>
    ///     对<paramref name="pData" />的参数做如下约定
    ///     <list type="table">
    ///         <item>
    ///             <term>参数</term><description>说明</description>
    ///         </item>
    ///         <item>
    ///             <term>tip_name</term><description>国策Id</description>
    ///         </item>
    ///         <item>
    ///             <term>tip_description</term><description>当前进度</description>
    ///         </item>
    ///         <item>
    ///             <term>tip_description_2</term><description>当前阶段</description>
    ///         </item>
    ///     </list>
    /// </remarks>
    /// <param name="pTooltip"></param>
    /// <param name="pType"></param>
    /// <param name="pData"></param>
    [Hotfixable]
    private static void showPolicy(Tooltip pTooltip, string pType, TooltipData pData = default)
    {
        if (string.IsNullOrEmpty(pData.tip_name)) return;
        KingdomPolicyAsset policy = KingdomPolicyLibrary.Instance.get(pData.tip_name);

        pTooltip.name.text = LM.Get(policy.policyname);
        pTooltip.setDescription(LM.Get(policy.description));
        if (!string.IsNullOrEmpty(pData.tip_description))
        {
            int total = pData.tip_description_2 == AW_KingdomDataAddition.PolicyStatus.InPlanning.ToString()
                ? policy.cost_in_plan
                : policy.cost_in_progress;
            pTooltip.addBottomDescription(
                $"{LM.Get(pData.tip_description_2)}: {LM.Get(pData.tip_description)}/{total}");
        }
    }

    /// <summary>
    ///     显示政治状态
    /// </summary>
    /// <remarks>
    ///     对<paramref name="pData" />的参数做如下约定
    ///     <list type="table">
    ///         <item>
    ///             <term>参数</term><description>说明</description>
    ///         </item>
    ///         <item>
    ///             <term>tip_name</term><description>政治状态Id</description>
    ///         </item>
    ///     </list>
    /// </remarks>
    /// <param name="pTooltip"></param>
    /// <param name="pType"></param>
    /// <param name="pData"></param>
    [Hotfixable]
    private static void showPolicyState(Tooltip pTooltip, string pType, TooltipData pData = default)
    {
        if (string.IsNullOrEmpty(pData.tip_name)) return;
        KingdomPolicyStateAsset state = KingdomPolicyStateLibrary.Instance.get(pData.tip_name);
        pTooltip.name.text = LM.Get(state.name);
        if (!string.IsNullOrEmpty(state.description) && LocalizedTextManager.stringExists(state.description))
            pTooltip.setDescription(LM.Get(state.description));
    }

    /// <summary>
    ///     显示历史君主
    /// </summary>
    /// <remarks>
    ///     对<paramref name="pData" />的参数做如下约定
    ///     <list type="table">
    ///         <item>
    ///             <term>参数</term><description>说明</description>
    ///         </item>
    ///         <item>
    ///             <term>tip_name</term><description><see cref="ActorTableItem" />JSON字符串</description>
    ///         </item>
    ///         <item>
    ///             <term>tip_description</term><description><see cref="KingRuleTableItem" />JSON字符串</description>
    ///         </item>
    ///         <item>
    ///             <term>tip_description_2</term><description><see cref="KingdomTableItem" />JSON字符串</description>
    ///         </item>
    ///     </list>
    /// </remarks>
    /// <param name="pTooltip"></param>
    /// <param name="pType"></param>
    /// <param name="pData"></param>
    private static void showHistoryKing(Tooltip pTooltip, string pType, TooltipData pData = default)
    {
        var king = GeneralHelper.FromJSON<ActorTableItem>(pData.tip_name);
        var rule = GeneralHelper.FromJSON<KingRuleTableItem>(pData.tip_description);
        var kingdom = GeneralHelper.FromJSON<KingdomTableItem>(pData.tip_description_2);

        pTooltip.name.text = king.curr_name;
        pTooltip.addDescription(LM.Get("rule_time")
            .Replace("$start_time$", GeneralHelper.getYearsOn(rule.start_time).ToString())
            .Replace("$end_time$",
                rule.end_time < 0 ? LM.Get("rule_time_now") : GeneralHelper.getYearsOn(rule.end_time).ToString()));

        pTooltip.addStatValues("其他信息", "其他信息值");
    }

    [Hotfixable]
    private static void showTax(Tooltip pTooltip, string pType, TooltipData pData = default)
    {


        AW_City city = Config.selectedCity as AW_City;

        // 添加税收相关的信息
        ResourceAsset resource = pData.resource;
        pTooltip.name.text = LocalizedTextManager.getText(resource.id, null);
        if (resource.id == SR.gold)
        {
            pTooltip.addLineBreak("----");
            if (!city.isCapitalCity() && city.gold_pay_tax != 0)
            {
                pTooltip.addItemText("pay_tax", (float)city.gold_pay_tax, false, true, true, "#FB2C21", false);
            }

            if (city.isCapitalCity() && city.GetTaxToltal() != 0)
            {
                pTooltip.addItemText("tax_income", (float)city.GetTaxToltal(), false, true, true, "#43FF43", false);

            }

        }

    }
}