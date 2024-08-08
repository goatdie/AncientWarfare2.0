using System.Collections.Generic;
using AncientWarfare.Core.Profession;

namespace AncientWarfare.Core.Additions
{
    public class ActorAdditionData
    {
        public string                                clan_name   = "";
        public string                                family_name = "";
        public HashSet<string>                       Forces      = new();
        public Dictionary<string, NewProfessionData> ProfessionDatas;
        public HashSet<string>                       TechsOwned;
    }
}