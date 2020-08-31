using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Cards;
using Assets.Enemies;
using UnityEngine;

namespace Assets.PlayersClasses.Statuses
{
    public abstract class StatusBase
    {
        // Name of the status for printing and checking
        public string name;

        // when the effect takes place
        public List<StatusEffectTimes> effectTimes;

        // cant be static in this version of C# but its static so implement it as static?
        public string description;

        // duration
        public int duration = -1;  // default is until removed

        // Apply the affect by checking phase
        public abstract int apply(object target, StatusEffectTimes sType);

        public abstract Sprite GetSprite();
    }
}
