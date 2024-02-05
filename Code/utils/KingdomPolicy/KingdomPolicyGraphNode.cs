using System.Collections.Generic;
using UnityEngine;

namespace Figurebox.Utils.KingdomPolicy;

public class KingdomPolicyGraphNode
{
    public readonly Asset   asset;
    public readonly bool    is_state;
    public Vector2 position;
    public readonly HashSet<KingdomPolicyGraphNode> next = new();

    public KingdomPolicyGraphNode(Asset pAsset)
    {
        asset = pAsset;
        if (asset is KingdomPolicyStateAsset) is_state = true;
    }

    public KingdomPolicyAsset      as_policy => asset as KingdomPolicyAsset;
    public KingdomPolicyStateAsset as_state  => asset as KingdomPolicyStateAsset;
}