using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AncientWarfare.Core.Force
{
    public interface IHasMember
    {
        public void RemoveMemberOneside(string actor_id);
        public void AddMemberOneside(Actor actor);
    }
}
