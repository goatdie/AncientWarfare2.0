using System.Collections.Generic;
using Figurebox.attributes;
using Figurebox.core.events;
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
}