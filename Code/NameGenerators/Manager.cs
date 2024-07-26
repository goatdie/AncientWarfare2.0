using AncientWarfare.Abstracts;
using AncientWarfare.Const;
using Chinese_Name;
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
            WordLibraryManager.SubmitDirectoryToLoad(CPaths.WordLibrariesPath);

            CN_NameGeneratorLibrary.Submit(new ActorNameGenerator("human_name"));
            CN_NameGeneratorLibrary.Submit(new TribeNameGenerator());

            ParameterGetters.PutActorParameterGetter("default", ActorNameGenerator.NewDefaultActorParameterGetter);
            ParameterGetters.PutCustomParameterGetter<TribeNameGenerator.TribeParameterGetter>("default", TribeNameGenerator.DefaultTribeParameterGetter);
        }
    }
}
