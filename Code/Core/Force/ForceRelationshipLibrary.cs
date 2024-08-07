using AncientWarfare.Abstracts;

namespace AncientWarfare.Core.Force;

public class ForceRelationshipLibrary : AW_AssetLibrary<ForceRelationshipAsset, ForceRelationshipLibrary>, IManager
{
    public void Initialize()
    {
        id = "aw_force_relations";
        init();
    }

    public override void init()
    {
    }
}