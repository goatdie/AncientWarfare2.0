using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ai.behaviours;
using Figurebox.core;
using Figurebox.Utils.MoH;
using UnityEngine;

namespace Figurebox.ai;
public class KingdomBehCheckMOH : BehaviourActionKingdom
{
    public override BehResult execute(Kingdom pKingdom)
    {
        AW_Kingdom awKingdom = pKingdom as AW_Kingdom;

        if (!MoHTools.ExistMoHKingdom)
        {
            if (checkMOHcondition(awKingdom))
            {
                //Debug.Log("天命设置完毕");
                MoHTools.SetMoHKingdom(awKingdom);

            }
            return BehResult.Continue;
        }
        return BehResult.Continue;



    }
    private bool checkMOHcondition(AW_Kingdom pKingdom)
    {
       // Debug.Log("天命寻找中");

        if (pKingdom.king != null && pKingdom.king.hasTrait("first"))
        {
            if (pKingdom.isSupreme())
            {
                return true;
            }

        }

        return false;
    }



}