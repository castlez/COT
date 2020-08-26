using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Assets.Enemies;
using Assets.PlayersClasses;
using System.Linq;

namespace Assets.Cards
{
    public class BarbarianCards : CardHandlerBase
    {
        public Dictionary<string, CardBase> cardPool;
        public List<CardBase> startingDeck;

        public BarbarianCards()
        {
            cardPool = new Dictionary<string, CardBase>();

            // Common
            cardPool.Add("Axe Swing", new CardBase()
            {
                name = "Axe Swing",
                cost = 1,
                cardText = delegate (PlayerClassBase caster)
                {
                    int damMod = caster.damageModifier + this.GetCard("Axe Swing").baseDamage;
                    return $"Deals {damMod} slashing damage";
                },
                cardType = CardTypes.ATTACK,
                targetType = TargetTypes.ENEMY,
                baseDamage = 6,
                action = delegate (object targetObj, PlayerClassBase caster)
                {
                    EnemyBase target = (EnemyBase)targetObj;
                    Barbarian me = (Barbarian)caster;

                    int damMod = me.damageModifier + this.GetCard("Axe Swing").baseDamage;
                    int meDmg = (int)Mathf.Floor(damMod / 4);
                    target.TakeDamage(damMod, DamageTypes.PHYSICAL);
                    Debug.Log($"axe swing hit for {damMod}");
                }
            });
            cardPool.Add("Drop Shoulder", new CardBase()
            {
                name = "Drop Shoulder",
                cost = 1,
                cardText = delegate (PlayerClassBase caster)
                {
                    return "Gain 5 physical armor";
                },
                cardType = CardTypes.SKILLS,
                targetType = TargetTypes.SELF,
                action = delegate (object target, PlayerClassBase caster)
                {
                    PlayerClassBase t = (PlayerClassBase)target;
                    t.gainArmour(DamageTypes.PHYSICAL, 5);
                }
            });

            // Uncommon
            cardPool.Add("Enrage", new CardBase()
            {
                name = "Enrage",
                cost = 1,
                cardText = delegate (PlayerClassBase caster)
                {
                    return "Become Enraged\n(Increase physical damage by 50% and\nphysical damage taken by 25%)";
                },
                cardType = CardTypes.AURA,
                targetType = TargetTypes.SELF,
                action = delegate (object target, PlayerClassBase caster)
                {
                    PlayerClassBase t = (PlayerClassBase)target;
                    t.gainArmour(DamageTypes.PHYSICAL, 5);
                }
            });

            cardPool.Add("Wild Swing", new CardBase()
            {
                name = "Wild Swing",
                cost = 2,
                cardText = delegate (PlayerClassBase caster)
                {
                    int damMod = caster.damageModifier + this.GetCard("Wild Swing").baseDamage;
                    return $"Deals {damMod} slashing damage. \nTake a quarter as" +
                           " much physical \ndamage rounded down.";
                },
                cardType = CardTypes.ATTACK,
                targetType = TargetTypes.ENEMY,
                baseDamage = 16,
                action = delegate (object targetObj, PlayerClassBase caster)
                {
                    EnemyBase target = (EnemyBase)targetObj;
                    Barbarian me = (Barbarian)caster;

                    int damMod = me.damageModifier + this.GetCard("Wild Swing").baseDamage;
                    int meDmg = (int)Mathf.Floor(damMod / 4);
                    target.TakeDamage(damMod, DamageTypes.PHYSICAL);
                    me.TakeDamage(meDmg, DamageTypes.PHYSICAL);
                    Debug.Log($"wild swing hits for {damMod} and caster takes {meDmg}");
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
                cardPool["Axe Swing"],
                cardPool["Axe Swing"],
                cardPool["Axe Swing"],
                cardPool["Axe Swing"],
                cardPool["Axe Swing"],
                //cardPool["Wild Swing"],
                //cardPool["Wild Swing"],
                //cardPool["Wild Swing"],
                cardPool["Drop Shoulder"],
                cardPool["Drop Shoulder"],
            };
        }
    }
}
