using AncientWarfare.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AncientWarfare.Core
{
    public class TechLibrary : AW_AssetLibrary<TechAsset, TechLibrary>, IManager
    {
        public override void init()
        {
            base.init();
        }

        public void Initialize()
        {
            init();
            id = "aw_techs";
        }
    }
}
