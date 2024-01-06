using NeoModLoader.General.UI.Prefabs;
using UnityEngine;

namespace Figurebox.ui.prefabs;

public class KingdomPolicyStateTooltip : APrefab<KingdomPolicyStateTooltip>
{
    private static void _init()
    {
        GameObject obj = Instantiate(Resources.Load<Tooltip>("tooltips/tooltip_normal"), Main.prefabs_library)
            .gameObject;
        obj.name = "policy_state";

        Prefab = obj.AddComponent<KingdomPolicyStateTooltip>();
    }
}