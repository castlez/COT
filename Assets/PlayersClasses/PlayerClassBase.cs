using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Cards;
using UnityEngine;


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
        public abstract void DrawPhase();

        // card functions / actions
        public abstract void drawCard();
        public void playCard(int cardIndex, object target)
        {
            CardBase toPlay = hand[cardIndex];
            if (toPlay.cost <= currentResource)
            {
                spendResource(toPlay.cost);
                toPlay.action(target);
                hand.RemoveAt(cardIndex);
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

            // TODO add to existing armour
            armAmount.GetComponent<TextMesh>().text = armours[kind].ToString();
        }

        public abstract void TakeDamage(int amount, DamageTypes dType);

    }
}
