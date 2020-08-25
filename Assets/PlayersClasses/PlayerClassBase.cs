using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Cards;
using Assets.Scripts;
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
        public void DrawPhase()
        {
            if (regensAtStartOfTurn)
            {
                setResource(maxResource);
            }
            for (int i = 0; i < Settings.STARTING_HAND_SIZE; i++)
            {
                drawCard();
            }
        }

        public void EndPhase()
        {
            foreach(CardBase h in hand)
            {
                grave.Add(h);
            }
            hand = new List<CardBase>();
        }

        // card functions / actions
        public void drawCard()
        {
            if (hand.Count + 1 <= Settings.MAX_HAND_SIZE)
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
            GameObject plr = GameObject.Find($"Player{playerNum}").gameObject;
            GameObject rCur = plr.transform.Find("rCur").gameObject;
            GameObject rMax = plr.transform.Find("rMax").gameObject;

            rCur.GetComponent<TextMesh>().text = currentResource.ToString();
            rMax.GetComponent<TextMesh>().text = maxResource.ToString();

            GameObject physArm = plr.transform.Find("PhysicalArmour").gameObject;
            physArm.GetComponent<SpriteRenderer>().enabled = false;
            physArm.transform.Find("armAmount").GetComponent<MeshRenderer>().enabled = false;
            armours = new Dictionary<DamageTypes, int>();

            // shuffle the deck and make curDeck a stack of shuffled cards
            List<CardBase> shuffled = new List<CardBase>(deck);
            System.Random rnd = new System.Random();
            shuffled = shuffled.Select(x => new { value = x, order = rnd.Next() })
                               .OrderBy(x => x.order).Select(x => x.value).ToList();
            curDeck = new Stack<CardBase>(shuffled);
            grave = new List<CardBase>();

            GameObject pObj = GameObject.Find("Player" + playerNum);
            pObj.GetComponent<SpriteRenderer>().sprite = GetSprite();
        }

        public void spendResource(int amount)
        {
            currentResource -= amount;
            GameObject plr = GameObject.Find($"Player{playerNum}").gameObject;
            GameObject rCur = plr.transform.Find("rCur").gameObject;
            rCur.GetComponent<TextMesh>().text = currentResource.ToString();
        }

        public void setResource(int value)
        {
            currentResource = value;
            GameObject plr = GameObject.Find($"Player{playerNum}").gameObject;
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

            GameObject plr = GameObject.Find($"Player{playerNum}").gameObject;
            GameObject physArm = plr.transform.Find("PhysicalArmour").gameObject;
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
                int result = armours[dType] - amount;
                if (result < 1)
                {
                    armours.Remove(dType);
                    GameObject plr = GameObject.Find($"Player{playerNum}");
                    GameObject physArm = plr.transform.Find("PhysicalArmour").gameObject;
                    physArm.GetComponent<SpriteRenderer>().enabled = false;
                    physArm.transform.Find("armAmount").GetComponent<MeshRenderer>().enabled = false;
                    damageTaken -= Math.Abs(result);
                }
                else
                {
                    armours[dType] -= amount;
                    GameObject plr = GameObject.Find($"Player{playerNum}").gameObject;
                    GameObject physArm = plr.transform.Find("PhysicalArmour").gameObject;
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
