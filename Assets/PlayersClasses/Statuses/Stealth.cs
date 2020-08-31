using Assets.Enemies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

using Assets.Cards;

namespace Assets.PlayersClasses.Statuses
{
    class Stealth : StatusBase
    {
        public Stealth()
        {
            name = "Stealth";
            effectTimes = new List<StatusEffectTimes>() { StatusEffectTimes.ONDAMAGE, StatusEffectTimes.ONTAKEDAMAGE };
        }
        public override int apply(object target, StatusEffectTimes sType)
        {
            if (sType == StatusEffectTimes.ONDAMAGE)
            {
                float t = float.Parse(target.ToString() + ".0");  // HAXXXXXX
                int dmgMod = (int)Math.Ceiling(t * 5);
                return dmgMod;
            }
            if (sType == StatusEffectTimes.ONTAKEDAMAGE)
            {
                return 0;
            }
            else
            {
                throw new NotImplementedException();  // shouldnt hit this if caller is doing it right
            }
        }
        public override Sprite GetSprite()
        {
            try
            {
                return Resources.Load<Sprite>("Images/Stealth"); ;

            }
            catch (Exception)
            {
                Debug.Log("Failed to load sprite!");
                return null;
            }
        }
    }
}
