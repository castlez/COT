using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Cards;

namespace Assets.PlayersClasses.Statuses
{
    public abstract class StatusBase
    {
        // Name of the status for printing and checking
        public string name;

        // when the effect takes place
        public StatusEffectTimes activatePhase;

        // cant be static in this version of C# but its static so implement it as static?
        public string description;

        // Apply the affect by checking phase
        public abstract bool apply();
    }
}
