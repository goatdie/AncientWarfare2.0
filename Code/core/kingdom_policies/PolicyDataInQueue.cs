using Figurebox.utils;
namespace Figurebox;

/// <summary>
///     正在等待执行的国策
/// </summary>
public class PolicyDataInQueue : IReusable
{
    public static readonly ObjectPool<PolicyDataInQueue> Pool = new(64);

    public string policy_id;
    public int progress;
    public AW_KingdomDataAddition.PolicyStatus status;
    public double timestamp_start;
    public void Setup()
    {
        timestamp_start = World.world.mapStats.worldTime;
        status = AW_KingdomDataAddition.PolicyStatus.InPlanning;
    }
    public void Recycle()
    {
    }
}