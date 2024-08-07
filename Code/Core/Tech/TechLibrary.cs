using System;
using AncientWarfare.Abstracts;
using AncientWarfare.Attributes;
using AncientWarfare.Core.Profession;

namespace AncientWarfare.Core.Tech
{
    [ManagerInitializeAfter(typeof(TechCategoryLibrary), typeof(NewProfessionLibrary))]
    public class TechLibrary : AW_AssetLibrary<TechAsset, TechLibrary>, IManager
    {
        public static readonly TechAsset hunt;
        public static readonly TechAsset dissect_animal;
        public static readonly TechAsset identify_plants;
        public static readonly TechAsset collect;
        public static readonly TechAsset collect_berry_lightly;
        public static readonly TechAsset sharp_tools;
        public static readonly TechAsset armor;
        public static readonly TechAsset leather_armor;

        public void Initialize()
        {
            id = "aw_techs";
            init();
            post_init();
        }

        public void LoadFromTableFile(string file_path)
        {
            throw new NotImplementedException();
        }

        public override void init()
        {
            add(new TechAsset(nameof(collect), 1, NewProfessionLibrary.collect));
            add(new TechAsset(nameof(hunt), 1, NewProfessionLibrary.battle, NewProfessionLibrary.collect));
            add(new TechAsset(nameof(collect_berry_lightly), 1, NewProfessionLibrary.collect));
            add(new TechAsset(nameof(dissect_animal), 3, NewProfessionLibrary.collect, NewProfessionLibrary.battle));
            add(new TechAsset(nameof(identify_plants), 1, NewProfessionLibrary.collect));
            add(new TechAsset(nameof(sharp_tools), 3, NewProfessionLibrary.battle, NewProfessionLibrary.collect,
                              NewProfessionLibrary.study));
            add(new TechAsset(nameof(armor),         3, NewProfessionLibrary.battle, NewProfessionLibrary.study));
            add(new TechAsset(nameof(leather_armor), 5, NewProfessionLibrary.craft,  NewProfessionLibrary.forge));
        }

        public override void post_init()
        {
            dissect_animal.AddPreliminaries(hunt);

            hunt.AddInspirations(sharp_tools);
            hunt.AddInspirations(dissect_animal);
            hunt.AddInspirations(armor);

            leather_armor.AddPreliminaries(armor);
        }
    }
}