using Figurebox.constants;
using Figurebox.core;
using Figurebox.prefabs;
using Figurebox.ui.prefabs;
using NeoModLoader.General.UI.Window.Layout;
using NeoModLoader.General.UI.Window.Utils.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Figurebox.ui.windows;

internal class KingdomWindowAdditionComponent : AutoVertLayoutGroup
{
    private AutoVertLayoutGroup      AutoLayoutRoot;
    private RectTransform            BackgroundTransform;
    private RectTransform            ContentTransform;
    private KingdomPolicyStateButton current_state;
    private KingdomPolicyButton      executing_policy;
    private UiUnitAvatarElement      heir_avatar;
    private UiUnitAvatarElement      king_avatar;
    private KingdomWindow            Window;
    private SimpleText               year_name;
    private AW_Kingdom               kingdom => (AW_Kingdom)Config.selectedKingdom;

    private void OnEnable()
    {
        if (!Initialized) return;
        heir_avatar.show(kingdom.heir);
        heir_avatar.unit_type_bg.sprite = heir_avatar.type_leader;
        actorAvatarDisplaySetting(heir_avatar, heir_avatar.gameObject.activeSelf);
        actorAvatarDisplaySetting(king_avatar, king_avatar.gameObject.activeSelf);

        void actorAvatarDisplaySetting(UiUnitAvatarElement avatar, bool pActive)
        {
            avatar.transform.Find("Mask").gameObject.SetActive(pActive);
            avatar.GetComponent<Button>().enabled = pActive;
            avatar.GetComponentInChildren<BannerLoader>()?.gameObject.SetActive(pActive);
            avatar.GetComponentInChildren<BannerLoaderClans>()?.gameObject.SetActive(pActive);

            if (avatar.GetComponent<EventTrigger>() == null)
            {
                avatar.gameObject.AddComponent<EventTrigger>();
            }

            avatar.GetComponent<EventTrigger>().enabled = pActive;
            if (!pActive)
            {
                avatar.gameObject.SetActive(true);
            }
        }

        year_name.text.text = kingdom.GetYearName(true);
        year_name.text.color = kingdom.kingdomColor.getColorText();


        executing_policy.gameObject.SetActive(true);
        executing_policy.Setup(
            string.IsNullOrEmpty(kingdom.policy_data.current_policy_id) ||
            kingdom.policy_data.p_status == KingdomPolicyData.PolicyStatus.Completed
                ? null
                : KingdomPolicyLibrary.Instance.get(kingdom.policy_data.current_policy_id));
        executing_policy.SetSize(new Vector2(16, 16));
        RectTransform bgRect = executing_policy.bg.GetComponent<RectTransform>();
        bgRect.sizeDelta = new Vector2(60, 16);
        executing_policy.tip_data.tip_description = kingdom.policy_data.p_progress.ToString();
        executing_policy.tip_data.tip_description_2 = kingdom.policy_data.p_status.ToString();

        current_state.gameObject.SetActive(true);
        current_state.Setup(
            KingdomPolicyStateLibrary.Instance.get(
                kingdom.policy_data.GetPolicyStateId(PolicyStateType.social_level)) ??
            KingdomPolicyStateLibrary.DefaultState, null);
        current_state.SetSize(new Vector2(16, 16));
        bgRect = current_state.bg.GetComponent<RectTransform>();
        bgRect.sizeDelta = new Vector2(60, 16);


        Window.elements.Clear();
    }

    private void OnDisable()
    {
        executing_policy.tip_data.tip_name = "";
        if (string.IsNullOrEmpty(executing_policy.tip_data.tip_name))
        {
            executing_policy.tip_button.enabled = false;
        }

        // 清除描述

        Window.elements.Clear();
        int count = Window.cityInfoPlacement.childCount;
        for (int i = 0; i < count; i++)
        {
            Destroy(Window.cityInfoPlacement.GetChild(i).gameObject, 1);
        }
    }

    protected override void Init()
    {
        ContentTransform = transform.Find("Background/Scroll View/Viewport/Content").GetComponent<RectTransform>();
        BackgroundTransform = transform.Find("Background").GetComponent<RectTransform>();

        var content_size_fitter = ContentTransform.gameObject.AddComponent<ContentSizeFitter>();
        var layout_group = ContentTransform.gameObject.AddComponent<VerticalLayoutGroup>();
        AutoLayoutRoot = ContentTransform.gameObject.AddComponent<AutoVertLayoutGroup>();

        content_size_fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        layout_group.spacing = 3;
        layout_group.childAlignment = TextAnchor.UpperCenter;
        layout_group.childControlHeight = false;
        layout_group.childControlWidth = false;
        layout_group.childForceExpandHeight = false;
        layout_group.childForceExpandWidth = false;
        layout_group.padding = new RectOffset(0, 0, 10, 5);

        Window = GetComponent<KingdomWindow>();
    }

