namespace Figurebox;

public class KingdomPolicyData : BaseSystemData
{
    public enum PolicyStatus { InPlanning, InProgress, Completed }
    public string current_policy_id = "";
    public string current_state_id = "";
    public int p_progress = 100;
    public PolicyStatus p_status;
    public double p_timestamp_done;
    public double p_timestamp_start;
}