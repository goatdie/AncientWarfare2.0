using Figurebox.core;
using Figurebox.prefabs;
using Figurebox.prefabs;
using NeoModLoader.General.UI.Window.Layout;
using NeoModLoader.General.UI.Window.Utils.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Figurebox.prefabs;
namespace Figurebox;

internal class KingdomWindowAdditionComponent : AutoVertLayoutGroup
{
    private AutoVertLayoutGroup AutoLayoutRoot;
    private RectTransform BackgroundTransform;
    private RectTransform ContentTransform;
    private UiUnitAvatarElement heir_avatar;
    private UiUnitAvatarElement king_avatar;
    private SimpleText yearNameText;
    private KingdomWindow Window;
    private SimpleText year_name;
    private AW_Kingdom kingdom => (AW_Kingdom)Config.selectedKingdom;

    private void OnEnable()
    {
        if (!Initialized) return;
        heir_avatar.show(kingdom.heir);
        actorAvatarDisplaySetting(heir_avatar, heir_avatar.gameObject.activeSelf);
        actorAvatarDisplaySetting(king_avatar, king_avatar.gameObject.activeSelf);
        void actorAvatarDisplaySetting(UiUnitAvatarElement avatar, bool pActive)
        {
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
        Window.elements.Clear();
    }
    private void OnDisable()
    {
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

        var custom_part = AutoLayoutRoot.BeginHoriGroup(new Vector2(200, 36), TextAnchor.MiddleLeft, -4, new RectOffset(-3, 0, 3, 3));
        custom_part.name = "Middle";
        custom_part.transform.SetSiblingIndex(AutoLayoutRoot.transform.Find("MottoName").GetSiblingIndex());

        king_avatar = BackgroundTransform.Find("BackgroundLeft").GetComponentInChildren<UiUnitAvatarElement>(true);
        custom_part.AddChild(king_avatar.gameObject);
        king_avatar.GetComponent<Image>().enabled = true;
        king_avatar.transform.localScale = new Vector3(0.7f, 0.7f);

        var middle_bar_group = custom_part.BeginVertGroup(new Vector2(120, 36), TextAnchor.UpperCenter, 2, new RectOffset(0, 0, 0, 0));
        year_name = Instantiate(SimpleText.Prefab, null);
        year_name.Setup("", TextAnchor.MiddleCenter, new Vector2(120, 16));
        middle_bar_group.AddChild(year_name.gameObject);


        GameObject policy_bar = new("PolicyBar", typeof(Image));
        middle_bar_group.AddChild(policy_bar.gameObject);
        policy_bar.GetComponent<RectTransform>().sizeDelta = new Vector2(120, 16);
        middle_bar_group.AddChild(policy_bar.gameObject);
        policy_bar.GetComponent<RectTransform>().sizeDelta = new Vector2(120, 16);
        policy_bar.GetComponent<Image>().sprite = Resources.Load<Sprite>("ui/special/windowInnerSliced");
        policy_bar.GetComponent<Image>().type = Image.Type.Sliced;

        heir_avatar = Instantiate(king_avatar, null);
        custom_part.AddChild(heir_avatar.gameObject);
        heir_avatar.transform.localScale = new Vector3(0.7f, 0.7f);
        heir_avatar.unit_type_bg.sprite = heir_avatar.type_leader;
        heir_avatar.transform.Find("Kingdom Icon").gameObject.SetActive(false);

        base.Init();
        OnEnable();
    }

}