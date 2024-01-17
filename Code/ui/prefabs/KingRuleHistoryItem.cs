using System.Text;
using Figurebox.constants;
using Figurebox.core.table_items;
using Figurebox.ui.windows;
using Figurebox.Utils;
using Figurebox.Utils.extensions;
using NeoModLoader.api.attributes;
using NeoModLoader.General.UI.Prefabs;
using UnityEngine;
using UnityEngine.UI;

namespace Figurebox.ui.prefabs;

public class KingRuleHistoryItem : APrefab<KingRuleHistoryItem>
{
    private SimpleAvatar Avatar;
    private SimpleText Date;

    private ActorTableItem king;
    private KingdomTableItem kingdom;
    private SimpleText Name;
    private KingRuleTableItem rule;

    protected override void Init()
    {
        if (Initialized) return;
        base.Init();
        Name = transform.Find("Name").GetComponent<SimpleText>();
        Date = transform.Find("Date").GetComponent<SimpleText>();
        Avatar = transform.Find("Avatar").GetComponent<SimpleAvatar>();

        Name.Setup("", TextAnchor.MiddleCenter, new Vector2(44, 18));
        Name.background.enabled = false;
        Name.text.resizeTextMaxSize = 10;
        Date.Setup("", TextAnchor.MiddleCenter, new Vector2(44, 18));
        Date.background.enabled = false;
        Date.text.resizeTextMaxSize = 10;

        var button = Avatar.transform.Find("Mask").GetComponent<Button>();
        button.OnHover(() =>
        {
            Tooltip.show(Avatar.gameObject, "history_king", new TooltipData
            {
                tip_name = GeneralHelper.ToJSON(king),
                tip_description = GeneralHelper.ToJSON(rule),
                tip_description_2 = GeneralHelper.ToJSON(kingdom)
            });
        });
        button.onClick.AddListener(() =>
        {
            NewKingdomHistoryWindow.Instance.SelectKingRule(rule);
            MarkSelected(true);
        });
    }

    [Hotfixable]
    public void MarkSelected(bool pIsSelected)
    {
        Avatar.transform.Find("AvatarUnitType").gameObject.SetActive(pIsSelected);
    }

    public void Setup(ActorTableItem pKing, KingRuleTableItem pRule, KingdomTableItem pKingdom)
    {
        Init();
        king = pKing;
        rule = pRule;
        kingdom = pKingdom;
        Name.text.text = pKing.curr_name;
        if (pRule.end_time < 0)
            Date.text.text = $"{GeneralHelper.getYearsOn(pRule.start_time)}~";
        else
            Date.text.text =
                $"{GeneralHelper.getYearsOn(pRule.start_time)}~{GeneralHelper.getYearsOn(pRule.end_time)}";

        Avatar.Show(pKing.asset_id, pKing.skin, pKing.skin_set, pKing.head, pKingdom.color_id, AWUnitProfession.King);
    }

    public override void SetSize(Vector2 pSize)
    {
    }

    private static void _init()
    {
        var obj = new GameObject(nameof(KingRuleHistoryItem), typeof(Image));
        var bg = obj.GetComponent<Image>();
        bg.sprite = SpriteTextureLoader.getSprite("ui/special/backgroundSpectateElement");
        bg.type = Image.Type.Sliced;
        obj.GetComponent<RectTransform>().sizeDelta = new Vector2(80, 36);

        var avatar_obj =
            Instantiate(Resources.Load<WindowCreatureInfo>("windows/inspect_unit").avatarElement.gameObject,
                obj.transform);
        avatar_obj.name = "Avatar";
        avatar_obj.GetComponent<RectTransform>().sizeDelta = new Vector2(36, 36);
        Destroy(avatar_obj.GetComponent<UiUnitAvatarElement>());
        avatar_obj.transform.localPosition = new Vector3(-25, 0, 0);
        var avatar = avatar_obj.AddComponent<SimpleAvatar>();
        avatar.transform.Find("AvatarUnitType").gameObject.SetActive(false);
        avatar.transform.Find("AvatarUnitType").GetComponent<Image>().sprite =
            SpriteTextureLoader.getSprite("ui/special/windowAvatarElement_king");

        var name = Instantiate(SimpleText.Prefab, obj.transform);
        name.name = "Name";
        name.transform.localPosition = new Vector3(14, 9);
        var date = Instantiate(SimpleText.Prefab, obj.transform);
        date.name = "Date";
        date.transform.localPosition = new Vector3(14, -7);

        Prefab = obj.AddComponent<KingRuleHistoryItem>();
    }

