﻿using System.Collections.Generic;
using System.Linq;

namespace AncientWarfare.Core
{
    public class Storage
    {
        private readonly Dictionary<string, ResourceContainer> resource_slots = new();
        private readonly Dictionary<ResType, List<ResourceContainer>> typed_resource_slots = new();
        private          int _curr_amount;
        private          bool _curr_amount_dirty = true;
        private          int _size = -1;
        public           int CurrAmount => _curr_amount_dirty ? UpdateCurrAmount() : _curr_amount;
        public           int Size => _size < 0 ? int.MaxValue : _size;

        public bool IsFull()
        {
            return _size >= 0 && CurrAmount >= _size;
        }

        public void SetSize(int size)
        {
            _size = size;
        }

        private int UpdateCurrAmount()
        {
            _curr_amount = resource_slots.Sum(x => x.Value.amount);
            _curr_amount_dirty = false;
            return _curr_amount;
        }

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
            if (resource_slots.TryGetValue(resource_id, out ResourceContainer resource_slot))
            {
                resource_slot.amount += count;
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

            _curr_amount_dirty = true;
        }

        public bool Take(string resource_id, int count)
        {
            if (resource_slots.TryGetValue(resource_id, out var slot) && slot.amount >= count)
            {
                slot.amount -= count;
                _curr_amount_dirty = true;
                return true;
            }

            return false;
        }

        public bool HasResourceForConstruct(ConstructionCost cost)
        {
            if (cost.gold          > 0 && cost.gold          > GetCount(SR.gold)) return false;
            if (cost.stone         > 0 && cost.stone         > GetCount(SR.stone)) return false;
            if (cost.wood          > 0 && cost.wood          > GetCount(SR.wood)) return false;
            if (cost.common_metals > 0 && cost.common_metals > GetCount(SR.common_metals)) return false;
            return true;
        }

        public ResourceAsset TakeFood(string prefer_food)
        {
            var first_check_type = ResType.Food;
            if (!string.IsNullOrEmpty(prefer_food)) first_check_type = AssetManager.resources.get(prefer_food).type;

            ResourceAsset food = take_one_type(first_check_type);
            if (food != null) return food;
            if (first_check_type != ResType.Food)
                food = take_one_type(ResType.Food);
            else
                food = take_one_type(ResType.IngredientFood);

            return food;

            ResourceAsset take_one_type(ResType res_type)
            {
                CleanEmptySlots(res_type);
                if (typed_resource_slots.TryGetValue(res_type, out var slots))
                {
                    if (slots.Count == 0) return null;
                    _curr_amount_dirty = true;

                    if (string.IsNullOrEmpty(prefer_food) || !
                            resource_slots.TryGetValue(prefer_food, out ResourceContainer food_to_take))
                        food_to_take = slots.GetRandom();

                    food_to_take.amount--;


                    return AssetManager.resources.get(food_to_take.id);
                }

                return null;
            }
        }

        public void CleanEmptySlots(ResType type)
        {
            if (typed_resource_slots.TryGetValue(type, out var slots))
                slots.FindAll(x => x.amount <= 0).ForEach(x =>
                {
                    slots.Remove(x);
                    resource_slots.Remove(x.id);
                });
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