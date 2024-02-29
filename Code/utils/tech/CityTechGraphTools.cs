using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Figurebox.content;
using Figurebox.core.city_techs;
using Figurebox.utils.policy;
using UnityEngine;

namespace Figurebox.utils.tech;

public class CityTechGraphTools
{
    public static List<List<CityTechGraphNode>> SortTechs(IEnumerable<string> pTechs)
    {
        var nodes = new Dictionary<string, SortNode>();

        foreach (var tech_id in pTechs)
        {
            AW_CityTechAsset tech = CityTechLibrary.Instance.get(tech_id);
            if (tech == null) continue;
            nodes.Add(tech_id, new SortNode { asset = tech });
        }

        foreach (var node in nodes.Values)
        {
            AW_CityTechAsset tech = node.asset;
            if (tech.all_prepositions == null) continue;
            foreach (var pre_tech in tech.all_prepositions)
                if (nodes.TryGetValue(pre_tech.id, out SortNode pre_node))
                {
                    pre_node.next_nodes.Add(node);
                    node.prev_nodes.Add(pre_node);
                }
        }
        _checkNodes(nodes.Values);

        var result = nodes.Values
            .GroupBy(x => x.sort_value)
            .OrderBy(x => x.Key)
            .Select(x => x.Select(y => new CityTechGraphNode(y.asset)).ToList())
            .ToList();

        var tmp_dict = result.SelectMany(x => x).ToDictionary(x => x.asset.id);
        foreach (var node in nodes.Values)
        {
            var graph_node = tmp_dict[node.asset.id];
            foreach (var next_node in node.next_nodes)
            {
                graph_node.next.Add(tmp_dict[next_node.asset.id]);
            }
        }

        return result;
    }

    public static void CalcNodePositions(List<List<CityTechGraphNode>> pSortedTechs)
    {
        var y = -20f;
        foreach (var list in pSortedTechs)
        {
            var j = 0;
            foreach (CityTechGraphNode node in list)
            {
                node.position = new Vector2((j - list.Count * 0.5f) * 50, y);
                j++;
            }

            y -= Mathf.Min(Mathf.Sqrt(list.Count * 0.2f) * 100 + 20, 50);
        }
    }

    private static void _checkNodes(IEnumerable<SortNode> pNodesValues)
    {
        foreach (var node in pNodesValues)
        {
            if (node.sort_value >= 0) continue;
            if (node.prev_nodes.Count == 0)
            {
                node.sort_value = 0;
                continue;
            }
            _checkNodes(node.prev_nodes);
            node.sort_value = node.prev_nodes.Max(x => x.sort_value) + 1;
        }
    }
    private class SortNode
    {
        public readonly HashSet<SortNode> next_nodes = new();
        public readonly HashSet<SortNode> prev_nodes = new();
        public AW_CityTechAsset asset;
        public int sort_value = -1;
    }
}