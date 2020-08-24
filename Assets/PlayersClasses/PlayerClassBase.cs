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
        public int maxHp;
        public int Hp;
        public CardHandlerBase cardHandler;
        public List<CardBase> deck;
        public Stack<CardBase> curDeck;
        public List<CardBase> hand;

        public string playerNum;

        public abstract void drawCard();
        public void playCard(int cardIndex, object target)
        {
            CardBase toPlay = hand[cardIndex];
            toPlay.action(target);
            hand.RemoveAt(cardIndex);
        }
        public abstract void Init();

        public abstract Sprite GetSprite();

        public abstract void TakeDamage(int amount);
    }
}
