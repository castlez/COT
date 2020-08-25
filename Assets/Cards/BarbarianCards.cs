using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Assets.Enemies;
using Assets.PlayersClasses;

namespace Assets.Cards
{
    public class BarbarianCards : CardHandlerBase
    {
        public Dictionary<string, CardBase> cardPool;
        public List<CardBase> startingDeck;

        public BarbarianCards()
        {
            cardPool = new Dictionary<string, CardBase>();
            cardPool.Add("Axe Throw", new CardBase()
            {
                name = "Axe Swing",
                cost = 1,
                cardText = "Deal 6 slashing damage",  // these support \n!!!
                cardType = CardTypes.ATTACK,
                targetType = TargetTypes.ENEMY,
                action = delegate (object target)
                {
                    EnemyBase t = (EnemyBase)target;
                    t.TakeDamage(6);
                }
            });
            cardPool.Add("Drop Shoulder", new CardBase()
            {
                name = "Drop Shoulder",
                cost = 1,
                cardText = "Gain 5 physical armor",  // these support \n!!!
                cardType = CardTypes.SKILLS,
                targetType = TargetTypes.SELF,
                action = delegate (object target)
                {
                    PlayerClassBase t = (PlayerClassBase)target;
                    t.gainArmour(DamageTypes.PHYSICAL, 5);
                }
            });
        }

        public List<CardBase> GetStartingDeck()
        {
            return new List<CardBase>() {
                cardPool["Axe Throw"],
                cardPool["Axe Throw"],
                cardPool["Axe Throw"],
                cardPool["Axe Throw"],
                cardPool["Axe Throw"],
                cardPool["Drop Shoulder"],
                cardPool["Drop Shoulder"],
                cardPool["Drop Shoulder"],
                cardPool["Drop Shoulder"],
                cardPool["Drop Shoulder"],
            };
        }
    }
}
