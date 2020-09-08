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
                isReward = false,
                action = delegate (object targetObj, PlayerClassBase caster)
                {
                    EnemyBase target = (EnemyBase)targetObj;

                    int damMod = caster.getModdedDamage(this.GetCard("Stab").baseDamage, StatusEffectTimes.ONDAMAGE);
                    target.TakeDamage(damMod, DamageTypes.PHYSICAL);
                    Debug.Log($"Stab hit for {damMod}");
                }
            });

            cardPool.Add("Side Step", new CardBase()
            {
                name = "Side Step",
                cost = 1,
                cardText = delegate (PlayerClassBase caster)
                {
                    return "Gain 5 physical armor";
                },
                cardType = CardTypes.SKILLS,
                targetType = TargetTypes.SELF,
                isReward = false,
                action = delegate (object target, PlayerClassBase caster)
                {
                    PlayerClassBase t = (PlayerClassBase)target;
                    t.gainArmour(DamageTypes.PHYSICAL, 5);
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
                           "(Cannot be hit by physical\n" +
                           "damage,\n" +
                           "Stab becomes a Backstab\n" +
                           "dealing 5x damage)\n" + 
                           "Duration: 1 Turn";

                },
                cardType = CardTypes.AURA,
                targetType = TargetTypes.SELF,
                action = delegate (object target, PlayerClassBase caster)
                {
                    PlayerClassBase t = (PlayerClassBase)target;
                    t.AddStatus(new Stealth() { duration = 1});
                }
            });

            cardName = "Deploy Stab-Bot";
            cardPool.Add(cardName, new CardBase()
            {
                name = cardName,
                cost = 2,
                cardText = delegate (PlayerClassBase caster)
                {                                      //\n <- end of line on card TODO make dynamic
                    return "Deploy a stabby robot to aid\n" +
                           "target player. Increases\n" +
                           "damage by 4?\n";

                },
                cardType = CardTypes.AURA,
                targetType = TargetTypes.PLAYER,
                isOneShot = true,
                action = delegate (object target, PlayerClassBase caster)
                {
                    PlayerClassBase t = (PlayerClassBase)target;
                    t.AddStatus(new StabBot());
                }
            });

            cardName = "Deploy Decoy-Bot";
            cardPool.Add(cardName, new CardBase()
            {
                name = cardName,
                cost = 2,
                cardText = delegate (PlayerClassBase caster)
                {                                      //\n <- end of line on card TODO make dynamic
                    return "Deploy a decoy robot to aid\n" +
                           "target player. Takes up to\n"  +
                           "15 damage for the player";

                },
                cardType = CardTypes.AURA,
                targetType = TargetTypes.PLAYER,
                isOneShot = true,
                action = delegate (object target, PlayerClassBase caster)
                {
                    PlayerClassBase t = (PlayerClassBase)target;
                    t.AddStatus(new DecoyBot());
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
                cardPool["Side Step"],
                cardPool["Side Step"],
                cardPool["Side Step"],
                cardPool["Side Step"],
                cardPool["Dip Into Shadow"],
                cardPool["Deploy Stab-Bot"]
            };
        }

        public List<CardBase> GetRewardPool(int floor)
        {
            return cardPool.Select(k=>k.Value).Where(c => c.isReward).ToList();
        }
    }
}