    private class SimpleAvatar : MonoBehaviour
    {
        private UnitAvatarLoader _loader;

        public void Show(string pAssetId, int pSkin, int pSkinSet, int pHead, int pColorID,
            AWUnitProfession pProfession)
        {
            if (_loader == null) _loader = transform.GetComponentInChildren<UnitAvatarLoader>(true);
            var asset = AssetManager.actor_library.get(pAssetId);
            if (asset == null)
            {
                gameObject.SetActive(false);
                return;
            }

            gameObject.SetActive(true);

            while (_loader.transform.childCount > 0)
            {
                var child = _loader.transform.GetChild(0);
                child.SetParent(null);
                Destroy(child.gameObject);
            }

            _loader.transform.localScale = new Vector3(asset.inspectAvatarScale * _loader.avatarSize,
                asset.inspectAvatarScale * _loader.avatarSize, asset.inspectAvatarScale);
            var sprite = TrackActorSprite(asset, pSkin, pSkinSet, pHead, pProfession,
                AssetManager.kingdom_colors_library.getColorByIndex(pColorID));
            _loader.showSpritePart(sprite, null,
                new Vector3(asset.inspectAvatar_offset_x, asset.inspectAvatar_offset_y));
        }

        private Sprite TrackActorSprite(ActorAsset pAsset, int pSkin, int pSkinSet, int pHead,
            AWUnitProfession pProfession, ColorAsset pColorAsset)
        {
            if (pAsset.has_override_sprite)
                // 并不能处理这种情况，使用icon吧
                return SpriteTextureLoader.getSprite($"ui/icons/{pAsset.icon}");

            string texture_path;
            if (pAsset.unit)
            {
                var race = AssetManager.raceLibrary.get(pAsset.race);
                var sb = new StringBuilder();
                sb.Append(race.main_texture_path);
                if (pAsset.baby)
                {
                    sb.Append("unit_child");
                }
                else
                {
                    var profession = AssetManager.professions.get(pProfession.C());
                    sb.Append(!string.IsNullOrEmpty(profession.special_skin_path)
                        ? profession.special_skin_path
                        : race.skin_civ_default_male);
                }

                texture_path = sb.ToString();
            }
            else
            {
                texture_path = pAsset.texture_path;
            }

            var anim_container = ActorAnimationLoader.loadAnimationUnit($"actors/{texture_path}", pAsset);

            if (pHead < 0) pHead = 0;
            Sprite head = null;
            if (pAsset.body_separate_part_head)
            {
                if (!pAsset.unit)
                {
                    head = anim_container.heads[pHead];
                }
                else
                {
                    var special_head = false;
                    var head_sub_path = "";
                    if (pProfession == AWUnitProfession.King)
                    {
                        head_sub_path = "head_king";
                        special_head = true;
                    }
                    else if (pProfession == AWUnitProfession.Warrior)
                    {
                        head_sub_path = "head_warrior";
                        special_head = true;
                    }
                    else
                    {
                        head_sub_path = "head_male";
                    }

                    if (special_head)
                        head = ActorAnimationLoader.getHeadSpecial($"actors/races/{pAsset.race}/heads_special",
                            head_sub_path);
                    else
                        head = ActorAnimationLoader.getHead($"actors/races/{pAsset.race}/{head_sub_path}", pHead);
                }
            }

            var body = anim_container.idle.frames[0];

            return UnitSpriteConstructorExtension.GetSpriteUnit(anim_container.dict_frame_data[body.name], body, head,
                pAsset, pColorAsset, pSkinSet, pSkin, pAsset.texture_atlas);
        }
    }
}