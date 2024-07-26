using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AncientWarfare.Core
{
    public class Storage
    {
        class ResourceSlot
        {
            public int count;
            public string id;
        }
        private Dictionary<string, ResourceSlot> resource_slots = new();
        private Dictionary<ResType, List<ResourceSlot>> typed_resource_slots = new();
        public void Store(string resource_id, int count)
        {
            if (resource_slots.ContainsKey(resource_id))
            {
                resource_slots[resource_id].count += count;
            }
            else
            {
                var slot = new ResourceSlot { count = count, id = resource_id };
                resource_slots[resource_id] = slot;
                var asset = AssetManager.resources.get(resource_id);
                if (asset != null)
                {
                    if (!typed_resource_slots.TryGetValue(asset.type, out List<ResourceSlot> slots))
                    {
                        slots = new List<ResourceSlot>();
                        typed_resource_slots[asset.type] = slots;
                    }
                    slots.Add(slot);
                }
            }
        }
        public bool Take(string resource_id, int count)
        {
            if (resource_slots.TryGetValue(resource_id, out var slot) && slot.count >= count)
            {
                slot.count -= count;
                return true;
            }
            return false;
        }
        public ResourceAsset TakeFood(string prefer_food)
        {
            if (typed_resource_slots.TryGetValue(ResType.Food, out var slots))
            {
                if (slots.Count == 0) return null;
                if (!string.IsNullOrEmpty(prefer_food) && resource_slots.TryGetValue(prefer_food, out var slot) && slot.count > 0)
                {
                    slot.count--;
                    return AssetManager.resources.get(prefer_food);
                }
                return AssetManager.resources.get(slots.GetRandom().id);
            }
            return null;
        }
        public void CleanEmptySlots()
        {
            foreach(var list in typed_resource_slots.Values)
            {
                list.FindAll(x => x.count <= 0).ForEach(x => { list.Remove(x); resource_slots.Remove(x.id); });
            }
        }
    }
}
