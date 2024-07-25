using AncientWarfare.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AncientWarfare.Core.AI
{
    internal class Manager : IManager
    {
        public void Initialize()
        {
            _ = new ActorJobExtendLibrary();
            _ = new ActorTaskExtendLibrary();
        }
    }
}
