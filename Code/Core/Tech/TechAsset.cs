using System;
using System.Collections.Generic;
using System.Linq;

namespace AncientWarfare.Core.Tech
{
    public class TechAsset : Asset
    {
        public int               base_cost;
        public TechCategoryAsset direct_category;

        /// <summary>
        ///     是否来自于生产经验
        /// </summary>
        public bool from_production;

        public int  min_intelligence_required;
        public bool only_random;

        public TechAsset(string id, TechCategoryAsset category, int cost, params TechAsset[] preliminaries)
        {
            this.id = id;
            base_cost = cost;
            direct_category = category;
            AddPreliminaries(preliminaries.ToList());
        }

        public List<TechAsset> PreliminaryList   { get; } = new();
        public List<string>    PreliminaryIDList { get; } = new();

        public void AddPreliminaries(List<TechAsset> preliminaries)
        {
            throw new NotImplementedException();
        }
    }
}