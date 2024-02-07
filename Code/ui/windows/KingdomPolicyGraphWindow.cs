using System.Collections.Generic;
using System.Linq;
using Figurebox.core;
using Figurebox.prefabs;
using Figurebox.ui.prefabs;
using Figurebox.utils.KingdomPolicy;
using NeoModLoader.api;
using UnityEngine;
using UnityEngine.UI;

namespace Figurebox.ui.windows;

public class KingdomPolicyGraphWindow : AbstractWideWindow<KingdomPolicyGraphWindow>
{
    private AW_Kingdom _kingdom;

    private ObjectPoolGenericMono<KingdomPolicyButton> _policy_button_pool;

    private List<List<KingdomPolicyGraphNode>>              _sorted_policy_states;
    private ObjectPoolGenericMono<KingdomPolicyStateButton> _state_button_pool;

    protected override void Init()
    {
        Transform scroll_view = BackgroundTransform.Find("Scroll View");
        scroll_view.transform.localPosition = Vector3.zero;
        scroll_view.transform.localScale = Vector3.one;

        var viewport = scroll_view.Find("Viewport").GetComponent<RectTransform>();
        viewport.pivot = new Vector2(0.5f,    0.5f);
        viewport.sizeDelta = new Vector2(310, 33);
        viewport.localPosition = Vector3.zero;

        var bg = new GameObject("BG", typeof(Image)).GetComponent<Image>();
        bg.transform.SetParent(BackgroundTransform);
        bg.transform.SetAsFirstSibling();
        bg.transform.localPosition = Vector3.zero;
        bg.transform.localScale = Vector3.one;
        bg.sprite = SpriteTextureLoader.getSprite("ui/special/darkInputFieldEmpty");
        bg.type = Image.Type.Sliced;
        bg.GetComponent<RectTransform>().sizeDelta = new Vector2(570, 250);

        _policy_button_pool =
            new ObjectPoolGenericMono<KingdomPolicyButton>(KingdomPolicyButton.Prefab, ContentTransform);
        _state_button_pool =
            new ObjectPoolGenericMono<KingdomPolicyStateButton>(KingdomPolicyStateButton.Prefab, ContentTransform);
    }

    public static void ShowWindow(Kingdom pSelectedKingdom)
    {
        Instance._kingdom = (AW_Kingdom)pSelectedKingdom;
        ScrollWindow.showWindow(WindowId);
    }

    public override void OnNormalEnable()
    {
        _kingdom ??= (AW_Kingdom)Config.selectedKingdom;

        _sorted_policy_states =
            KingdomPolicyGraphTools.SortPoliciesWithStates(_kingdom.addition_data.policy_history);

        KingdomPolicyGraphTools.CalcNodePositions(_sorted_policy_states);
        Main.LogInfo("Sorted Policy (States):");
        var i = 0;
        foreach (var list in _sorted_policy_states)
        {
            foreach (KingdomPolicyGraphNode node in list)
                Main.LogInfo($"[{i}]: {node.asset.id}({node.position.x}, {node.position.y})");

            i++;
        }

        _policy_button_pool.clear();
        _state_button_pool.clear();
        foreach (KingdomPolicyGraphNode node in _sorted_policy_states.SelectMany(list => list))
        {
            Transform transform;
            if (node.is_state)
            {
                KingdomPolicyStateButton state_button = _state_button_pool.getNext();
                state_button.Setup(node.as_state, null);
                state_button.SetSize(new Vector2(32, 32));
                transform = state_button.transform;
            }
            else
            {
                KingdomPolicyButton policy_button = _policy_button_pool.getNext();
                policy_button.Setup(node.as_policy);
                policy_button.SetSize(new Vector2(32, 32));
                transform = policy_button.transform;
            }

            transform.localPosition = node.position;
            transform.localScale = Vector3.one;
        }
        DrawArrowLines();
    }

    private void DrawArrowLines()
    {
        foreach (var node in _sorted_policy_states.SelectMany(x=>x))
        {
            foreach (var next_node in node.next)
            {
                DrawArrowLine(node, next_node);
            }
        }
    }

    private void DrawArrowLine(KingdomPolicyGraphNode node, KingdomPolicyGraphNode next_node)
    {
        Main.LogInfo($"Draw arrow {node.position} -> {next_node.position}");
    }

    public override void OnNormalDisable()
    {
    }
}