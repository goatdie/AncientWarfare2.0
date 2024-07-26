using AncientWarfare.Core.Extensions;
using Chinese_Name;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AncientWarfare.NameGenerators
{
    internal class ActorNameGenerator : CN_NameGeneratorAsset
    {
        private Dictionary<string, List<CN_NameTemplate>> groups = new();

        private const string BASE_GROUP = "base";
        private const string HAS_FAMILY_NAME_GROUP = "has_family_name";

        private const string FAMILY_NAME_PARAM = "family_name";
        public ActorNameGenerator(string override_id)
        {
            id = override_id;

            parameter_getter = "default";
            AddTemplate("{千字文}", BASE_GROUP);
            AddTemplate("{中文名字}{千字文}", BASE_GROUP);
            AddTemplate($"${FAMILY_NAME_PARAM}${{中文名字}}", HAS_FAMILY_NAME_GROUP);
            AddTemplate($"${FAMILY_NAME_PARAM}${{中文名字}}{{中文名字}}", HAS_FAMILY_NAME_GROUP);
        }
        public override CN_NameTemplate GetTemplate(Dictionary<string, string> pParameters = null)
        {
            if (pParameters.ContainsKey(FAMILY_NAME_PARAM) && !string.IsNullOrEmpty(pParameters[FAMILY_NAME_PARAM]))
            {
                return groups[HAS_FAMILY_NAME_GROUP].GetRandom();
            }
            return groups[BASE_GROUP].GetRandom();
        }
        private void AddTemplate(string format, string group)
        {
            templates.Add(CN_NameTemplate.Create(format, 1));
            if (!groups.ContainsKey(group))
            {
                groups[group] = new();
            }
            groups[group].Add(templates.Last());
        }
        public static void NewDefaultActorParameterGetter(Actor actor, Dictionary<string, string> parameters)
        {
            actor.data.get(Chinese_Name.constants.DataS.family_name, out var family_name, "");
            var addition_data = actor.GetAdditionData();
            if (!string.IsNullOrEmpty(addition_data.clan_name))
            {
                family_name = addition_data.clan_name;
            }
            else if (!string.IsNullOrEmpty(addition_data.family_name))
            {
                family_name = addition_data.family_name;
            }
            else if (string.IsNullOrEmpty(family_name))
            {
                var tribe = actor.GetTribe();
                if (tribe != null)
                {
                    family_name = tribe.Data.clan_name;

                    addition_data.clan_name = family_name;
                    addition_data.family_name = family_name;
                }
                /*
                family_name = WordLibraryManager.GetRandomWord(WordLibraryNames.clan_name);

                addition_data.clan_name = family_name;
                addition_data.family_name = family_name;
                */
            }
            parameters[FAMILY_NAME_PARAM] = family_name;
        }
    }
}
