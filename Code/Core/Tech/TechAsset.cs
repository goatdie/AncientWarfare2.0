using AncientWarfare.Abstracts;
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

        public AssetListContainer<ActorJob>           Suggestions     { get; } = new();
        public AssetListContainer<NewProfessionAsset> ProfessionList  { get; } = new();
        public AssetListContainer<TechAsset>          PreliminaryList { get; } = new();
        public AssetListContainer<TechAsset>          InspirationList { get; } = new();

        public void AddSuggestions(params ActorJob[] suggestions)
        {
            Suggestions.AddRange(suggestions);
        }

        public void AddProfessions(params NewProfessionAsset[] professions)
        {
            ProfessionList.AddRange(professions);
        }

        public void AddPreliminaries(params TechAsset[] preliminaries)
        {
            PreliminaryList.AddRange(preliminaries);
            TechLibrary.Instance.SetTopoDirty();
        }

        public void AddInspirations(params TechAsset[] inspirations)
        {
            InspirationList.AddRange(inspirations);
            TechLibrary.Instance.SetTopoDirty();
        }
    }
}