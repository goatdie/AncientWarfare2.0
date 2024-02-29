using System.Collections.Generic;
using System.Linq;
using Figurebox.content;
using Figurebox.core.kingdom_policies;
using NeoModLoader.api.attributes;
using UnityEngine;

namespace Figurebox.utils.policy;

public class KingdomPolicyGraphTools
{
    [Hotfixable]
    public static List<List<KingdomPolicyGraphNode>> SortPoliciesWithStates(IEnumerable<string> pPolicies)
    {
        var nodes = new Dictionary<string, SortNode>();
        foreach (var policy_id in pPolicies)
        {
            KingdomPolicyAsset policy = KingdomPolicyLibrary.Instance.get(policy_id);
            if (policy == null || policy.can_repeat) continue;
            nodes.Add("p_" + policy_id, new SortNode { asset = policy });
            if (!string.IsNullOrEmpty(policy.target_state_id))
            {
                KingdomPolicyStateAsset state = KingdomPolicyStateLibrary.Instance.get(policy.target_state_id);
                if (state == null) continue;
                nodes.Add("s_" + policy.target_state_id, new SortNode { asset = state, is_state = true });
            }
        }

        nodes.Add("s_" + KingdomPolicyStateLibrary.DefaultState.id, new SortNode
        {
            asset = KingdomPolicyStateLibrary.DefaultState, is_state = true
        });

        foreach (SortNode node in nodes.Values)
            if (node.is_state)
            {
                KingdomPolicyStateAsset state = node.as_state;
                if (state.all_optional_policies == null) continue;
                foreach (var branch_policy in state.all_optional_policies)
                    if (nodes.TryGetValue("p_" + branch_policy.id, out SortNode branch_node))
                    {
                        branch_node.prev_nodes.Add(node);
                        node.next_nodes.Add(branch_node);
                    }
            }
            else
            {
                KingdomPolicyAsset policy = node.as_policy;
                if (!string.IsNullOrEmpty(policy.target_state_id) &&
                    nodes.TryGetValue("s_" + policy.target_state_id, out SortNode target_node))
                {
                    target_node.prev_nodes.Add(node);
                    node.next_nodes.Add(target_node);
                }

                if (policy.all_prepositions == null) continue;

                foreach (var prev_state in policy.all_prepositions)
                    if (nodes.TryGetValue("s_" + prev_state.id, out SortNode prev_node))
                    {
                        prev_node.next_nodes.Add(node);
                        node.prev_nodes.Add(prev_node);
                    }
            }

        CheckNodes(nodes.Values);

        var result = nodes.Values
            .GroupBy(x => x.sort_value)
            .OrderBy(x => x.Key)
            .Select(x => x.Select(y => new KingdomPolicyGraphNode(y.asset)).ToList())
            .ToList();

        var tmp_dict = result.SelectMany(x => x)
            .ToDictionary(x => (x.is_state ? "s_" : "p_") + x.asset.id);
        foreach (var node in nodes.Values)
        {
            var graph_node = tmp_dict[(node.is_state ? "s_" : "p_") + node.asset.id];
            foreach (var next_node in node.next_nodes)
                graph_node.next.Add(tmp_dict[(next_node.is_state ? "s_" : "p_") + next_node.asset.id]);
        }
        
        return result;
    }

    public static void CalcNodePositions(List<List<KingdomPolicyGraphNode>> pSortedPolicies)
    {
        var y = -20f;
        foreach (var list in pSortedPolicies)
        {
            var j = 0;
            foreach (KingdomPolicyGraphNode node in list)
            {
                node.position = new Vector2((j - list.Count * 0.5f) * 50, y);
                j++;
            }

            y -= Mathf.Min(Mathf.Sqrt(list.Count * 0.2f) * 100 + 20, 50);
        }
    }

    private static void CheckNodes(IEnumerable<SortNode> pNodesValues)
    {
        foreach (SortNode node in pNodesValues)
        {
            if (node.sort_value >= 0) continue;
            if (node.prev_nodes.Count == 0)
            {
                node.sort_value = 0;
                continue;
            }

            CheckNodes(node.prev_nodes);
            node.sort_value = node.prev_nodes.Max(x => x.sort_value) + 1;
        }
    }

    private class SortNode
    {
        public readonly HashSet<SortNode>       next_nodes = new();
        public readonly HashSet<SortNode>       prev_nodes = new();
        public          Asset                   asset;
        public          bool                    is_state;
        public          int                     sort_value = -1;
        public          KingdomPolicyAsset      as_policy => asset as KingdomPolicyAsset;
        public          KingdomPolicyStateAsset as_state  => asset as KingdomPolicyStateAsset;
    }
}