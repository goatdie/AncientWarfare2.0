using System.Collections.Generic;
using System.Linq;
using AncientWarfare.Core.Profession;

namespace AncientWarfare.Core.Tech
{
    public class TechAsset : Asset
    {
        public int               base_cost;
        public TechCategoryAsset direct_category;

        public TechAsset(string id, int cost, params NewProfessionAsset[] professions)
        {
            this.id = id;
            base_cost = cost;
            AddProfessions(professions);
        }

        public List<NewProfessionAsset> ProfessionList    { get; } = new();
        public List<string>             ProfessionIDList  { get; } = new();
        public List<TechAsset>          PreliminaryList   { get; } = new();
        public List<string>             PreliminaryIDList { get; } = new();
        public List<TechAsset>          InspirationList   { get; } = new();
        public List<string>             InspirationIDList { get; } = new();

        public void AddProfessions(params NewProfessionAsset[] professions)
        {
            ProfessionList.AddRange(professions);
            ProfessionIDList.AddRange(professions.Select(x => x.id));
        }

        public void AddPreliminaries(params TechAsset[] preliminaries)
        {
            PreliminaryList.AddRange(preliminaries);
            PreliminaryIDList.AddRange(preliminaries.Select(x => x.id));
        }

        public void AddInspirations(params TechAsset[] inspirations)
        {
            InspirationList.AddRange(inspirations);
            InspirationIDList.AddRange(inspirations.Select(x => x.id));
        }
    }
}