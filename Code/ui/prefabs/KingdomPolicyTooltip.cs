using NeoModLoader.General.UI.Prefabs;
using UnityEngine;
namespace Figurebox.prefabs;

public class KingdomPolicyTooltip : APrefab<KingdomPolicyTooltip>
{
    private static void _init()
    {
        GameObject obj = Instantiate(Resources.Load<Tooltip>("tooltips/tooltip_normal"), Main.prefabs_library).gameObject;
        obj.name = "policy";

        Prefab = obj.AddComponent<KingdomPolicyTooltip>();
    }
}