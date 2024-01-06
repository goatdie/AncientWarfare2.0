using DG.Tweening;
using NeoModLoader.api.attributes;
using NeoModLoader.General;
using NeoModLoader.General.UI.Prefabs;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Figurebox.prefabs;

/// <summary>
///     国家政策按钮
/// </summary>
public class KingdomPolicyButton : APrefab<KingdomPolicyButton>
{
    public Image bg;
    public TipButton tip_button;
    private Button button;
    private Image icon;
    private Text text;
    public TooltipData tip_data;
    public KingdomPolicyAsset policy { get; private set; }

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

    [Hotfixable]
    public void Setup(KingdomPolicyAsset pPolicyAsset, UnityAction<KingdomPolicyAsset> pClickAction = null,
        bool pSpecial = false, bool pHiddenBackground = false)
    {
        Init();
        bg.enabled = !pHiddenBackground; // 确保背景总是被启用，除非明确要求隐藏

        if (pPolicyAsset == null)
        {
            // 当没有内容时，隐藏图标和文本，但保持背景显示
            icon.enabled = false;
            text.enabled = false;
            tip_button.enabled = false;
        }
        else
        {
            tip_button.enabled = true;
            // 当有内容时，正常设置图标、文本和按钮
            policy = pPolicyAsset;
            Sprite iconSprite = SpriteTextureLoader.getSprite(pPolicyAsset.path_icon);
            icon.enabled = iconSprite != null;
            text.enabled = iconSprite == null;
            if (iconSprite == null)
            {
                text.text = LM.Get(pPolicyAsset.path_icon);
            }
            else
            {
                icon.sprite = iconSprite;
            }

            button.onClick.RemoveAllListeners();
            if (pClickAction != null)
            {
                button.onClick.AddListener(() => pClickAction?.Invoke(policy));
            }

            tip_data = new TooltipData
            {
                tip_name = policy.id
            };
        }

        // 设置背景图片
        bg.sprite = SpriteTextureLoader.getSprite(pSpecial ? "ui/special/button2" : "ui/special/button");
    }

    private void showTooltip()
    {
        if (policy == null) return;
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

    private static void _init()
    {
        GameObject obj = new("KingdomPolicyButton", typeof(Image), typeof(Button), typeof(TipButton));
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

        obj.GetComponent<TipButton>().type = "kingdom_policy";
        Prefab = obj.AddComponent<KingdomPolicyButton>();
    }
}