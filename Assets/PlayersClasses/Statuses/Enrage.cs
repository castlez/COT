using Assets.Enemies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Assets.Cards;

namespace Assets.PlayersClasses.Statuses
{
    class Enrage : StatusBase
    {
        public Enrage()
        {
            effectTimes = new List<StatusEffectTimes>() { StatusEffectTimes.ONDAMAGE, StatusEffectTimes.ONTAKEDAMAGE };
        }
        public override int apply(object target, StatusEffectTimes sType)
        {
            if (sType == StatusEffectTimes.ONDAMAGE)
            {
                float t = float.Parse(target.ToString() + ".0");  // HAXXXXXX
                int dmgMod = (int)Math.Ceiling(t + (t * 0.5));
                return dmgMod;
            }
            if (sType == StatusEffectTimes.ONTAKEDAMAGE)
            {
                float t = float.Parse(target.ToString() + ".0");
                int dmgMod = (int)Math.Floor(t + (t * 0.25));
                return dmgMod;
            }
            else
            {
                throw new NotImplementedException();  // shouldnt hit this if caller is doing it right
            }
        }
    }
}
