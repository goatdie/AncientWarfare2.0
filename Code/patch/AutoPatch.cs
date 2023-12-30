using HarmonyLib;
namespace Figurebox.patch;

public abstract class AutoPatch
{
    protected AutoPatch()
    {
        Harmony.CreateAndPatchAll(GetType(), GetType().FullName);
    }
}