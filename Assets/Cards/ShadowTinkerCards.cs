using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Assets.Enemies;
using Assets.PlayersClasses;
using System.Linq;
using Assets.PlayersClasses.Statuses;

namespace Assets.Cards
{
    public class ShadowTinkerCards : CardHandlerBase
    {
        public Dictionary<string, CardBase> cardPool;
        public List<CardBase> startingDeck;

        public ShadowTinkerCards()
        {
            cardPool = new Dictionary<string, CardBase>();
            string cardName = "";

            // Common
            cardName = "Stab";
            cardPool.Add("Stab", new CardBase()
            {
                name = cardName,
                cost = 1,
                cardText = delegate (PlayerClassBase caster)
                {
                    int damMod = caster.getModdedDamage(this.GetCard("Stab").baseDamage, StatusEffectTimes.ONDAMAGE);
                    return $"Deals {damMod} piercing damage";
                },
                cardType = CardTypes.ATTACK,
                targetType = TargetTypes.ENEMY,
                baseDamage = 4,
                action = delegate (object targetObj, PlayerClassBase caster)
                {
                    EnemyBase target = (EnemyBase)targetObj;

                    int damMod = caster.getModdedDamage(this.GetCard("Stab").baseDamage, StatusEffectTimes.ONDAMAGE);
                    target.TakeDamage(damMod, DamageTypes.PHYSICAL);
                    Debug.Log($"Stab hit for {damMod}");
                }
            });


            // Uncommon
            cardName = "Dip Into Shadow";
            cardPool.Add(cardName, new CardBase()
            {
                name = cardName,
                cost = 2,
                cardText = delegate (PlayerClassBase caster)
                {
                    return "Become Stealthed for one turn\n" +
                           "(Cannot be hit by physical damage,\n" +
                           "Stab becomes a Backstab dealing 5x damage)";
                },
                cardType = CardTypes.AURA,
                targetType = TargetTypes.SELF,
                action = delegate (object target, PlayerClassBase caster)
                {
                    PlayerClassBase t = (PlayerClassBase)target;
                    t.AddStatus(new Stealth() { duration = 1});
                }
            });
        }

        public CardBase GetCard(string name)
        {
            return cardPool[name];
        }

        public List<CardBase> GetStartingDeck()
        {
            return new List<CardBase>() {
                cardPool["Stab"],
                cardPool["Stab"],
                cardPool["Stab"],
                cardPool["Stab"],
                cardPool["Stab"],
                cardPool["Dip Into Shadow"],
                cardPool["Dip Into Shadow"],
                cardPool["Dip Into Shadow"],
                cardPool["Dip Into Shadow"],
                cardPool["Dip Into Shadow"]
            };
        }
    }
}
