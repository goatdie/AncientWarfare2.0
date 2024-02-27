using Figurebox.abstracts;
using Figurebox.core.city_techs;
using System;
using UnityEngine.Rendering;

namespace Figurebox.content;

public class CityTechLibrary : AW_AssetLibrary<AW_CityTechAsset, CityTechLibrary>
{
    public static readonly AW_CityTechAsset slave_labor_analysis;
    public static readonly AW_CityTechAsset enfeoffment_power_analysis;
    public static readonly AW_CityTechAsset enfeoffment_range_analysis;

    /// <summary>
    /// ÌÕÆ÷
    /// </summary>
    public static readonly AW_CityTechAsset pottery;
    /// <summary>
    /// ²É¿ó
    /// </summary>
    public static readonly AW_CityTechAsset mining;
    /// <summary>
    /// º½º£
    /// </summary>
    public static readonly AW_CityTechAsset navigation;
    /// <summary>
    /// Õ¼ÐÇ
    /// </summary>
    public static readonly AW_CityTechAsset horoscope;
    /// <summary>
    /// ÊéÐ´
    /// </summary>
    public static readonly AW_CityTechAsset handwriting;
    /// <summary>
    /// ¹­¼ý
    /// </summary>
    public static readonly AW_CityTechAsset bow;
    /// <summary>
    /// Ê¯¹¤
    /// </summary>
    public static readonly AW_CityTechAsset masonry;
    /// <summary>
    /// ÇàÍ­
    /// </summary>
    public static readonly AW_CityTechAsset bronze;
    /// <summary>
    /// ÂÖ×Ó
    /// </summary>
    public static readonly AW_CityTechAsset wheel;
    /// <summary>
    /// Ìú
    /// </summary>
    public static readonly AW_CityTechAsset iron;

    /// <summary>
    /// ÐóÄÁ
    /// </summary>
    public static readonly AW_CityTechAsset husbandry;
    /// <summary>
    /// ¹à¸È
    /// </summary>
    public static readonly AW_CityTechAsset irrigate;

    public override void init()
    {
        base.init();
        add_agriculture_techs();
        add_industry_techs();
        add_policy_techs();
    }
    public override void post_init()
    {
        base.post_init();
    }

    private void add_policy_techs()
    {
        add(new AW_CityTechAsset()
        {
            id = "slave_labor_analysis",
            tech_category = "policy:labor"
        });

        add(new AW_CityTechAsset()
        {
            id = "enfeoffment_power_analysis",
            tech_category = "policy:enfeoffment"
        });
        add(new AW_CityTechAsset()
        {
            id = "enfeoffment_range_analysis",
            tech_category = "policy:enfeoffment"
        });
    }

    private void add_industry_techs()
    {
        add(new AW_CityTechAsset()
        {
            id = nameof(pottery)
        });
        add(new AW_CityTechAsset()
        {
            id = nameof(mining)
        });
        add(new AW_CityTechAsset()
        {
            id = nameof(navigation)
        });
        add(new AW_CityTechAsset()
        {
            id = nameof(horoscope)
        });
        add(new AW_CityTechAsset()
        {
            id = nameof(handwriting)
        });
        add(new AW_CityTechAsset()
        {
            id = nameof(bow)
        });
        add(new AW_CityTechAsset()
        {
            id = nameof(masonry)
        });
        add(new AW_CityTechAsset()
        {
            id = nameof(bronze)
        });
        add(new AW_CityTechAsset()
        {
            id = nameof(wheel)
        });
        add(new AW_CityTechAsset()
        {
            id = nameof(iron)
        });
    }

    private void add_agriculture_techs()
    {
        add(new AW_CityTechAsset()
        {
            id = nameof(husbandry)
        });
        add(new AW_CityTechAsset()
        {
            id = nameof(irrigate)
        });
    }
}