using Figurebox.constants;
using UnityEngine;
namespace Figurebox.Utils.extensions;

public static class ObjectPoolGenericMonoExtension
{
    public static void InactiveObj<T>(this ObjectPoolGenericMono<T> pPool, T obj) where T : MonoBehaviour
    {
        if (pPool._elements_inactive.Contains(obj) || !pPool._elements_total.Contains(obj))
        {
            if (DebugConst.LOG_ALL_EXCEPTION) Main.LogWarning($"[ObjectGenericMonoPoolExtension] ReturnToPool: {obj.name} is already in inactive pool({pPool._elements_inactive.Contains(obj)}) or not in pool({!pPool._elements_total.Contains(obj)})", true);
            return;
        }
        obj.gameObject.SetActive(false);
        pPool._elements_inactive.Push(obj);
    }
}