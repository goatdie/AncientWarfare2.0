using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Figurebox.core;
using Figurebox.ui.prefabs;
using Figurebox.utils.extensions;
using Figurebox.utils.tech;
using NeoModLoader.api;
using UnityEngine;
using UnityEngine.UI;

namespace Figurebox.ui.windows;
public class CityTechGraphWindow : AbstractWideWindow<CityTechGraphWindow>
{
    private AW_City _city;

    private ObjectPoolGenericMono<CityTechButton> _tech_button_pool;
    private List<List<CityTechGraphNode>> _sorted_techs;
    protected override void Init()
    {
        Transform scroll_view = BackgroundTransform.Find("Scroll View");
        scroll_view.transform.localPosition = Vector3.zero;
        scroll_view.transform.localScale = Vector3.one;

        var viewport = scroll_view.Find("Viewport").GetComponent<RectTransform>();
        viewport.pivot = new Vector2(0.5f, 0.5f);
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

        _tech_button_pool = new ObjectPoolGenericMono<CityTechButton>(CityTechButton.Prefab, ContentTransform);
    }

    public override void OnNormalEnable()
    {
        _city ??= Config.selectedCity.AW();
        _sorted_techs = CityTechGraphTools.SortTechs(_city.addition_data.own_tech);
        CityTechGraphTools.CalcNodePositions(_sorted_techs);
        // TODO: 放置按钮
    }
}
