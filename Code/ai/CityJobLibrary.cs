using System.Collections.Generic;
using System.Text;
using Figurebox.abstracts;
using Figurebox.content;

namespace Figurebox.ai;

public class CityJobLibrary : ExtendedLibrary<JobCityAsset>
{
    protected List<TaskContainer<BehaviourCityCondition, City>> base_city_tasks = new();

    protected override void init()
    {
        var city_tasks = AssetManager.job_city.get("city").tasks;
        foreach (var task in city_tasks) base_city_tasks.Add(task);
        base_city_tasks.Add(new TaskContainer<BehaviourCityCondition, City>()
        {
            id = "produce_noble"
        });
        base_city_tasks.Add(new TaskContainer<BehaviourCityCondition, City>()
        {
            id = "check_guard"
        });
        base_city_tasks.Add(new TaskContainer<BehaviourCityCondition, City>()
        {
            id = "check_retirement"
        });
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

        foreach (var base_task in ((CityJobLibrary)Instance).base_city_tasks) job.tasks.Add(base_task);
        AssetManager.job_city.add(job);
    }
}