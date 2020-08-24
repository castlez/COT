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

        public abstract void TakeDamage(int amount);
    }
}
