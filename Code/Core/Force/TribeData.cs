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
        public HashSet<string> techs = new(); // TODO: 等出现实体图书馆后换成存于建筑中
    }
}