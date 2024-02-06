using System.Collections.Generic;
using Figurebox.attributes;
using Figurebox.core.events;
using Figurebox.Utils.extensions;
using Figurebox.Utils.MoH;

#if 一米_中文名
using Chinese_Name;
#endif

namespace Figurebox.core;

public class AW_WarManager : WarManager
{
    internal static void init()
    {
        World.world.wars.clear();
        World.world.wars = new AW_WarManager();
        World.world.list_base_managers[World.world.list_base_managers.FindIndex(x => x is WarManager)] =
            World.world.wars;
    }

    [MethodReplace]
    public new War newWar(Kingdom pAttacker, Kingdom pDefender, WarTypeAsset pType)
    {
        warStateChanged();
        var war = newObject();
        war.initWar(pAttacker, pDefender, pType.id);
        war.data.started_by_king = pAttacker.king?.getName();
        war.data.started_by_kingdom = pAttacker.data.name;
#if 一米_中文名
        if (pDefender != null && pDefender.getAge() <= 1) pType = WarTypeLibrary.rebellion;

        var generator = CN_NameGeneratorLibrary.Get(pType.name_template);
        if (generator != null)
        {
            var para = new Dictionary<string, string>();
            ParameterGetters.GetWarParameterGetter(generator.parameter_getter)(war, para);

            war.data.name = generator.GenerateName(para);
        }
#else
        war.data.name =
 NameGenerator.generateNameFromTemplate(pType.name_template, null, pType.kingdom_for_name_attacker ? pAttacker : pDefender);
#endif

        EventsManager.Instance.NewWar(war);
        // 检查攻击者是否是宗主国
        if (pAttacker.AW().IsSuzerain())
        {
            foreach (var vassal in pAttacker.AW().GetVassals())
            {
                if (vassal != null)
                {
                    war.joinAttackers(vassal);
                }
            }
        }

        // 如果攻击者是附庸国，则将其宗主国加入攻击方
        if (pAttacker.AW().IsVassal() && pDefender != pAttacker.AW().suzerain)
        {
            var suzerain = pAttacker.AW().suzerain;
            if (suzerain != null)
            {
                war.joinAttackers(suzerain);
                // 将宗主国的其他附庸也加入攻击方
                foreach (var otherVassal in suzerain.GetVassals())
                {
                    if (otherVassal != pAttacker.AW()) // 避免重复加入当前附庸
                    {
                        war.joinAttackers(otherVassal);
                    }
                }
            }
        }


        if (pDefender.AW().IsSuzerain() && pAttacker.AW().suzerain != pDefender.AW())
        {

            // 检查宗主国的所有附庸，确定哪些支持独立的附庸国
            foreach (var vassal in pDefender.AW().GetVassals())
            {
                if (vassal != null) // 假设存在一个标记支持独立的属性
                {
                    // 将支持独立的附庸国加入攻击方
                    war.joinDefenders(vassal);
                }
            }
        }


        if (pDefender.AW().IsVassal() && pAttacker != pDefender.AW().suzerain)
        {
            var suzerain = pDefender.AW().suzerain;
            if (suzerain != null)
            {
                war.joinDefenders(suzerain);
                // 将宗主国的其他附庸也加入防御方
                foreach (var otherVassal in suzerain.GetVassals())
                {
                    if (otherVassal != pDefender.AW()) // 避免重复加入当前附庸
                    {
                        war.joinDefenders(otherVassal);
                    }
                }
            }
        }

        return war;
    }

    public override War loadObject(WarData pData)
    {
        AW_War val = new();
        val.setHash(_latest_hash++);
        val.loadData(pData);
        addObject(val);
        return val;
    }

    public override War newObject(string pSpecialID = null)
    {
        AW_War new_war = new();
        new_war.setHash(_latest_hash++);
        WarData tdata = new();
        tdata.id = string.IsNullOrEmpty(pSpecialID) ? World.world.mapStats.getNextId(type_id) : pSpecialID;
        tdata.created_time = World.world.getCreationTime();
        new_war.data = tdata;

        addObject(new_war);
        return new_war;
    }

    public override void removeObject(War pObject)
    {
        base.removeObject(pObject);
        EventsManager.Instance.EndWar(pObject);
    }
}