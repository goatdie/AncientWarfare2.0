using AncientWarfare.Abstracts;

namespace AncientWarfare.Core.Tech;

public class TechCategoryLibrary : AW_AssetLibrary<TechCategoryAsset, TechCategoryLibrary>, IManager
{
    /// <summary>
    ///     哲学
    /// </summary>
    public static readonly TechCategoryAsset Philosophy;

    /// <summary>
    ///     经济学
    /// </summary>
    public static readonly TechCategoryAsset Economics;

    /// <summary>
    ///     法学
    /// </summary>
    public static readonly TechCategoryAsset Law;

    /// <summary>
    ///     教育学
    /// </summary>
    public static readonly TechCategoryAsset Education;

    /// <summary>
    ///     文学
    /// </summary>
    public static readonly TechCategoryAsset Literature;

    /// <summary>
    ///     历史学
    /// </summary>
    public static readonly TechCategoryAsset History;

    /// <summary>
    ///     理学
    /// </summary>
    public static readonly TechCategoryAsset Science;

    /// <summary>
    ///     工学
    /// </summary>
    public static readonly TechCategoryAsset Engineering;

    /// <summary>
    ///     农学
    /// </summary>
    public static readonly TechCategoryAsset Agronomy;

    /// <summary>
    ///     医学
    /// </summary>
    public static readonly TechCategoryAsset MedicalScience;

    /// <summary>
    ///     军事学
    /// </summary>
    public static readonly TechCategoryAsset MilitaryScience;

    /// <summary>
    ///     管理学
    /// </summary>
    public static readonly TechCategoryAsset Management;

    /// <summary>
    ///     艺术学
    /// </summary>
    public static readonly TechCategoryAsset Art;

    public void Initialize()
    {
        id = "aw_tech_categories";
        init();
    }

    public override void init()
    {
        add_top_level_categories();
    }

    private void add_top_level_categories()
    {
        add(new TechCategoryAsset(nameof(Philosophy)));
        add(new TechCategoryAsset(nameof(Economics)));
        add(new TechCategoryAsset(nameof(Law)));
        add(new TechCategoryAsset(nameof(Education)));
        add(new TechCategoryAsset(nameof(Literature)));
        add(new TechCategoryAsset(nameof(History)));
        add(new TechCategoryAsset(nameof(Science)));
        add(new TechCategoryAsset(nameof(Engineering)));
        add(new TechCategoryAsset(nameof(Agronomy)));
        add(new TechCategoryAsset(nameof(MedicalScience)));
        add(new TechCategoryAsset(nameof(MilitaryScience)));
        add(new TechCategoryAsset(nameof(Management)));
        add(new TechCategoryAsset(nameof(Art)));
    }
}