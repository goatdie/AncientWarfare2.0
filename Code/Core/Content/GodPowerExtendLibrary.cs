using AncientWarfare.Abstracts;
using AncientWarfare.Const;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AncientWarfare.Core.Content
{
    public class GodPowerExtendLibrary : ExtendedLibrary<GodPower>, IManager
    {
        public static readonly GodPower tribe_zones;
        public void Initialize()
        {
        }

        protected override void init()
        {
            add(new() { id = nameof(tribe_zones) });
            t.name = nameof(tribe_zones);
            t.unselectWhenWindow = true;
            t.map_modes_switch = true;
            t.toggle_name = ToggleNames.map_tribe_zones;
            t.toggle_action = _ => MapModes.Manager.SetAllDirty();
        }
    }
}
