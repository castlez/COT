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
    public class BarbarianCards : CardHandlerBase
    {
        public Dictionary<string, CardBase> cardPool;
        public List<CardBase> startingDeck;

        public BarbarianCards()
        {
            cardPool = new Dictionary<string, CardBase>();
            string cardName = "";

            // Common
            cardPool.Add("Axe Swing", new CardBase()
            {
                name = "Axe Swing",
                cost = 1,
                cardText = delegate (PlayerClassBase caster)
                {
                    int damMod = caster.getModdedDamage(this.GetCard("Axe Swing").baseDamage, DamageTypes.PHYSICAL);
                    return $"Deals {damMod} slashing damage";
                },
                cardType = CardTypes.ATTACK,
                targetType = TargetTypes.ENEMY,
                baseDamage = 6,
                action = delegate (object targetObj, PlayerClassBase caster)
                {
                    EnemyBase target = (EnemyBase)targetObj;

                    int damMod = caster.getModdedDamage(this.GetCard("Axe Swing").baseDamage, DamageTypes.PHYSICAL);
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
                    t.AddStatus(new Enrage());
                }
            });

            cardPool.Add("Wild Swing", new CardBase()
            {
                name = "Wild Swing",
                cost = 2,
                cardText = delegate (PlayerClassBase caster)
                {
                    int damMod = caster.getModdedDamage(this.GetCard("Wild Swing").baseDamage, DamageTypes.PHYSICAL);
                    return $"Deals {damMod} slashing damage. \nTake a quarter as" +
                           " much physical \ndamage rounded down.";
                },
                cardType = CardTypes.ATTACK,
                targetType = TargetTypes.ENEMY,
                baseDamage = 16,
                action = delegate (object targetObj, PlayerClassBase caster)
                {
                    EnemyBase target = (EnemyBase)targetObj;

                    int damMod = caster.getModdedDamage(this.GetCard("Wild Swing").baseDamage, DamageTypes.PHYSICAL);
                    int meDmg = (int)Mathf.Floor(damMod / 4);
                    target.TakeDamage(damMod, DamageTypes.PHYSICAL);
                    caster.TakeDamage(meDmg, DamageTypes.PHYSICAL);
                    Debug.Log($"wild swing hits for {damMod} and caster takes {meDmg}");
                }
            });

            cardName = "All In";
            cardPool.Add(cardName, new CardBase()
            {
                name = cardName,
                cost = 2,
                cardText = delegate (PlayerClassBase caster)
                {
                    int damMod = caster.getModdedDamage(this.GetCard(cardName).baseDamage, DamageTypes.PHYSICAL);
                    return $"Deals {damMod} crushing damage.";
                },
                cardType = CardTypes.ATTACK,
                targetType = TargetTypes.ENEMY,
                baseDamage = 16,
                action = delegate (object targetObj, PlayerClassBase caster)
                {
                    EnemyBase target = (EnemyBase)targetObj;

                    int damMod = caster.getModdedDamage(this.GetCard(cardName).baseDamage, DamageTypes.PHYSICAL);
                    target.TakeDamage(damMod, DamageTypes.PHYSICAL);
                    Debug.Log($"wild swing hits for {damMod}");

                    caster.RemoveStatus(delegate (StatusBase stat) {
                        return stat.name == "Enrage";
                    });
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
                cardPool["Drop Shoulder"],
                cardPool["Drop Shoulder"],
                cardPool["Drop Shoulder"],
                cardPool["Drop Shoulder"],
                cardPool["Enrage"],
                cardPool["All In"]
            };
        }
    }
}
