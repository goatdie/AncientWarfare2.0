namespace AncientWarfare.Core.Extensions;

public static class ActorJobExtension
{
    public static void InsertTask(this   ActorJob                  job, int idx, string task_id,
                                  params BehaviourActorCondition[] conditions)
    {
        if (idx < 0) idx = job.tasks.Count - -idx % (job.tasks.Count + 1);

        var container = new TaskContainer<BehaviourActorCondition, Actor>();
        container.id = task_id;
        foreach (BehaviourActorCondition cond in conditions) container.addCondition(cond);

        job.tasks.Insert(idx, container);
    }
}