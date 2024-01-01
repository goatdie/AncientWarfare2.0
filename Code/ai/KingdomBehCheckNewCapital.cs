using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ai.behaviours;
using Figurebox.core;
using UnityEngine;
using System.Linq;

namespace Figurebox.ai;
public class KingdomBehCheckNewCapital : BehaviourActionKingdom
{
    public override BehResult execute(Kingdom pKingdom)
    {
        AW_Kingdom awKingdom = pKingdom as AW_Kingdom;

        City newCapital = AW_Kingdom.FindNewCapital(awKingdom);
        if (newCapital == awKingdom.capital)
        {
            return BehResult.Continue;
        }
        else
        {
            KingdomPolicyAsset policy = KingdomPolicyLibrary.Instance.get("change_capital");

            awKingdom.StartPolicy(policy, true);
        }


        return BehResult.Continue;


    }

}