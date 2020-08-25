using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Assets.Enemies;

namespace Assets.Cards
{
    public class CardBase
    {
        public string name;
        public string cardText;
        public int cost;
        public CardTypes cardType;
        public TargetTypes targetType;

        // Play the card, each card can target different things, 
        // so object is the safest argument 
        public delegate void playCard(object target);

        public playCard action;

        //public static int RollDamage(int numDice, int diceType, int modifier)
        //{
        //    System.Random rnd = new System.Random();
        //    int total = 0;
        //    for (int i = 0; i < numDice; i++)
        //    {
        //        total += rnd.Next(1, diceType + 1);
        //    }
        //    total += modifier;
        //    return total;
        //}
    }
}
