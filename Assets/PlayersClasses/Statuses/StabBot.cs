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
    class StabBot : StatusBase
    {
        public StabBot()
        {
            name = "StabBot";
            effectTimes = new List<StatusEffectTimes>() { StatusEffectTimes.ONDAMAGE};
        }
        public override int apply(object target, StatusEffectTimes sType)
        {
            if (sType == StatusEffectTimes.ONDAMAGE)
            {
                int baseDamage = (int)target;
                return baseDamage + 4; // 4?
            }
            return (int)target;
            
        }
        public override Sprite GetSprite()
        {
            try
            {
                return Resources.Load<Sprite>("Images/StabBot"); ;

            }
            catch (Exception)
            {
                Debug.Log("Failed to load sprite!");
                return null;
            }
        }
    }
}
