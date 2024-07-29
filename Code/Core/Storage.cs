using System.Collections.Generic;
using System.Linq;

namespace AncientWarfare.Core
{
    public class Storage
    {
        private readonly Dictionary<string, ResourceContainer> resource_slots = new();
        private Dictionary<ResType, List<ResourceContainer>> typed_resource_slots = new();

        public Dictionary<string, int> GetResDict()
        {
            return resource_slots.Where(x => x.Value.amount > 0).ToDictionary(x => x.Key, x => x.Value.amount);
        }

        public int GetCount(string resource_id)
        {
            return resource_slots.TryGetValue(resource_id, out ResourceContainer slot) ? slot.amount : 0;
        }

        public int GetCount(ResType resource_type)
        {
            return typed_resource_slots.TryGetValue(resource_type, out var slots) ? slots.Sum(x => x.amount) : 0;
        }

        public void Store(string resource_id, int count)
        {
            if (resource_slots.ContainsKey(resource_id))
            {
                resource_slots[resource_id].amount += count;
            }
            else
            {
                var slot = new ResourceContainer { amount = count, id = resource_id };
                resource_slots[resource_id] = slot;
                var asset = AssetManager.resources.get(resource_id);
                if (asset != null)
                {
                    if (!typed_resource_slots.TryGetValue(asset.type, out var slots))
                    {
                        slots = new List<ResourceContainer>();
                        typed_resource_slots[asset.type] = slots;
                    }

                    slots.Add(slot);
                }
            }
        }

        public bool Take(string resource_id, int count)
        {
            if (resource_slots.TryGetValue(resource_id, out var slot) && slot.amount >= count)
            {
                slot.amount -= count;
                return true;
            }

            return false;
        }

        public ResourceAsset TakeFood(string prefer_food)
        {
            if (typed_resource_slots.TryGetValue(ResType.Food, out var slots))
            {
                if (slots.Count == 0) return null;
                if (!string.IsNullOrEmpty(prefer_food)                                  &&
                    resource_slots.TryGetValue(prefer_food, out ResourceContainer slot) &&
                    slot.amount > 0)
                {
                    slot.amount--;
                    return AssetManager.resources.get(prefer_food);
                }

                return AssetManager.resources.get(slots.GetRandom().id);
            }

            return null;
        }

        public void CleanEmptySlots()
        {
            foreach (var list in typed_resource_slots.Values)
            {
                list.FindAll(x => x.amount <= 0).ForEach(x =>
                {
                    list.Remove(x);
                    resource_slots.Remove(x.id);
                });
            }
        }
    }
}