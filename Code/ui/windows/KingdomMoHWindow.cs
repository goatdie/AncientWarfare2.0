using Figurebox.core;
using Figurebox.prefabs;
using Figurebox.Utils.MoH;
using NeoModLoader.api.attributes;
using NeoModLoader.General;
using NeoModLoader.General.UI.Window;
using NeoModLoader.General.UI.Window.Utils.Extensions;
using UnityEngine;
using UnityEngine.UI;
namespace Figurebox;

public class KingdomMoHWindow : AutoLayoutWindow<KingdomMoHWindow>
{
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
        king_avatar.GetComponent<RectTransform>().sizeDelta = new Vector2(36, 36);
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

        SimpleText policy_queue_desc = Instantiate(SimpleText.Prefab, null);
        policy_queue_desc.Setup("", TextAnchor.MiddleCenter, new Vector2(150, 11));
        policy_queue_desc.background.enabled = false;
        var auto_localized_text = policy_queue_desc.text.gameObject.AddComponent<LocalizedText>();
        auto_localized_text.key = "policy_queue";
        auto_localized_text.autoField = true;
        auto_localized_text.updateText();
        LocalizedTextManager.addTextField(auto_localized_text);

        AddChild(policy_queue_desc.gameObject);

        var policy_queue = this.BeginHoriGroup(new Vector2(200, 24), pSpacing: 5, pPadding: new RectOffset(3, 3, 0, 0));
        policy_queue_transform = policy_queue.transform;
        Image policy_queue_image = policy_queue.gameObject.AddComponent<Image>();
        policy_queue_image.sprite = SpriteTextureLoader.getSprite("ui/special/windowInnerSliced");
        policy_queue_image.type = Image.Type.Sliced;
        policy_queue_image.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);


        SimpleText optional_policy_desc = Instantiate(SimpleText.Prefab, null);
        optional_policy_desc.Setup("", TextAnchor.MiddleCenter, new Vector2(150, 11));
        optional_policy_desc.background.enabled = false;
        auto_localized_text = optional_policy_desc.text.gameObject.AddComponent<LocalizedText>();
        auto_localized_text.key = "optional_policies";
        auto_localized_text.autoField = true;
        auto_localized_text.updateText();
        LocalizedTextManager.addTextField(auto_localized_text);
        AddChild(optional_policy_desc.gameObject);

        var optional_policy_grid_group = this.BeginGridGroup(5, GridLayoutGroup.Constraint.FixedColumnCount, new Vector2(200, 50), new Vector2(24, 24), new Vector2(4, 2));
        optional_policy_grid = optional_policy_grid_group.transform;
        Image optional_policy_grid_image = optional_policy_grid_group.gameObject.AddComponent<Image>();
        optional_policy_grid_image.sprite = SpriteTextureLoader.getSprite("ui/special/windowInnerSliced");
        optional_policy_grid_image.type = Image.Type.Sliced;
        optional_policy_grid_image.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);

        policy_queue_pool = new ObjectPoolGenericMono<KingdomPolicyButton>(KingdomPolicyButton.Prefab, policy_queue_transform);
        optional_policy_pool = new ObjectPoolGenericMono<KingdomPolicyButton>(KingdomPolicyButton.Prefab, optional_policy_grid);
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

        kingdom_name_text.text = moh_kingdom.name;
        if (!string.IsNullOrEmpty(moh_kingdom.policy_data.year_name))
        {
            int year_number = World.world.mapStats.getYearsSince(moh_kingdom.policy_data.year_start_timestamp) + 1;
            if (year_number == 1)
            {
                kingdom_name_text.text += "|" + LM.Get("year_name_format")
                    .Replace("$year_name$", moh_kingdom.policy_data.year_name)
                    .Replace("$year_number$", LM.Get("first_year_number"));
            }
            else
            {
                kingdom_name_text.text += "|" + LM.Get("year_name_format")
                    .Replace("$year_name$", moh_kingdom.policy_data.year_name)
                    .Replace("$year_number$", year_number.ToString());
            }
        }
        kingdom_moh_desc.text = LM.Get(MoHTools.GetMoHDescKey());

        king_avatar.show(moh_kingdom.king);

        int queue_idx = 0;
        foreach (var queue in moh_kingdom.policy_data.policy_queue)
        {
            KingdomPolicyButton policy_button = policy_queue_pool.getNext(queue_idx++);
            policy_button.Setup(KingdomPolicyLibrary.Instance.get(queue.policy_id), null, false, true);
            policy_button.SetSize(new Vector2(28, 28));
        }

        foreach (var policy in KingdomPolicyLibrary.Instance.list)
        {
            if (!moh_kingdom.CheckPolicy(policy)) continue;

            KingdomPolicyButton policy_button = optional_policy_pool.getNext();
            policy_button.Setup(policy, pPolicy =>
            {

            }, false, true);
            policy_button.SetSize(new Vector2(28, 28));
        }
    }
    public override void OnNormalDisable()
    {
        base.OnNormalDisable();
        Clean();
    }
    private void Clean()
    {
        kingdom_name_text.text = "NONE";
        kingdom_moh_desc.text = "NONE";

        king_avatar.show(null);
        policy_queue_pool.clear();
        optional_policy_pool.clear();
    }
}