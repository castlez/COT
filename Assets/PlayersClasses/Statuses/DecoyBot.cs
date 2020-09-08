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
    class DecoyBot : StatusBase
    {
        private int Hp;
        public DecoyBot()
        {
            name = "DecoyBot";
            Hp = 15;
            effectTimes = new List<StatusEffectTimes>() { StatusEffectTimes.ONTAKEDAMAGE};
        }
        public override int apply(object target, StatusEffectTimes sType)
        {
            if (sType == StatusEffectTimes.ONTAKEDAMAGE)
            {
                int baseDamage = (int)target;
                int trueDamage = baseDamage;
                if(Hp - baseDamage <= 0)
                {
                    trueDamage -= Hp;
                    duration = 0;
                }
                else
                {
                    Hp -= trueDamage;
                    trueDamage = 0;
                }
                Debug.Log($"Decoy blocked {baseDamage-trueDamage} damage! ({baseDamage} - {trueDamage})");
                return trueDamage;
            }
            return (int)target;
            
        }
        public override Sprite GetSprite()
        {
            try
            {
                return Resources.Load<Sprite>("Images/DecoyBot"); ;

            }
            catch (Exception)
            {
                Debug.Log("Failed to load sprite!");
                return null;
            }
        }
    }
}
