using UnityEngine;

namespace Figurebox.Utils.KingdomPolicy;

public class KingdomPolicyGraphNode
{
    public Asset   asset;
    public bool    is_state;
    public Vector2 position;

    public KingdomPolicyGraphNode(Asset pAsset)
    {
        asset = pAsset;
        if (asset is KingdomPolicyStateAsset) is_state = true;
    }

    public KingdomPolicyAsset      as_policy => asset as KingdomPolicyAsset;
    public KingdomPolicyStateAsset as_state  => asset as KingdomPolicyStateAsset;
}