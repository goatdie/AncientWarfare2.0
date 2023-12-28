using HarmonyLib;
namespace Figurebox.test;

internal class ActorTest
{
  [HarmonyPrefix]
  [HarmonyPatch(typeof(Actor), nameof(Actor.setStatsDirty))]
  public static bool CheckBatchNull(Actor __instance)
  {
    if (__instance.batch == null)
    {
      Main.LogWarning($"Actor {__instance.data.id}({__instance.data.asset_id}) batch is null!");
    }
    else if (__instance.batch.c_stats_dirty == null)
    {
      Main.LogWarning($"Actor {__instance.data.id}({__instance.data.asset_id}) batch({__instance.batch.batch_id})'s c_stats_dirty is null!");
    }
    return true;
  }
}