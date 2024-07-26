using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AncientWarfare.Core.Extensions
{
    public static partial class ActorExtension
    {
        public static bool IsValid(this Actor actor)
        {
            return actor != null && actor.isAlive();
        }
        public static ActorAdditionData GetAdditionData(this Actor actor)
        {
            return ActorAdditionDataManager.Get(actor.data.id);
        }
        public static void ConsumeFood(this Actor actor, ResourceAsset food)
        {
            if (!actor.IsValid() || food == null)
            {
                return;
            }
            int hunger_restore = food.restoreHunger;
            float health_restore = food.restoreHealth;
            if(food.id == actor.data.favoriteFood)
            {
                hunger_restore += hunger_restore / 2;
                health_restore *= 1.5f;
            }
            actor.restoreStatsFromEating(hunger_restore, health_restore);
            if (Toolbox.randomChance(food.give_chance))
            {
                if (food.give_trait != null && food.give_trait.Count > 0 && Toolbox.randomBool())
                {
                    string trait = food.give_trait.GetRandom();
                    if (trait != null)
                    {
                        actor.addTrait(trait, false);
                    }
                }
                if (food.give_status != null && food.give_status.Count > 0 && Toolbox.randomBool())
                {
                    string status = food.give_status.GetRandom();
                    if (status != null)
                    {
                        actor.addStatusEffect(status, -1f);
                    }
                }
                if (food.give_action != null && Toolbox.randomBool())
                {
                    food.give_action(food);
                }
            }



            actor.timer_action = 1.5f;
            actor.ate_last_item_id = food.id;
            actor.ate_last_time = World.world.getCurSessionTime();


            if (string.IsNullOrEmpty(actor.data.favoriteFood)) return;

            if (food.id == actor.data.favoriteFood && actor.data.mood != "happy")
            {
                actor.changeMood("happy");
                return;
            }
            if (actor.data.mood == "happy")
            {
                actor.changeMood("normal");
                return;
            }
            if (actor.data.mood == "normal")
            {
                if (Toolbox.randomChance(0.2f))
                {
                    actor.changeMood("sad");
                    return;
                }
            }
            else if (actor.data.mood == "sad" && Toolbox.randomChance(0.05f))
            {
                actor.changeMood("angry");
            }
        }
    }
}
