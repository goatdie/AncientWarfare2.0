using AncientWarfare.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AncientWarfare.Abstracts
{
    public class AdditionDataManager<TAdditionData>
    {
        protected static Dictionary<string, TAdditionData> _data = new();
    }
}
