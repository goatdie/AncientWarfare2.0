using System.Collections.Generic;
using Figurebox.attributes;
using Figurebox.core.events;

namespace Figurebox.core;

public partial class AW_KingdomManager : KingdomManager
{
    private AW_KingdomManager(List<KingdomAsset> pKingdomAssetsToCreatedAsHidden)
    {
        _latest_hash = 0;
        dict.Clear();
        list.Clear();
        list_hidden.Clear();
        dict_hidden.Clear();
        foreach (KingdomAsset item in pKingdomAssetsToCreatedAsHidden)
        {
            newHiddenKingdom(item);
        }
    }

    public static AW_KingdomManager Instance { get; private set; }

    public override void update(float pElapsed)
    {
        base.update(pElapsed);

        if (World.world.isPaused()) return;

        foreach (var civ_kingdom in list_civs)
        {
            ((AW_Kingdom)civ_kingdom).UpdateForPolicy(pElapsed);
        }
    }

    [MethodReplace]
    public new void updateAge()
    {
        for (int i = 0; i < list_civs.Count; i++)
        {
            list_civs[i].updateAge();
        }

        CalcKingdomsPower();
        CheckMOHKingdom();
        UpdateMoHValue();
        UpdateMoHCondition();
        CheckDeclareEmpire();
    }

    private void CalcKingdomsPower()
    {
        foreach (Kingdom kingdom in list_civs) ((AW_Kingdom)kingdom).CalcPower();
    }


    internal static void init()
    {
        List<KingdomAsset> kingdom_assets_to_created_as_hidden = new();
        foreach (var hidden_kingdom in World.world.kingdoms.list_hidden)
        {
            kingdom_assets_to_created_as_hidden.Add(hidden_kingdom.asset);
        }

        World.world.kingdoms.clear();
        World.world.kingdoms = new AW_KingdomManager(kingdom_assets_to_created_as_hidden);
        World.world.list_base_managers[World.world.list_base_managers.FindIndex(x => x is KingdomManager)] =
            World.world.kingdoms;
        Instance = (AW_KingdomManager)World.world.kingdoms;
    }

    public override Kingdom loadObject(KingdomData pData)
    {
        AW_Kingdom val = new();
        val.setHash(_latest_hash++);
        val.loadData(pData);
        addObject(val);
        setupKingdom(val, true);
        return val;
    }

    public override Kingdom newObject(string pSpecialID = null)
    {
        AW_Kingdom new_kingdom = new();
        new_kingdom.setHash(_latest_hash++);
        KingdomData tdata = new();
        tdata.id = string.IsNullOrEmpty(pSpecialID) ? World.world.mapStats.getNextId(type_id) : pSpecialID;
        tdata.created_time = World.world.getCreationTime();
        new_kingdom.data = tdata;

        addObject(new_kingdom);
        return new_kingdom;
    }

    public override void removeObject(Kingdom pKingdom)
    {
        base.removeObject(pKingdom);
        EventsManager.Instance.EndKingdom(pKingdom);
    }
}