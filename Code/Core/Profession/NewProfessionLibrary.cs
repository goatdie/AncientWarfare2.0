using AncientWarfare.Abstracts;

namespace AncientWarfare.Core.Profession;

public class NewProfessionLibrary : AW_AssetLibrary<NewProfessionAsset, NewProfessionLibrary>, IManager
{
    public static readonly string hunter;
    public static readonly string blacksmith;
    public static readonly string collector;
    public static readonly string miner;

    public void Initialize()
    {
        id = "aw_professions";
        init();
    }

    public override void init()
    {
        add(new NewProfessionAsset(nameof(hunter)));
        add(new NewProfessionAsset(nameof(blacksmith)));
        add(new NewProfessionAsset(nameof(collector)));
        add(new NewProfessionAsset(nameof(miner)));
    }
}