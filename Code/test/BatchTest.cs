using System.Collections.Generic;
using HarmonyLib;
namespace Figurebox.test;

internal class BatchTest<T> where T : class
{
  internal static List<string> jobs = new();
  public static void SelfPatch()
  {
    Harmony harmony = new($"AW2.BatchTest.{typeof(T).Name}");
    harmony.Patch(typeof(Batch<T>).GetMethod("updateJobsPost"),
      new HarmonyMethod(typeof(BatchTest<T>), nameof(batchUpdateJobsPost)));

    harmony.Patch(typeof(Batch<T>).GetMethod("runUpdater"),
      postfix: new HarmonyMethod(typeof(BatchTest<T>), nameof(batchRunUpdaterRecord)));
  }
  private static bool batchUpdateJobsPost()
  {
    jobs.Clear();
    return true;
  }
  private static void batchRunUpdaterRecord(Job<T> pObj)
  {
    jobs.Add(pObj.id);
  }
}