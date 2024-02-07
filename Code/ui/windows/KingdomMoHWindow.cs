using System.Collections.Generic;
using System.Linq;
using Figurebox.constants;
using Figurebox.content;
using Figurebox.core;
using Figurebox.core.kingdom_policies;
using Figurebox.prefabs;
using Figurebox.ui.prefabs;
using Figurebox.utils.extensions;
using Figurebox.utils.MoH;
using NeoModLoader.api.attributes;
using NeoModLoader.General;
using NeoModLoader.General.UI.Window;
using NeoModLoader.General.UI.Window.Utils.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Figurebox;

public class KingdomMoHWindow : AutoLayoutWindow<KingdomMoHWindow>
{
    private Transform curr_state_grid;
    private ObjectPoolGenericMono<KingdomPolicyStateButton> curr_state_pool;
    private KingdomPolicyButton executing_policy;
    private UiUnitAvatarElement king_avatar;
    private Text kingdom_moh_desc;
    private Text kingdom_name_text;
    private Transform optional_policy_grid;
    private ObjectPoolGenericMono<KingdomPolicyButton> optional_policy_pool;
    private ObjectPoolGenericMono<KingdomPolicyButton> policy_queue_pool;

    private Transform policy_queue_transform;
    private static KingdomMoHWindow Instance { get; set; }

    protected override void Init()
    {
        GetLayoutGroup().spacing = 3;

        var top = this.BeginHoriGroup(new Vector2(200, 40), pSpacing: 5, pPadding: new RectOffset(3, 3, 0, 5));
        king_avatar = Instantiate(Main.backgroundAvatar.GetComponent<UiUnitAvatarElement>(), null);
        king_avatar.GetComponent<RectTransform>().sizeDelta = new Vector2(40, 40);
        king_avatar.show_banner_clan = false;
        king_avatar.show_banner_kingdom = true;
        king_avatar.tooltip_id = "actor_king";

        void click_king()
        {
            Config.selectedKingdom = king_avatar._actor.kingdom;
            ScrollWindow.moveAllToLeftAndRemove();
            ScrollWindow.showWindow("kingdom");
        }

        king_avatar.gameObject.AddComponent<Button>().onClick.AddListener(click_king);
        king_avatar.transform.Find("Mask").GetComponent<Button>().onClick.AddListener(click_king);

        king_avatar.GetComponent<TipButton>().enabled = true;
        top.AddChild(king_avatar.gameObject);

        var kingdom_text_group = top.BeginVertGroup(new Vector2(150, 40), pSpacing: 3);
        SimpleText kingdom_name_text_input = Instantiate(SimpleText.Prefab, null);
        kingdom_name_text_input.Setup("", TextAnchor.MiddleCenter, new Vector2(150, 18));
        kingdom_name_text_input.text.resizeTextMaxSize = 10;
        kingdom_name_text = kingdom_name_text_input.text;

        SimpleText kingdom_moh_desc_input = Instantiate(SimpleText.Prefab, null);
        kingdom_moh_desc_input.Setup("", TextAnchor.MiddleCenter, new Vector2(150, 18));
        kingdom_moh_desc_input.text.resizeTextMaxSize = 10;
        kingdom_moh_desc = kingdom_moh_desc_input.text;

        kingdom_text_group.AddChild(kingdom_name_text_input.gameObject);
        kingdom_text_group.AddChild(kingdom_moh_desc_input.gameObject);

        SimpleText curr_state_desc = Instantiate(SimpleText.Prefab, null);
        curr_state_desc.Setup("", TextAnchor.MiddleCenter, new Vector2(150, 11));
        curr_state_desc.background.enabled = false;
        var auto_localized_text = curr_state_desc.text.gameObject.AddComponent<LocalizedText>();
        auto_localized_text.key = "policy_state";
        auto_localized_text.autoField = true;
        auto_localized_text.updateText();
        LocalizedTextManager.addTextField(auto_localized_text);

        AddChild(curr_state_desc.gameObject);

        var curr_state_grid_group = this.BeginGridGroup(7, GridLayoutGroup.Constraint.FixedColumnCount,
            new Vector2(200, 50), new Vector2(24, 24), new Vector2(4, 2));
        curr_state_grid = curr_state_grid_group.transform;
        Image curr_state_grid_image = curr_state_grid_group.gameObject.AddComponent<Image>();
        curr_state_grid_image.sprite = SpriteTextureLoader.getSprite("ui/special/windowInnerSliced");
        curr_state_grid_image.type = Image.Type.Sliced;
        curr_state_grid_image.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        curr_state_grid_group.gameObject.AddComponent<ContentSizeFitter>().verticalFit =
            ContentSizeFitter.FitMode.PreferredSize;

        SimpleText policy_queue_desc = Instantiate(SimpleText.Prefab, null);
        policy_queue_desc.Setup("", TextAnchor.MiddleCenter, new Vector2(150, 11));
        policy_queue_desc.background.enabled = false;
        auto_localized_text = policy_queue_desc.text.gameObject.AddComponent<LocalizedText>();
        auto_localized_text.key = "policy_queue";
        auto_localized_text.autoField = true;
        auto_localized_text.updateText();
        LocalizedTextManager.addTextField(auto_localized_text);

        AddChild(policy_queue_desc.gameObject);

        var policy_queue_part =
            this.BeginHoriGroup(new Vector2(200, 24), pSpacing: 3, pPadding: new RectOffset(3, 3, 0, 0));
        var policy_queue =
            policy_queue_part.BeginHoriGroup(new Vector2(170, 24), pSpacing: 5, pPadding: new RectOffset(3, 3, 0, 0));
        policy_queue_transform = policy_queue.transform;
        Image policy_queue_image = policy_queue.gameObject.AddComponent<Image>();
        policy_queue_image.sprite = SpriteTextureLoader.getSprite("ui/special/windowInnerSliced");
        policy_queue_image.type = Image.Type.Sliced;
        policy_queue_image.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        executing_policy = Instantiate(KingdomPolicyButton.Prefab, null);
        executing_policy.Setup(null, null, true, true);
        executing_policy.SetSize(new Vector2(24, 24));
        policy_queue_part.AddChild(executing_policy.gameObject);


        SimpleText optional_policy_desc = Instantiate(SimpleText.Prefab, null);
        optional_policy_desc.Setup("", TextAnchor.MiddleCenter, new Vector2(150, 11));
        optional_policy_desc.background.enabled = false;
        auto_localized_text = optional_policy_desc.text.gameObject.AddComponent<LocalizedText>();
        auto_localized_text.key = "optional_policies";
        auto_localized_text.autoField = true;
        auto_localized_text.updateText();
        LocalizedTextManager.addTextField(auto_localized_text);
        AddChild(optional_policy_desc.gameObject);

        var optional_policy_grid_group = this.BeginGridGroup(5, GridLayoutGroup.Constraint.FixedColumnCount,
            new Vector2(200, 50), new Vector2(24, 24), new Vector2(4, 2));
        optional_policy_grid = optional_policy_grid_group.transform;
        Image optional_policy_grid_image = optional_policy_grid_group.gameObject.AddComponent<Image>();
        optional_policy_grid_image.sprite = SpriteTextureLoader.getSprite("ui/special/windowInnerSliced");
        optional_policy_grid_image.type = Image.Type.Sliced;
        optional_policy_grid_image.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);

