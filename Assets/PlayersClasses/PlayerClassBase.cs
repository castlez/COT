﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Cards;
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
        public string playerNum;

        // general
        public abstract void Init();
        public abstract Sprite GetSprite();

        public void SetTurnIndicator(bool myTurn)
        {
            Debug.Log($"player {playerNum} setting turn ind to {myTurn}");
            GameObject me = GameObject.Find($"Player{playerNum}");
            GameObject cvs = me.transform.Find("Canvas").gameObject;
            GameObject turn = cvs.transform.Find("TurnInd").gameObject;
            turn.GetComponent<SpriteRenderer>().enabled = myTurn;
        }

        public void StartTurn()
        {
            GameObject ui = GameObject.Find("UI").gameObject;
            GameObject rMax = ui.transform.Find("rMax").gameObject;
            SetTurnIndicator(true);

            // populate resources
            rMax.GetComponent<TextMesh>().text = maxResource.ToString();
            if (regensAtStartOfTurn)
            {
                setResource(maxResource);
            }

            GameObject physArm = ui.transform.Find("PhysicalArmour").gameObject;
            GameObject armAmount = physArm.transform.Find("armAmount").gameObject;

            if (armours.ContainsKey(DamageTypes.PHYSICAL))
            {
                physArm.GetComponent<SpriteRenderer>().enabled = true;
                armAmount.GetComponent<MeshRenderer>().enabled = true;

                armAmount.GetComponent<TextMesh>().text = armours[DamageTypes.PHYSICAL].ToString();
            }
            else
            {
                physArm.GetComponent<SpriteRenderer>().enabled = false;
                armAmount.GetComponent<MeshRenderer>().enabled = false;
            }
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

        // card functions / actions
        public void drawCard()
        {
            if (hand.Count + 1 <= Meta.MAX_HAND_SIZE)
            {
                if (curDeck.Count == 0)
                {
                    List<CardBase> shuffled = new List<CardBase>(grave);
                    System.Random rnd = new System.Random();
                    shuffled = shuffled.Select(x => new { value = x, order = rnd.Next() })
                                       .OrderBy(x => x.order).Select(x => x.value).ToList();
                    curDeck = new Stack<CardBase>(shuffled);
                }
                hand.Add(curDeck.Pop());
            }
        }
        public void playCard(int cardIndex, object target)
        {
            // to play a card, spend the resource, call the action, remove from hand, add to grave
            CardBase toPlay = hand[cardIndex];
            if (toPlay.cost <= currentResource)
            {
                spendResource(toPlay.cost);
                toPlay.action(target);
                hand.RemoveAt(cardIndex);
                grave.Add(toPlay);
            }
        }

        public void BaseInit()
        {
            armours = new Dictionary<DamageTypes, int>();

            // shuffle the deck and make curDeck a stack of shuffled cards
            List<CardBase> shuffled = new List<CardBase>(deck);
            System.Random rnd = new System.Random();
            shuffled = shuffled.Select(x => new { value = x, order = rnd.Next() })
                               .OrderBy(x => x.order).Select(x => x.value).ToList();
            curDeck = new Stack<CardBase>(shuffled);
            grave = new List<CardBase>();

            Debug.Log($"Trying to get sprite for player {playerNum}");
            GameObject pObj = GameObject.Find("Player" + playerNum);
            pObj.GetComponent<SpriteRenderer>().sprite = GetSprite();
            Debug.Log($"Got sprite for player {playerNum}");

        }

        public void spendResource(int amount)
        {
            currentResource -= amount;
            GameObject plr = GameObject.Find($"UI").gameObject;
            GameObject rCur = plr.transform.Find("rCur").gameObject;
            rCur.GetComponent<TextMesh>().text = currentResource.ToString();
        }

        public void setResource(int value)
        {
            currentResource = value;
            GameObject plr = GameObject.Find($"UI").gameObject;
            GameObject rCur = plr.transform.Find("rCur").gameObject;
            rCur.GetComponent<TextMesh>().text = currentResource.ToString();
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

            GameObject ui = GameObject.Find($"UI").gameObject;
            GameObject physArm = ui.transform.Find("PhysicalArmour").gameObject;
            GameObject armAmount = physArm.transform.Find("armAmount").gameObject;
            
            physArm.GetComponent<SpriteRenderer>().enabled = true;
            armAmount.GetComponent<MeshRenderer>().enabled = true;

            armAmount.GetComponent<TextMesh>().text = armours[kind].ToString();
        }

        public void TakeDamage(int amount, DamageTypes dType)
        {
            int damageTaken = amount;
            if (armours.ContainsKey(dType))
            {
                // if we have armour, reduce it by the amount of damage taken, 
                // if damage still remains, lose health
                // TODO move this to SetArmour function or some shit
                int result = armours[dType] - amount;
                if (result < 1)
                {
                    armours.Remove(dType);
                    GameObject ui = GameObject.Find($"UI");
                    GameObject physArm = ui.transform.Find("PhysicalArmour").gameObject;
                    physArm.GetComponent<SpriteRenderer>().enabled = false;
                    physArm.transform.Find("armAmount").GetComponent<MeshRenderer>().enabled = false;
                    damageTaken -= Math.Abs(result);
                }
                else
                {
                    armours[dType] -= amount;
                    GameObject ui = GameObject.Find($"UI").gameObject;
                    GameObject physArm = ui.transform.Find("PhysicalArmour").gameObject;
                    GameObject armAmount = physArm.transform.Find("armAmount").gameObject;

                    armAmount.GetComponent<TextMesh>().text = armours[dType].ToString();
                    return;
                }
            }
            Hp -= damageTaken;
            GameObject me = GameObject.Find($"Player{playerNum}");
            GameObject cvs = me.transform.Find("Canvas").gameObject;
            GameObject hpobj = cvs.transform.Find("hpbar").gameObject;
            if (Hp > 0)
            {
                float newlifeperc = (float)Hp / maxHp;
                hpobj.GetComponent<Image>().fillAmount = newlifeperc;
            }
            else
            {
                // DIE
                me.GetComponent<SpriteRenderer>().enabled = false;
                cvs.GetComponent<Canvas>().enabled = false;
            }
        }

    }
}
