using System.Collections.Generic;
using System.Linq;
using ai.behaviours;
using Figurebox.abstracts;
using HarmonyLib;
using ReflectionUtility;
using UnityEngine;

namespace Figurebox.content
{
  class ProjectileLibrary : ExtendedLibrary<ProjectileAsset>
  {
    protected override void init()
    {
      ProjectileAsset fire_arrow = new ProjectileAsset();
      fire_arrow.id = "FireArrow";
      fire_arrow.speed = 15f;
      fire_arrow.texture = "qing";
      fire_arrow.parabolic = true;
      fire_arrow.texture_shadow = "shadow_arrow";
      fire_arrow.terraformOption = "demon_fireball";
      fire_arrow.startScale = 0.05f;
      fire_arrow.targetScale = 0.19f;
      fire_arrow.sound_impact = "event:/SFX/HIT/HitGeneric";
      fire_arrow.terraformRange = 0;
      add(fire_arrow);
    }
  }
}