        policy_queue_pool =
            new ObjectPoolGenericMono<KingdomPolicyButton>(KingdomPolicyButton.Prefab, policy_queue_transform);
        optional_policy_pool =
            new ObjectPoolGenericMono<KingdomPolicyButton>(KingdomPolicyButton.Prefab, optional_policy_grid);
        curr_state_pool =
            new ObjectPoolGenericMono<KingdomPolicyStateButton>(KingdomPolicyStateButton.Prefab, curr_state_grid);
    }

    [Hotfixable]
    internal static void InitAndShow()
    {
        if (Instance == null)
        {
            Instance = CreateWindow(nameof(KingdomMoHWindow), nameof(KingdomMoHWindow) + " Title");
        }

        if (!MoHTools.ExistMoHKingdom)
        {
            WorldTip.showNowTop(LM.Get("NoMoHKingdom"));
            return;
        }

        ScrollWindow.showWindow(nameof(KingdomMoHWindow));
    }

    [Hotfixable]
    public override void OnNormalEnable()
    {
        base.OnNormalEnable();
        if (!MoHTools.ExistMoHKingdom)
        {
            return;
        }

        AW_Kingdom moh_kingdom = MoHTools.MoHKingdom;

        kingdom_name_text.color = Toolbox.makeColor(moh_kingdom.kingdomColor.color_text);
        kingdom_name_text.text = moh_kingdom.name;
        if (!string.IsNullOrEmpty(moh_kingdom.addition_data.year_name))
        {
            int year_number = World.world.mapStats.getYearsSince(moh_kingdom.addition_data.year_start_timestamp) + 1;
            if (year_number == 1)
            {
                kingdom_name_text.text += "|" + LM.Get("year_name_format")
                    .Replace("$year_name$", moh_kingdom.addition_data.year_name)
                    .Replace("$year_number$", LM.Get("first_year_number"));
            }
            else
            {
                kingdom_name_text.text += "|" + LM.Get("year_name_format")
                    .Replace("$year_name$", moh_kingdom.addition_data.year_name)
                    .Replace("$year_number$", year_number.ToString());
            }
        }

        kingdom_moh_desc.text = LM.Get(MoHTools.GetMoHDescKey());
        if (DebugConst.IS_DEVELOPER)
        {
            kingdom_moh_desc.text += $"({MoHTools.MOH_Value})";
        }

        king_avatar.show(moh_kingdom.king);

        foreach (string key in moh_kingdom.addition_data.current_states.Keys)
        {
            KingdomPolicyStateButton state_button = curr_state_pool.getNext(0);
            state_button.Setup(KingdomPolicyStateLibrary.Instance.get(key), null, false, true);
            state_button.SetSize(new Vector2(24, 24));
        }

        int queue_idx = 0;
        foreach (var queue in moh_kingdom.addition_data.policy_queue)
        {
            KingdomPolicyButton policy_button = policy_queue_pool.getNext(queue_idx++);
            policy_button.Setup(KingdomPolicyLibrary.Instance.get(queue.policy_id),
                GetPolicyQueueButtonAction(moh_kingdom, policy_button), false, true);
            policy_button.SetSize(new Vector2(28, 28));
        }

        foreach (var policy in KingdomPolicyLibrary.Instance.list)
        {
            if (!moh_kingdom.CheckPolicy(policy)) continue;

            KingdomPolicyButton policy_button = optional_policy_pool.getNext(0);
            policy_button.Setup(policy, GetPolicySelectButtonAction(moh_kingdom, policy_button), false, true);
            policy_button.SetSize(new Vector2(28, 28));
        }

        executing_policy.gameObject.SetActive(true);
        executing_policy.Setup(
            string.IsNullOrEmpty(moh_kingdom.addition_data.current_policy_id) ||
            moh_kingdom.addition_data.p_status == AW_KingdomDataAddition.PolicyStatus.Completed
                ? null
                : KingdomPolicyLibrary.Instance.get(moh_kingdom.addition_data.current_policy_id), null, false, true);
        executing_policy.SetSize(new Vector2(24, 24));
        executing_policy.tip_data.tip_description = moh_kingdom.addition_data.p_progress.ToString();
        executing_policy.tip_data.tip_description_2 = moh_kingdom.addition_data.p_status.ToString();
    }

    [Hotfixable]
    public override void OnNormalDisable()
    {
        base.OnNormalDisable();
        Clean();
        Main.LogInfo("OnNormalDisable");
    }

    private void Clean()
    {
        kingdom_name_text.text = "NONE";
        kingdom_moh_desc.text = "NONE";

        king_avatar.show(null);
        policy_queue_pool.clear();
        optional_policy_pool.clear();
    }

    private UnityAction<KingdomPolicyAsset> GetPolicySelectButtonAction(AW_Kingdom moh_kingdom,
        KingdomPolicyButton policy_button)
    {
        return pPolicyAsset =>
        {
            if (moh_kingdom.addition_data.policy_queue.Count >= PolicyConst.MAX_POLICY_NR_IN_QUEUE) return;
            if (!moh_kingdom.CheckPolicy(pPolicyAsset)) return;
            if (!pPolicyAsset.can_repeat &&
                moh_kingdom.addition_data.policy_queue.Any(x => x.policy_id == pPolicyAsset.id)) return;
            if (!MoHTools.CostMoH(10, true)) return;

            KingdomPolicyButton policy_button_in_queue =
                policy_queue_pool.getNext(moh_kingdom.addition_data.policy_queue.Count);
            policy_button_in_queue.Setup(pPolicyAsset, GetPolicyQueueButtonAction(moh_kingdom, policy_button_in_queue),
                false, true);
            policy_button_in_queue.SetSize(new Vector2(28, 28));

            PolicyDataInQueue policy_data_in_queue = PolicyDataInQueue.Pool.GetNext();
            policy_data_in_queue.policy_id = pPolicyAsset.id;
            policy_data_in_queue.progress = pPolicyAsset.cost_in_plan;
            moh_kingdom.addition_data.policy_queue.Enqueue(policy_data_in_queue);
            if (!pPolicyAsset.can_repeat)
            {
                optional_policy_pool.InactiveObj(policy_button);
            }
        };
    }

    private UnityAction<KingdomPolicyAsset> GetPolicyQueueButtonAction(AW_Kingdom moh_kingdom,
        KingdomPolicyButton policy_button_in_queue)
    {
        return pPolicyAsset =>
        {
            if (!MoHTools.ReturnMoH(10)) return;

            Queue<PolicyDataInQueue> tmp = new(moh_kingdom.addition_data.policy_queue);
            moh_kingdom.addition_data.policy_queue.Clear();
            while (tmp.Count > 0)
            {
                var policy = tmp.Dequeue();
                if (policy.policy_id != pPolicyAsset.id)
                {
                    moh_kingdom.addition_data.policy_queue.Enqueue(policy);
                }

                PolicyDataInQueue.Pool.Recycle(policy);
            }


            policy_queue_pool.InactiveObj(policy_button_in_queue);
            foreach (KingdomPolicyButton button in optional_policy_pool._elements_total)
            {
                if (button.gameObject.activeSelf && button.policy?.id == pPolicyAsset.id)
                {
                    return;
                }
            }

            KingdomPolicyButton policy_button = optional_policy_pool.getNext(0);
            policy_button.Setup(pPolicyAsset, GetPolicySelectButtonAction(moh_kingdom, policy_button), false, true);
            policy_button.SetSize(new Vector2(28, 28));
        };
    }
}