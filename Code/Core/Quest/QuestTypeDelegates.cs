using System;
using System.Collections.Generic;
using AncientWarfare.Core.Force;
using AncientWarfare.Core.Quest.QuestSettingParams;
using NeoModLoader.api.attributes;

namespace AncientWarfare.Core.Quest;

public static class QuestTypeDelegates
{
    public static bool empty_merge(QuestInst source, QuestInst repeat_meet)
    {
        return true;
    }

    public static void init__typed_resource_collect(QuestInst                  quest, LowBaseForce owner,
                                                    Dictionary<string, object> setting)
    {
        var resource_type = (ResType)setting[TypedResourceCollectSettingKeys.resource_type_int];
        var resource_count = (int)setting[TypedResourceCollectSettingKeys.resource_count_int];

        quest.Data.set(TypedResourceCollectSettingKeys.resource_type_int,  (int)resource_type);
        quest.Data.set(TypedResourceCollectSettingKeys.resource_count_int, resource_count);
    }

    public static bool update__typed_resource_collect(QuestInst quest, LowBaseForce owner)
    {
        if (owner is not Tribe tribe) throw new NotImplementedException();

        quest.Data.get(TypedResourceCollectSettingKeys.resource_type_int,  out int res_type);
        quest.Data.get(TypedResourceCollectSettingKeys.resource_count_int, out int res_count);
        return tribe.Data.storage.GetCount((ResType)res_type) < res_count;
    }

    public static void init__resource_collect(QuestInst quest, LowBaseForce owner, Dictionary<string, object> setting)
    {
        var resource_id = (string)setting[ResourceCollectSettingKeys.resource_id_string];
        var resource_nr = (int)setting[ResourceCollectSettingKeys.resource_target_count_int];

        quest.Data.set(ResourceCollectSettingKeys.resource_id_string,        resource_id);
        quest.Data.set(ResourceCollectSettingKeys.resource_target_count_int, resource_nr);
    }

    public static bool update__resource_collect(QuestInst quest, LowBaseForce owner)
    {
        if (owner is not Tribe tribe) throw new NotImplementedException();

        quest.Data.get(ResourceCollectSettingKeys.resource_id_string,        out string res_id);
        quest.Data.get(ResourceCollectSettingKeys.resource_target_count_int, out int res_count);
        return tribe.Data.storage.GetCount(res_id) < res_count;
    }

    public static bool merge__resource_collect(QuestInst source, QuestInst repeat_meet)
    {
        source.Data.get(ResourceCollectSettingKeys.resource_id_string, out string res_id);
        repeat_meet.Data.get(ResourceCollectSettingKeys.resource_id_string, out string res_id_meet);
        if (res_id != res_id_meet) return false;
        source.Data.get(ResourceCollectSettingKeys.resource_target_count_int, out int res_count);
        repeat_meet.Data.get(ResourceCollectSettingKeys.resource_target_count_int, out int res_count_meet);

        source.Data.set(ResourceCollectSettingKeys.resource_target_count_int, Math.Max(res_count, res_count_meet));
        return true;
    }

    public static void init__tribe_expand_for_resource(QuestInst                  quest, LowBaseForce owner,
                                                       Dictionary<string, object> setting)
    {
        if (setting.TryGetValue(TribeExpandForResourceSettingKeys.resource_type_string, out var raw_resource_type))
            quest.Data.set(TribeExpandForResourceSettingKeys.resource_type_string, (string)raw_resource_type);

        if (setting.TryGetValue(TribeExpandForResourceSettingKeys.building_id_string, out var raw_building_id))
            quest.Data.set(TribeExpandForResourceSettingKeys.building_id_string, (string)raw_building_id);
    }

    public static bool update__tribe_expand_for_resource(QuestInst quest, LowBaseForce owner)
    {
        if (owner is not Tribe tribe) throw new NotImplementedException();

        quest.Data.get(TribeExpandForResourceSettingKeys.resource_type_string, out string type);
        quest.Data.get(TribeExpandForResourceSettingKeys.building_id_string,   out string building_id);
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

    public static void init__construct_building(QuestInst quest, LowBaseForce owner, Dictionary<string, object> setting)
    {
        if (setting.TryGetValue(ConstructBuildingSettingKeys.building_key_string, out var raw_building_id))
            quest.Data.set(ConstructBuildingSettingKeys.building_key_string, (string)raw_building_id);
        if (setting.TryGetValue(ConstructBuildingSettingKeys.upgrade_building_if_possible_bool,
                                out var upgrade_building_if_possible))
            quest.Data.set(ConstructBuildingSettingKeys.upgrade_building_if_possible_bool,
                           (bool)upgrade_building_if_possible);
    }

    public static bool update__construct_building(QuestInst quest, LowBaseForce owner)
    {
        if (owner is not Tribe tribe) throw new NotImplementedException();
        quest.Data.get(ConstructBuildingSettingKeys.building_key_string, out string building_id);

        return !tribe.buildings.getSimpleList().Exists([Hotfixable](x) =>
        {
            return x.asset.id == building_id && !x.isUnderConstruction();
        });
    }
}