    internal void Initialize()
    {
        Init();
        var top = AutoLayoutRoot.BeginHoriGroup(new Vector2(200, 54));
        top.name = "Top";
        top.transform.SetSiblingIndex(0);
        top.AddChild(Window.kingdomBanner.transform.parent.gameObject);
        var banners = top.BeginVertGroup(default, TextAnchor.UpperCenter, 5, new RectOffset(0, 0, 5, 5));
        banners.name = "Diplomacy";
        banners.AddChild(Window.containerBannersWar.transform.parent.gameObject);
        banners.AddChild(Window.containerBannersAllies.transform.parent.gameObject);

        var cities_fitter = Window.cityInfoPlacement.gameObject.AddComponent<ContentSizeFitter>();
        var cities_layout = Window.cityInfoPlacement.gameObject.AddComponent<VerticalLayoutGroup>();

        cities_fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        cities_layout.spacing = 3;
        cities_layout.childAlignment = TextAnchor.UpperCenter;
        cities_layout.childControlHeight = false;
        cities_layout.childControlWidth = false;
        cities_layout.childForceExpandHeight = false;
        cities_layout.childForceExpandWidth = false;

        var custom_part = AutoLayoutRoot.BeginHoriGroup(new Vector2(200, 36), TextAnchor.MiddleLeft, -4,
                                                        new RectOffset(-3, 0, 3, 3));
        custom_part.name = "Middle";
        custom_part.transform.SetSiblingIndex(AutoLayoutRoot.transform.Find("MottoName").GetSiblingIndex());

        king_avatar = BackgroundTransform.Find("BackgroundLeft").GetComponentInChildren<UiUnitAvatarElement>(true);
        custom_part.AddChild(king_avatar.gameObject);
        king_avatar.GetComponent<Image>().enabled = true;
        king_avatar.transform.localScale = new Vector3(0.7f, 0.7f);

        var middle_bar_group =
            custom_part.BeginVertGroup(new Vector2(120, 36), TextAnchor.UpperCenter, 2, new RectOffset(0, 0, 0, 0));
        year_name = Instantiate(SimpleText.Prefab, null);
        year_name.Setup("", TextAnchor.MiddleCenter, new Vector2(120, 16));
        middle_bar_group.AddChild(year_name.gameObject);


        var policy_group = middle_bar_group.BeginHoriGroup(new Vector2(120, 16), TextAnchor.UpperCenter, 2,
                                                           new RectOffset(0, 0, 0, 0));

        current_state = Instantiate(KingdomPolicyStateButton.Prefab, null);
        current_state.Setup(KingdomPolicyStateLibrary.DefaultState, null);
        current_state.SetSize(new Vector2(16, 16));
        var bgRect2 = current_state.bg.GetComponent<RectTransform>();
        bgRect2.sizeDelta = new Vector2(60, 16);
        policy_group.AddChild(current_state.gameObject);

        executing_policy = Instantiate(KingdomPolicyButton.Prefab, null);
        executing_policy.Setup(null, null, false, false);
        executing_policy.SetSize(new Vector2(16, 16));
        RectTransform bgRect = executing_policy.bg.GetComponent<RectTransform>();
        bgRect.sizeDelta = new Vector2(60, 16);

        policy_group.AddChild(executing_policy.gameObject);


        heir_avatar = Instantiate(king_avatar, null);
        custom_part.AddChild(heir_avatar.gameObject);
        heir_avatar.transform.localScale = new Vector3(0.7f, 0.7f);
        Button button = heir_avatar.GetComponent<Button>();
        button.onClick = new Button.ButtonClickedEvent();
        button.onClick.AddListener(() =>
        {
            kingdom.data.name = Window.nameInput.textField.text;
            Config.selectedUnit = kingdom.heir;
            ScrollWindow.moveAllToLeftAndRemove();
            ScrollWindow.showWindow("inspect_unit");
        });
        heir_avatar.tooltip_id = "actor_heir";
        heir_avatar.transform.Find("Kingdom Icon").gameObject.SetActive(false);


        NewUI.createBGWindowButton(
            Window.gameObject,
            -50,
            "iconworldlaw",
            "KingdomHistory",
            "Kingdom History",
            "Shows a kingdom's history",
            () => NewKingdomHistoryWindow.ShowWindow(Config.selectedKingdom.id)
        );
        // 临时使用的入口
        NewUI.createBGWindowButton(
            GameObject.Find("Canvas Container Main/Canvas - Windows/windows/kingdom"),
            0,
            "iconworldlaw",
            "KingdomPolicyGraph",
            "Kingdom Policy Graph",
            "Shows a kingdom's policy graph",
            () => KingdomPolicyGraphWindow.ShowWindow(Config.selectedKingdom)
        );
        base.Init();
        OnEnable();
    }
}