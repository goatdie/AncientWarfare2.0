using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReflectionUtility;

namespace Figurebox
{
    class trait_group
    {
        public static string aw2 = "aw2";
        public static void init()
        {
            ActorTraitGroupAsset aw2 = new ActorTraitGroupAsset();
            aw2.id = "aw2";
            aw2.name = "trait_group_aw2";
            aw2.color = Toolbox.makeColor("#3BAFFF", -1f);
            AssetManager.trait_groups.add(aw2);
        }
    }
}
