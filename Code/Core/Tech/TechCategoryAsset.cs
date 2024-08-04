using System.Collections.Generic;
using System.Linq;

namespace AncientWarfare.Core.Tech;

public class TechCategoryAsset : Asset
{
    public TechCategoryAsset(string id, params TechCategoryAsset[] parents)
    {
        this.id = id;
        AddParents(parents);
    }

    public HashSet<TechCategoryAsset> Parents     { get; } = new();
    public List<TechCategoryAsset>    ParentsList { get; } = new();

    public void AddParents(params TechCategoryAsset[] parents)
    {
        var except = parents.Except(Parents).ToList();
        ParentsList.AddRange(except);
        Parents.UnionWith(except);
    }

    public List<TechCategoryAsset> GetAllParents()
    {
        HashSet<TechCategoryAsset> res = new();
        Queue<TechCategoryAsset> queue = new();
        queue.Enqueue(this);
        while (queue.Count > 0)
        {
            TechCategoryAsset cat = queue.Dequeue();
            foreach (TechCategoryAsset parent in cat.ParentsList.Where(parent => res.Add(parent)))
                queue.Enqueue(parent);
        }

        return res.ToList();
    }
}