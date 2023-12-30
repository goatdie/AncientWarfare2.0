using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ai.behaviours;
using Figurebox.core;
using UnityEngine;
using System.Linq;

namespace Figurebox.ai;
public class KingdomBehCheckHeir : BehaviourActionKingdom
{
    public override BehResult execute(Kingdom pKingdom)
    {
        AW_Kingdom awKingdom = pKingdom as AW_Kingdom;

        if (awKingdom.hasHeir())
        {
            return BehResult.Continue;
        }


        else
        {
            awKingdom.SetHeir(awKingdom.FindHeir());
        }

        return BehResult.Continue;


    }

}