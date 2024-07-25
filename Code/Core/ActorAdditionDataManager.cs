using AncientWarfare.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AncientWarfare.Core
{
    internal class ActorAdditionDataManager : AdditionDataManager<ActorAdditionData>
    {
        public static ActorAdditionData Get(string id)
        {
            return _data.TryGetValue(id, out var data) ? data : _data[id] = new ActorAdditionData();
        }
    }
}
