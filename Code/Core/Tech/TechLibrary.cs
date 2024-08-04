using System;
using AncientWarfare.Abstracts;
using AncientWarfare.Attributes;

namespace AncientWarfare.Core.Tech
{
    [ManagerInitializeAfter(typeof(TechCategoryLibrary))]
    public class TechLibrary : AW_AssetLibrary<TechAsset, TechLibrary>, IManager
    {
        public static readonly TechAsset leather_armor;

        public void Initialize()
        {
            id = "aw_techs";
            init();
        }

        public void LoadFromTableFile(string file_path)
        {
            throw new NotImplementedException();
        }

        public override void init()
        {
        }
    }
}