using NeoModLoader.api.attributes;
using NeoModLoader.General;
using UnityEngine.UI;

namespace Figurebox;

internal class Tooltips
{
    public static void init()
    {
        AssetManager.tooltips.add(new TooltipAsset
        {
            id = "kingdom_policy",
            prefab_id = "tooltips/tooltip_policy",
            callback = showPolicy
        });
        AssetManager.tooltips.add(new TooltipAsset
        {
            id = "kingdom_policy_state",
            prefab_id = "tooltips/tooltip_policy_state",
            callback = showPolicyState
        });
        AssetManager.tooltips.add(new TooltipAsset
        {
            id = "actor_heir",
            prefab_id = "tooltips/tooltip_actor",
            callback = showHeir
        });
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
            int total = pData.tip_description_2 == KingdomPolicyData.PolicyStatus.InPlanning.ToString()
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
        pTooltip.setDescription(LM.Get(state.description));
    }
}