using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Assets.Cards;
using Assets.PlayersClasses.Statuses;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.PlayersClasses
{
    public abstract class PlayerClassBase
    {
        // status
        public int maxHp;
        public int Hp;
        public bool takenFirstTurn;
        public Dictionary<DamageTypes, int> armours;
        public List<StatusBase> statuses;
        public GameObject statusPrefabTemplate;
        public List<GameObject> statusPrefabs;
        public float currentStatusXPos;


        // info
        public string classDescription;
        public bool spawned = false;
        public bool myTurn = false;

        // cards
        public CardHandlerBase cardHandler;
        public List<CardBase> deck;
        public Stack<CardBase> curDeck;
        public List<CardBase> hand;
        public List<CardBase> grave;
        
        // resources
        public string resourceName;
        public int maxResource;
        public int currentResource;
        public bool regensAtStartOfTurn;
           
        // players
        public string ctrlNum;  // which controller this player is using
        public string pNum;  // which player this is
        // general
        public abstract void Init(GameObject stprefobj);
        public abstract Sprite GetSprite();

        public int getModdedDamage(int baseAmount, StatusEffectTimes effectTime)
        {
            // check if any status increase damage done
            // TODO apply damage type
            if (statuses == null)
            {
                return baseAmount;  // SHOULD only hit this in character select
            }
            int damMod = baseAmount;
            foreach (StatusBase stat in statuses)
            {
                if (stat.effectTimes.Contains(effectTime))
                {
                    damMod = stat.apply(damMod, StatusEffectTimes.ONDAMAGE);
                }
            }
            return damMod;
        }

        public void SetTurnIndicator(bool isTurnPlayer)
        {
            myTurn = isTurnPlayer;
        }

        public void UpdateStatuses()
        {
            if (statuses.Count == 0)
            {
                // need to remove them too
            }
            else
            {
                List<int> toRemove = new List<int>();
                for (int i = 0;i < statuses.Count;i++)
                {
                    if (statuses[i].duration != 0)
                        statuses[i].duration -= 1;
                    if (statuses[i].duration == 0)
                    {
                        statuses.RemoveAt(i);
                    }
                }
            }
        }

        public void AddStatus(StatusBase st)
        {
            if ((CheckStatus(st.name) && st.stackable) ||
                (!CheckStatus(st.name)))
            {
                statuses.Add(st);
            }
        }

        public void RemoveStatus(Func<StatusBase, bool> check)
        {
            if (statuses.Count == 0)
            {
                return;
            }
            for (int i = 0; i < statuses.Count; i++)
            {
                if (check(statuses[i]))
                {
                    statuses.RemoveAt(i);
                    return;
                }
            }
        }

        public bool CheckStatus(string name)
        {
            foreach (StatusBase status in statuses)
            {
                if (status.name == name)
                {
                    return true;
                }
            }
            return false;
        }
        public void StartTurn()
        {
            myTurn = true;
            if (regensAtStartOfTurn)
            {
                setResource(maxResource);
            }
            UpdateStatuses();
        }

        public void DrawPhase()
        {
            if (regensAtStartOfTurn)
            {
                setResource(maxResource);
            }
            for (int i = 0; i < Meta.STARTING_HAND_SIZE; i++)
            {
                drawCard();
            }
        }

        public void EndPhase()
        {
            // dump hand in grave, empty hand
            foreach(CardBase h in hand)
            {
                grave.Add(h);
            }
            hand = new List<CardBase>();

            // turn off your turn indicator
            SetTurnIndicator(false);
        }

        public void SetTargetted(bool targetted)
        {
            GameObject cvs = GameObject.Find("Canvas").gameObject;
            GameObject me = cvs.transform.Find($"Player{pNum}").gameObject;
            GameObject targInd = me.transform.Find("TargetInd").gameObject;
            targInd.GetComponent<SpriteRenderer>().enabled = targetted;
        }

        // card functions / actions
        public void drawCard()
        {
            if (hand.Count + 1 <= Meta.MAX_HAND_SIZE)
            {
                if (curDeck.Count == 0)
                {
                    List<CardBase> shuffled = grave.OrderBy(a => Guid.NewGuid()).ToList();
                    curDeck = new Stack<CardBase>(shuffled);
                    grave = new List<CardBase>();
                }
                hand.Add(curDeck.Pop());
            }
        }
        public bool playCard(int cardIndex, object target)
        {
            // to play a card, spend the resource, call the action, remove from hand, add to grave
            CardBase toPlay = hand[cardIndex];
            if (canPlayCardAtIndex(cardIndex))
            {
                spendResource(toPlay.cost);
                toPlay.action(target, this);
                hand.RemoveAt(cardIndex);
                if (!toPlay.isOneShot)  // exhaust
                {
                    grave.Add(toPlay);
                }
                return true;
            }
            return false;
        }

        public bool canPlayCardAtIndex(int index)
        {
            if (index < hand.Count)
            {
                return hand[index].cost <= currentResource;
            }
            return false;
        }

        public void BaseInit(GameObject stPrefTemp)
        {
            statusPrefabTemplate = stPrefTemp;
            armours = new Dictionary<DamageTypes, int>();
            if (!spawned)
            {
                Hp = maxHp;
                spawned = true;
            }

            // shuffle the deck and make curDeck a stack of shuffled cards
            // also init grave and hand
            List<CardBase> shuffled = deck.OrderBy(a => Guid.NewGuid()).ToList();
            curDeck = new Stack<CardBase>(shuffled);
            grave = new List<CardBase>();
            hand = new List<CardBase>();
            statuses = new List<StatusBase>();

            // Get Sprite
            GameObject pObj = GameObject.Find("Player" + pNum);
            pObj.GetComponent<SpriteRenderer>().sprite = GetSprite();
        }

        public void spendResource(int amount)
        {
            currentResource -= amount;
        }

        public void setResource(int value)
        {
            currentResource = value;
        }

        public void gainArmour(DamageTypes kind, int amount)
        {
            if (!armours.ContainsKey(kind))
            {
                armours.Add(kind, amount);
            }
            else
            {
                armours[kind] += amount;
            }
        }

        public void TakeDamage(int amount, DamageTypes dType)
        {
            int damageTaken = amount;

            // check if any status increase damage taken
            foreach (StatusBase stat in statuses)
            {
                if (stat.effectTimes.Contains(StatusEffectTimes.ONTAKEDAMAGE))
                {
                    damageTaken = stat.apply(damageTaken, StatusEffectTimes.ONTAKEDAMAGE);
                }
            }
            if (armours.ContainsKey(dType))
            {
                // if we have armour, reduce it by the amount of damage taken, 
                // if damage still remains, lose health
                // TODO move this to SetArmour function or some shit
                int result = armours[dType] - amount;
                if (result < 1)
                {
                    armours.Remove(dType);
                    damageTaken -= Math.Abs(result);
                }
                else
                {
                    armours[dType] -= amount;
                    return;
                }
            }
            Hp -= damageTaken;
        }

        public Dictionary<string, int> GetUniqueCardsInDeck()
        {
            Dictionary<string, int> temp = new Dictionary<string, int>();
            for (int i = 0; i < deck.Count;i++)
            {
                if (temp.ContainsKey(deck[i].name))
                {
                    temp[deck[i].name] += 1;
                }
                else
                {
                    temp.Add(deck[i].name, 1);
                }
            }
            return temp;
        }

        public CardBase GetCardInDeckByName(string name)
        {
            foreach(CardBase c in deck)
            {
                if (c.name == name)
                {
                    return c;
                }
            }
            return null;
        }
    }
}
