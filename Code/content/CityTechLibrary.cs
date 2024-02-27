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
    /// ����
    /// </summary>
    public static readonly AW_CityTechAsset pottery;
    /// <summary>
    /// �ɿ�
    /// </summary>
    public static readonly AW_CityTechAsset mining;
    /// <summary>
    /// ����
    /// </summary>
    public static readonly AW_CityTechAsset navigation;
    /// <summary>
    /// ռ��
    /// </summary>
    public static readonly AW_CityTechAsset horoscope;
    /// <summary>
    /// ��д
    /// </summary>
    public static readonly AW_CityTechAsset handwriting;
    /// <summary>
    /// ����
    /// </summary>
    public static readonly AW_CityTechAsset bow;
    /// <summary>
    /// ʯ��
    /// </summary>
    public static readonly AW_CityTechAsset masonry;
    /// <summary>
    /// ��ͭ
    /// </summary>
    public static readonly AW_CityTechAsset bronze;
    /// <summary>
    /// ����
    /// </summary>
    public static readonly AW_CityTechAsset wheel;
    /// <summary>
    /// ��
    /// </summary>
    public static readonly AW_CityTechAsset iron;

    /// <summary>
    /// ����
    /// </summary>
    public static readonly AW_CityTechAsset husbandry;
    /// <summary>
    /// ���
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