using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AncientWarfare.Core.Force
{
    public class SubForceManager<T, TData> where T : LowBaseForce where TData : BaseForceData
    {
        protected Dictionary<string, LowBaseForce> total_container;
        public SubForceManager(Dictionary<string, LowBaseForce> total_container)
        {
            this.total_container = total_container;
        }
    }
}
