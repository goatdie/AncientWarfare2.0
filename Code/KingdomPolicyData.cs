using System;
using System.Collections.Generic;

namespace Figurebox;
class KingdomPolicyData : Kingdom
{
 public int p_progress = 100;
 public double p_timestamp_start;
 public double p_timestamp_done;
 public enum PolicyStatus { Planned, InProgress, Completed }
 public PolicyStatus p_status;

}