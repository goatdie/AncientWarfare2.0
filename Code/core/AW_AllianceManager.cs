using System.Collections.Generic;
using Figurebox.attributes;
using Figurebox.core.events;
#if 一米_中文名
using Chinese_Name;
#endif

namespace Figurebox.core;

public class AW_AllianceManager : AllianceManager
{
    internal static void init()
    {
        World.world.alliances.clear();
        World.world.alliances = new AW_AllianceManager();
        World.world.list_base_managers[World.world.list_base_managers.FindIndex(x => x is AllianceManager)] =
            World.world.wars;
    }

    [MethodReplace]
    public new Alliance newAlliance(Kingdom pKingdom, Kingdom pKingdom2)
    {
        var alliance = newObject();
        alliance.createAlliance();
        alliance.addFounders(pKingdom, pKingdom2);
        WorldLog.logAllianceCreated(alliance);
#if 一米_中文名
        if (string.IsNullOrWhiteSpace(alliance.data.name))
        {
            var generator = CN_NameGeneratorLibrary.Get("alliance_name");
            if (generator != null)
            {
                var para = new Dictionary<string, string>();

                ParameterGetters.GetAllianceParameterGetter(generator.parameter_getter)(alliance, para);

                alliance.data.name = generator.GenerateName(para);
            }
        }
#endif

        EventsManager.Instance.NewAlliance(alliance);
        return alliance;
    }

    public override Alliance loadObject(AllianceData pData)
    {
        AW_Alliance val = new();
        val.setHash(_latest_hash++);
        val.loadData(pData);
        addObject(val);
        return val;
    }

    public override Alliance newObject(string pSpecialID = null)
    {
        AW_Alliance new_war = new();
        new_war.setHash(_latest_hash++);
        AllianceData tdata = new();
        tdata.id = string.IsNullOrEmpty(pSpecialID) ? World.world.mapStats.getNextId(type_id) : pSpecialID;
        tdata.created_time = World.world.getCreationTime();
        new_war.data = tdata;

        addObject(new_war);
        return new_war;
    }

    public override void removeObject(Alliance pObject)
    {
        base.removeObject(pObject);
        EventsManager.Instance.EndAlliance(pObject);
    }
}