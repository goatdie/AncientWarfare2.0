using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AncientWarfare.Core.Force
{
    public class TribeData : BaseForceData
    {
        public int color_id = -1;
        public string clan_name = "";
        public HashSet<string> members = new();
    }
}
