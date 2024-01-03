using System;
using UnityEngine;
using System.Collections.Generic;
using Figurebox.constants;
using Figurebox.core;
namespace Figurebox.Utils;
public static class ActorTools
{
    public static void ChangeCity(this Actor actor, City ncity)
    {
        actor.city?.removeCitizen(actor, false, AttackType.Other);
        ncity.addNewUnit(actor);
    }

}