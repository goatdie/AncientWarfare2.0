using DG.Tweening;
using NeoModLoader.api.attributes;
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
    private Image bg;
    private Button button;
    private Image icon;
    private KingdomPolicyAsset policy;
    private TipButton tip_button;
    protected override void Init()
    {
        if (Initialized) return;
        icon = transform.Find("Icon").GetComponent<Image>();
        bg = GetComponent<Image>();
        button = GetComponent<Button>();
        tip_button = gameObject.GetComponent<TipButton>();
        tip_button.hoverAction = showTooltip;
        tip_button.clickAction = showTooltip;
        base.Init();
    }
    [Hotfixable]
    public void Setup(KingdomPolicyAsset pPolicyAsset, UnityAction<KingdomPolicyAsset> pClickAction = null, bool pSpecial = false, bool pHiddenBackground = false)
    {
        Init();
        bg.enabled = !pHiddenBackground;
        policy = pPolicyAsset;
        icon.sprite = string.IsNullOrEmpty(pPolicyAsset.path_icon) ? SpriteTextureLoader.getSprite("ui/icons/iconDamage") : SpriteTextureLoader.getSprite(pPolicyAsset.path_icon);

        button.onClick.RemoveAllListeners();

        if (pClickAction != null)
        {
            button.onClick.AddListener(() => pClickAction?.Invoke(policy));
        }
        bg.sprite = SpriteTextureLoader.getSprite(pSpecial ? "ui/special/button2" : "ui/special/button");
    }
    private void showTooltip()
    {
        if (policy == null) return;
        Tooltip.show(icon.gameObject, tip_button.type, new TooltipData
        {
            tip_name = policy.id
        });
        icon.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        icon.transform.DOKill();
        icon.transform.DOScale(1f, 0.1f).SetEase(Ease.InBack);
    }
    public override void SetSize(Vector2 pSize)
    {
        icon.GetComponent<RectTransform>().sizeDelta = pSize * 0.875f;
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

        obj.GetComponent<TipButton>().type = "kingdom_policy";
        Prefab = obj.AddComponent<KingdomPolicyButton>();
    }
}