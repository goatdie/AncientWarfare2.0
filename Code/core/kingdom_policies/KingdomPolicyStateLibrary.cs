using System;
using Figurebox.constants;
using Figurebox.core;
namespace Figurebox;

/// <summary>
///     政治状态（政体）
/// </summary>
public class KingdomPolicyStateLibrary : AssetLibrary<KingdomPolicyStateAsset>
{
    public static KingdomPolicyStateLibrary Instance { get; } = new();
    /// <summary>
    ///     默认政体(政治状态)
    /// </summary>
    public static KingdomPolicyStateAsset DefaultState { get; } = new();
    public override void init()
    {
        base.init();
        // 只注册状态, 不要链接政策
        DefaultState.id = "default";
        DefaultState.type = PolicyStateType.social_level;
        // 原始公平
        add(DefaultState);
        // 奴隶制
        add("slaves", PolicyStateType.social_level);
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
        base.post_init();
    }
    public override void linkAssets()
    {
        base.linkAssets();
    }
    public KingdomPolicyStateAsset add(string pID, string pType, FindPolicy pPolicyFinder = null, CalcKingdomStrength pCalcKingdomPower = null)
    {
        return add(new KingdomPolicyStateAsset
        {
            id = pID,
            type = pType,
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
    public static KingdomPolicyAsset DefaultPolicyFinder(AW_Kingdom pKingdom)
    {
        throw new NotImplementedException();
    }
}