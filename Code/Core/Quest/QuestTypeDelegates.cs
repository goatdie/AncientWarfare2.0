using System;
using System.Collections.Generic;
using AncientWarfare.Core.Force;
using AncientWarfare.Core.Quest.QuestSettingParams;

namespace AncientWarfare.Core.Quest;

public static class QuestTypeDelegates
{
    public static void empty_merge(QuestInst source, QuestInst repeat_meet)
    {
    }

    public static void init__typed_resource_collect(QuestInst                  quest, LowBaseForce owner,
                                                    Dictionary<string, object> setting)
    {
        var resource_type = (ResType)setting[TypedResourceCollectSettingKeys.resource_type];
        var resource_count = (int)setting[TypedResourceCollectSettingKeys.resource_count];

        quest.Data.set(TypedResourceCollectSettingKeys.resource_type,  (int)resource_type);
        quest.Data.set(TypedResourceCollectSettingKeys.resource_count, resource_count);
    }

    public static bool update__typed_resource_collect(QuestInst quest, LowBaseForce owner)
    {
        if (owner is not Tribe tribe) throw new NotImplementedException();

        quest.Data.get(TypedResourceCollectSettingKeys.resource_type,  out int res_type);
        quest.Data.get(TypedResourceCollectSettingKeys.resource_count, out int res_count);
        return tribe.Data.storage.GetCount((ResType)res_type) < res_count;
    }

    public static void init__tribe_expand_for_resource(QuestInst                  quest, LowBaseForce owner,
                                                       Dictionary<string, object> setting)
    {
        if (setting.TryGetValue(TribeExpandForResourceSettingKeys.resource_type, out var raw_resource_type))
            quest.Data.set(TribeExpandForResourceSettingKeys.resource_type, (string)raw_resource_type);

        if (setting.TryGetValue(TribeExpandForResourceSettingKeys.building_id, out var raw_building_id))
            quest.Data.set(TribeExpandForResourceSettingKeys.building_id, (string)raw_building_id);
    }

    public static bool update__tribe_expand_for_resource(QuestInst quest, LowBaseForce owner)
    {
        if (owner is not Tribe tribe) throw new NotImplementedException();

        quest.Data.get(TribeExpandForResourceSettingKeys.resource_type, out string type);
        quest.Data.get(TribeExpandForResourceSettingKeys.building_id,   out string building_id);
        foreach (TileZone zone in tribe.zones)
        {
            if (string.IsNullOrEmpty(type))
            {
                foreach (Building building in zone.buildings)
                    if (building.asset.id == building_id)
                        return false;

                continue;
            }

            HashSet<Building> search_set = null;
            if (type == SB.type_flower || type == SB.type_vegetation)
                search_set = zone.plants;
            else if (type == SB.type_tree)
                search_set = zone.trees;
            else if (type == SB.type_mineral)
                search_set = zone.minerals;
            else if (type == SB.type_fruits) search_set = zone.food;

            if (search_set       == null) throw new NotImplementedException();
            if (search_set.Count > 0) return false;
        }

        return true;
    }
}