using AncientWarfare.Core.Force;
using Chinese_Name;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AncientWarfare.NameGenerators
{
    internal class TribeNameGenerator : CN_NameGeneratorAsset
    {
        public const string ID = "tribe_name";
        private const string TRIBE_CLAN_NAME_PARAM = "clan_name";
        public delegate void TribeParameterGetter(Tribe tribe, Dictionary<string, string> parameters);
        public TribeNameGenerator()
        {
            id = ID;

            parameter_getter = "default";
            templates.Add(CN_NameTemplate.Create($"${TRIBE_CLAN_NAME_PARAM}${{{WordLibraryNames.tribe_postfix}}}", 1));
        }
        public static void DefaultTribeParameterGetter(Tribe tribe, Dictionary<string, string> parameters)
        {
            var clan_name = WordLibraryManager.GetRandomWord(WordLibraryNames.clan_name);
            tribe.Data.clan_name = clan_name;
            parameters[TRIBE_CLAN_NAME_PARAM] = clan_name;
        }
    }
}
