using System.Collections.Generic;
using Figurebox.core;
using Figurebox.Utils.KingdomPolicy;
using NeoModLoader.api;
using UnityEngine;
using UnityEngine.UI;

namespace Figurebox.ui.windows;

public class KingdomPolicyGraphWindow : AbstractWideWindow<KingdomPolicyGraphWindow>
{
    private AW_Kingdom _kingdom;

    private List<List<Asset>> _sorted_policy_states;

    protected override void Init()
    {
        Transform scroll_view = BackgroundTransform.Find("Scroll View");
        scroll_view.transform.localPosition = Vector3.zero;
        scroll_view.transform.localScale = Vector3.one;

        var bg = new GameObject("BG", typeof(Image)).GetComponent<Image>();
        bg.transform.SetParent(BackgroundTransform);
        bg.transform.SetAsFirstSibling();
        bg.transform.localPosition = Vector3.zero;
        bg.transform.localScale = Vector3.one;
        bg.sprite = SpriteTextureLoader.getSprite("ui/special/darkInputFieldEmpty");
        bg.type = Image.Type.Sliced;
        bg.GetComponent<RectTransform>().sizeDelta = new Vector2(570, 250);
        // 临时使用的入口
        NewUI.createBGWindowButton(
            GameObject.Find("Canvas Container Main/Canvas - Windows/windows/kingdom"),
            0,
            "iconWorldLog",
            "KingdomPolicyGraph",
            "Kingdom Policy Graph",
            "Shows a kingdom's policy graph",
            () => ShowWindow(Config.selectedKingdom)
        );
    }

    public static void ShowWindow(Kingdom pSelectedKingdom)
    {
        Instance._kingdom = (AW_Kingdom)pSelectedKingdom;
        ScrollWindow.showWindow(WindowId);
    }

    public override void OnNormalEnable()
    {
        if (_kingdom == null) _kingdom = (AW_Kingdom)Config.selectedKingdom;

        _sorted_policy_states =
            KingdomPolicyGraphTools.SortPoliciesWithStates(_kingdom.policy_data.policy_history);

        Main.LogInfo("Sorted Policy (States):");
        var i = 0;
        foreach (var list in _sorted_policy_states)
        {
            foreach (Asset asset in list) Main.LogInfo($"[{i}]: {asset.id}");

            i++;
        }
    }

    public override void OnNormalDisable()
    {
    }
}