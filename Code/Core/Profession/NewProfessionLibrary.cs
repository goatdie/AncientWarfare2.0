using AncientWarfare.Abstracts;

namespace AncientWarfare.Core.Profession;

public class NewProfessionLibrary : AW_AssetLibrary<NewProfessionAsset, NewProfessionLibrary>, IManager
{
    public static readonly NewProfessionAsset battle;
    public static readonly NewProfessionAsset forge;
    public static readonly NewProfessionAsset collect;
    public static readonly NewProfessionAsset build;
    public static readonly NewProfessionAsset mine;
    public static readonly NewProfessionAsset craft;
    public static readonly NewProfessionAsset lead;
    public static readonly NewProfessionAsset study;

    public void Initialize()
    {
        id = "aw_professions";
        init();
    }

    public override void init()
    {
        add(new NewProfessionAsset(nameof(battle)));
        add(new NewProfessionAsset(nameof(forge)));
        add(new NewProfessionAsset(nameof(build)));
        add(new NewProfessionAsset(nameof(collect)));
        add(new NewProfessionAsset(nameof(mine)));
        add(new NewProfessionAsset(nameof(craft)));
        add(new NewProfessionAsset(nameof(study)));
        add(new NewProfessionAsset(nameof(lead)));
    }
}