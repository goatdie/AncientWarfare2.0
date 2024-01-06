using Figurebox.constants;
using Figurebox.core;
using NeoModLoader.api.attributes;

namespace Figurebox;

/// <summary>
///     政治状态（政体）
/// </summary>
public class KingdomPolicyStateLibrary : AssetLibrary<KingdomPolicyStateAsset>
{
    /// <summary>
    ///     社会等级制度: 奴隶制
    /// </summary>
    public static KingdomPolicyStateAsset SocialLevel_Slaves;

    public static KingdomPolicyStateLibrary Instance { get; } = new();

    /// <summary>
    ///     默认政体(政治状态)
    /// </summary>
    public static KingdomPolicyStateAsset DefaultState { get; private set; }

    public override void init()
    {
        base.init();
        // 只注册状态, 不要链接政策
        // 原始公平
        DefaultState = add("default", PolicyStateType.social_level);
        // 奴隶制
        SocialLevel_Slaves = add("slaves", PolicyStateType.social_level);
        // 封建贵族
        add("aristocrat", PolicyStateType.social_level);
        // 地主
        add("landlord", PolicyStateType.social_level);
        // 资本家
        add("capitalist", PolicyStateType.social_level);
        // 无产阶级
        add("proletarian", PolicyStateType.social_level);
    }

    public override void post_init()
    {
        // 添加可选政策
        DefaultState.AddOptionalPolicy(KingdomPolicyLibrary.Instance.get("start_slaves"));

        SocialLevel_Slaves.AddOptionalPolicy(
            KingdomPolicyLibrary.Instance.get("control_slaves"),
            KingdomPolicyLibrary.Instance.get("slaves_army")
        );
    }

    public override void linkAssets()
    {
        base.linkAssets();
    }

    public KingdomPolicyStateAsset add(string pID, string pType, string pPathIcon = null,
        FindPolicy pPolicyFinder = null,
        CalcKingdomStrength pCalcKingdomPower = null)
    {
        return add(new KingdomPolicyStateAsset
        {
            id = pID,
            type = pType,
            name = pID,
            description = pID + "_desc",
            path_icon = string.IsNullOrEmpty(pPathIcon) ? "ui/icons/iconDamage" : pPathIcon,
            policy_finder = pPolicyFinder ?? DefaultPolicyFinder,
            calc_kingdom_strength = pCalcKingdomPower ?? DefaultCalcKingdomPower
        });
    }

    public static float DefaultCalcKingdomPower(AW_Kingdom pKingdom)
    {
        int kingdomValue = 0;
        int populationTotal = pKingdom.getPopulationTotal();
        int cityCount = pKingdom.cities.Count * 100;
        int armySize = pKingdom.getArmy();
        int stewardship = 8;
        if (pKingdom.king != null)
        {
            stewardship = pKingdom.king.data.stewardship * 10;
        }

        return populationTotal + cityCount + armySize + stewardship;
    }

    [Hotfixable]
    public static KingdomPolicyAsset DefaultPolicyFinder(KingdomPolicyStateAsset pState, AW_Kingdom pKingdom)
    {
        return KingdomPolicyLibrary.Instance.get(pState.all_optional_policies.GetRandom());
    }
}