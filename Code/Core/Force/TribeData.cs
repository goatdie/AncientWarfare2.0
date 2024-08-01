using System.Collections.Generic;

namespace AncientWarfare.Core.Force
{
    public class TribeData : BaseForceData
    {
        public string          clan_name = "";
        public int             color_id  = -1;
        public HashSet<string> members   = new();
        public string          race_id   = "";
        public Storage         storage   = new();
    }
}