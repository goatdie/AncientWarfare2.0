using System;
using Figurebox.core.kingdom_policies;

namespace Figurebox.exceptions;

public class RepeatPolicyTargetStateConfiguring : Exception
{
    public RepeatPolicyTargetStateConfiguring(KingdomPolicyAsset pKingdomPolicyAsset, string pPrevTargetStateID,
        string pCurrTargetStateID) : base(
        $"重复设置'{pKingdomPolicyAsset.id}'的状态转移目标. 已有'{pPrevTargetStateID}', 欲添加'{pCurrTargetStateID}'")
    {
    }
}