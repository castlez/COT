using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Assets.Enemies;

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
                cardText = "Deal 1d6+1 slashing damage",  // these support \n!!!
                cardType = CardType.ATTACK,
                action = delegate (object target)
                {
                    EnemyBase t = (EnemyBase)target;
                    var damage = CardBase.RollDamage(1, 6, 1);
                    t.TakeDamage(damage);
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
                cardPool["Axe Throw"],
                cardPool["Axe Throw"],
                cardPool["Axe Throw"],
                cardPool["Axe Throw"],
                cardPool["Axe Throw"],
            };
        }
    }
}
