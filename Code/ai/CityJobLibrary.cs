using System.Collections.Generic;
using System.Text;
using Figurebox.abstracts;

namespace Figurebox.ai;

public class CityJobLibrary : ExtendedLibrary<JobCityAsset>
{
    protected override void init()
    {
    }

    internal static string CheckAndGetCityJob(Dictionary<string, string> pPolicyState)
    {
        if (pPolicyState == null || pPolicyState.Count == 0) return "city";
        StringBuilder job_id = new();
        foreach (var key in pPolicyState.Keys)
        {
            job_id.Append(key);
            job_id.Append(":");
            job_id.Append(pPolicyState[key]);
            job_id.Append(";");
        }

        var job_id_str = job_id.ToString();
        if (!AssetManager.job_city.dict.ContainsKey(job_id_str)) CreateCityJob(job_id_str, pPolicyState);
        return job_id.ToString();
    }

    private static void CreateCityJob(string pJobIDStr, Dictionary<string, string> pPolicyState)
    {
        JobCityAsset job = new();
        job.id = pJobIDStr;

        foreach (var key in pPolicyState.Keys)
        {
            var state = KingdomPolicyStateLibrary.Instance.get(pPolicyState[key]);
            if (state.city_task_list == null || state.city_task_list.Count == 0) continue;
            foreach (var task in state.city_task_list)
            {
                job.addTask(task.id);
                job.addTask("wait1");
            }
        }

        AssetManager.job_city.add(job);
    }
}