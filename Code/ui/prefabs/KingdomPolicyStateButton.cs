using DG.Tweening;
using NeoModLoader.General;
using NeoModLoader.General.UI.Prefabs;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Figurebox.ui.prefabs;

public class KingdomPolicyStateButton : APrefab<KingdomPolicyStateButton>
{
    public Image bg;
    private Button button;
    private Image icon;
    private Text text;
    private TipButton tip_button;
    public TooltipData tip_data;
    public KingdomPolicyStateAsset state { get; private set; }

    protected override void Init()
    {
        if (Initialized) return;
        icon = transform.Find("Icon").GetComponent<Image>();
        text = transform.Find("Text").GetComponent<Text>();
        bg = GetComponent<Image>();
        button = GetComponent<Button>();
        tip_button = gameObject.GetComponent<TipButton>();
        tip_button.hoverAction = showTooltip;
        tip_button.clickAction = showTooltip;
        base.Init();
    }

    private void showTooltip()
    {
        if (state == null) return;
        Tooltip.show(icon.gameObject, tip_button.type, tip_data);
        icon.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        icon.transform.DOKill();
        icon.transform.DOScale(1f, 0.1f).SetEase(Ease.InBack);
    }

    public override void SetSize(Vector2 pSize)
    {
        icon.GetComponent<RectTransform>().sizeDelta = pSize * 0.875f;
        text.GetComponent<RectTransform>().sizeDelta = pSize * 0.875f;
        base.SetSize(pSize);
    }

    public void Setup(KingdomPolicyStateAsset pPolicyStateAsset, UnityAction<KingdomPolicyStateAsset> pClickAction,
        bool pSpecial = false, bool pHiddenBackground = false)
    {
        Init();
        if (pPolicyStateAsset == null)
        {
            gameObject.SetActive(false);
            return;
        }

        bg.enabled = !pHiddenBackground;
        state = pPolicyStateAsset;
        Sprite iconSprite = SpriteTextureLoader.getSprite(pPolicyStateAsset.path_icon);
        icon.enabled = iconSprite != null;
        text.enabled = iconSprite == null;
        if (iconSprite == null)
        {
            text.text = LM.Get(pPolicyStateAsset.path_icon);
        }
        else
        {
            icon.sprite = iconSprite;
        }

        button.onClick.RemoveAllListeners();

        if (pClickAction != null)
        {
            button.onClick.AddListener(() => pClickAction?.Invoke(state));
        }

        bg.sprite = SpriteTextureLoader.getSprite(pSpecial ? "ui/special/button2" : "ui/special/button");
        tip_data = new TooltipData
        {
            tip_name = state.id
        };
    }

    private static void _init()
    {
        GameObject obj = new("KingdomPolicyStateButton", typeof(Image), typeof(Button), typeof(TipButton));
        obj.transform.SetParent(Main.prefabs_library);
        Image bg = obj.GetComponent<Image>();
        bg.sprite = SpriteTextureLoader.getSprite("ui/special/button");
        bg.type = Image.Type.Sliced;

        GameObject icon = new("Icon", typeof(Image));
        icon.transform.SetParent(obj.transform);
        icon.transform.localScale = Vector3.one;
        icon.transform.localPosition = Vector3.zero;
        Image iconImage = icon.GetComponent<Image>();
        iconImage.sprite = SpriteTextureLoader.getSprite("ui/icons/iconDamage");

        GameObject text = new("Text", typeof(Text));
        text.transform.SetParent(obj.transform);
        text.transform.localScale = Vector3.one;
        text.transform.localPosition = Vector3.zero;
        Text textComponent = text.GetComponent<Text>();
        textComponent.alignment = TextAnchor.MiddleCenter;
        textComponent.resizeTextForBestFit = true;
        textComponent.resizeTextMinSize = 1;
        textComponent.resizeTextMaxSize = 18;
        textComponent.text = "";
        textComponent.color = Color.black;
        textComponent.font = LocalizedTextManager.currentFont;

        obj.GetComponent<TipButton>().type = "kingdom_policy_state";
        Prefab = obj.AddComponent<KingdomPolicyStateButton>();
    }
}