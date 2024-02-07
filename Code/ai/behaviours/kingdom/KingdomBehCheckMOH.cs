using System.Collections.Generic;
using System.Linq;
using ai.behaviours;
using Figurebox.core;
using Figurebox.utils.MoH;

namespace Figurebox.ai.behaviours.kingdom;
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

                KingdomYearName.changeYearname(awKingdom);
                MoHTools.SetMoHKingdom(awKingdom);



            }
            return BehResult.Continue;
        }
        else
        {

        }
        return BehResult.Continue;



    }
    private bool checkMOHcondition(AW_Kingdom pKingdom)
    {
        // Debug.Log("天命寻找中");

        if (pKingdom.king != null && pKingdom.king.hasTrait("first"))
        {
            List<Kingdom> kingdomList = World.world.kingdoms.list_civs;

            // 将 Kingdom 类型的列表转换为 AW_Kingdom 类型的列表
            List<AW_Kingdom> awKingdomList = kingdomList.Select(k => k as AW_Kingdom).ToList();
            AW_Kingdom mostvaluekingdom = MoHTools.FindMostPowerfulKingdom(awKingdomList);
            if (mostvaluekingdom == pKingdom)
            {
                return true;
            }

        }

        return false;
    }




}