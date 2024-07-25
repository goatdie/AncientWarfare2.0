using AncientWarfare.Abstracts;
using AncientWarfare.Const;

#if 一米_中文名
using Chinese_Name;
#endif
using NeoModLoader.General;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AncientWarfare.NameGenerators
{
    internal class Manager : IManager
    {
        public void Initialize()
        {
#if 一米_中文名

#else
            return;
#endif
            WordLibraryManager.SubmitDirectoryToLoad(CPaths.WordLibrariesPath);

            //CN_NameGeneratorLibrary.Submit(new ActorNameGenerator());
        }
    }
}
