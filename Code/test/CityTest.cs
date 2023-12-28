using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using NeoModLoader.api.attributes;
using NeoModLoader.utils;
namespace Figurebox.test;

public class CityTest
{

  private static readonly Dictionary<string, DeadParam> deadActors = new();
  [Hotfixable]
  [HarmonyPrefix]
  [HarmonyPatch(typeof(City), nameof(City.addNewUnit))]
  private static bool patchCityAddUnit(City __instance, Actor pActor)
  {
    if (!pActor.isAlive())
    {
      StringBuilder sb = new();
      sb.AppendLine($"Actor {pActor.data.id}({pActor.data.asset_id})'{pActor.getName()}' is not alive to join {__instance.data.id}");
      sb.AppendLine("All traits:");
      foreach (var trait in pActor.data.traits)
      {
        sb.AppendLine(trait);
      }
      sb.AppendLine($"Destroyed: {pActor.object_destroyed}, ScheduledToDestroy:{World.world.units._to_destroy_objects.Contains(pActor)}, Health: {pActor.data.health}, Profession: {pActor.data.profession}, UpdateDone: {pActor.update_done}");
      if (deadActors.TryGetValue(pActor.data.id, out var dead_param))
      {
        sb.AppendLine($"AliveWhenDead: {dead_param.destroy}, AttackType: {dead_param.attack_type}");
        sb.AppendLine($"Kill Stack trace: {dead_param.kill_stacktrace}");
      }
      else
      {
        sb.AppendLine("No dead param");
      }
      sb.AppendLine("Job Update Order:");
      foreach (var job_id in BatchTest<Actor>.jobs)
      {
        sb.AppendLine(job_id);
      }

      Main.LogWarning(sb.ToString());

      Main.LogWarning("Stack Trace:", true);
      return false;
    }
    return true;
  }
  [Hotfixable]
  [HarmonyPrefix]
  [HarmonyPatch(typeof(Actor), nameof(Actor.killHimself))]
  private static bool patchActorKillHimself(Actor __instance, bool pDestroy, AttackType pType)
  {
    if (!deadActors.TryGetValue(__instance.data.id, out var param))
    {
      param = new DeadParam();
      param.kill_stacktrace = "none";
    }
    param.destroy &= __instance.isAlive();
    param.attack_type = pType;
    param.kill_stacktrace = OtherUtils.GetStackTrace(1);
    deadActors[__instance.data.id] = param;
    return true;
  }

  private struct DeadParam
  {
    public bool destroy;
    public AttackType attack_type;
    public string kill_stacktrace;
  }
}