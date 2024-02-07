using Figurebox.abstracts;

namespace Figurebox.content
{
    class StatusEffectLibrary : ExtendedLibrary<StatusEffect>
    {
        protected override void init()
        {

            StatusEffect tianming0 = new StatusEffect();
            tianming0.id = "tianming0";
            tianming0.texture = "tianming0";
            tianming0.name = "status_title_tianming0";
            tianming0.animated = true;
            tianming0.animation_speed = 0.1f;
            tianming0.base_stats[S.armor] = 10;
            tianming0.base_stats[S.damage] = 60;
            tianming0.base_stats[S.knockback_reduction] = 1f;
            tianming0.duration = 5f;
            //tianming0.action = new WorldAction(StatusLibrary.ashFeverEffect),
            tianming0.description = "status_description_tianming0";
            tianming0.path_icon = "ui/effects/tianming0/moh";
            add(tianming0);
            StatusEffect tianmingm1 = new StatusEffect();
            tianmingm1.id = "tianmingm1";
            tianmingm1.texture = "tianmingm1";
            tianmingm1.name = "status_title_tianmingm1";
            tianmingm1.animated = true;
            tianmingm1.animation_speed = 0.1f;
            //tianmingm1.base_stats[S.armor] = 10;
            //tianmingm1.base_stats[S.damage] = 60;
            tianmingm1.base_stats[S.knockback_reduction] = 1f;
            tianmingm1.duration = 5f;
            //tianmingm1.action = new WorldAction(StatusEffectLib.decline),
            tianmingm1.description = "status_description_tianmingm1";
            tianmingm1.path_icon = "ui/effects/tianming0/moh-1";
            add(tianmingm1);
            
            StatusEffect mozheng = new StatusEffect();
            mozheng.id = "qing";
            mozheng.texture = "qing";
            mozheng.path_icon = "effects/qing/tile002";
            mozheng.animated = true;
            mozheng.animation_speed = 0.1f;
            mozheng.duration = 0.5f;
            add(mozheng);
        }

    }

}