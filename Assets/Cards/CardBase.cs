using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Assets.Enemies;
using Assets.PlayersClasses;

namespace Assets.Cards
{
    public class CardBase
    {
        public string name;
        public int cost;
        public CardTypes cardType;
        public TargetTypes targetType;
        public int baseDamage;

        // Play the card, each card can target different things, 
        // so object is the safest argument 
        public delegate void playCard(object target, PlayerClassBase castingPlayer);

        public playCard action;

        public delegate string cardDel(PlayerClassBase castingPlayer);

        public cardDel cardText;

        public CardBase()
        {
            //cardText = cardText.Replace("{}", baseDamage.ToString());
        }

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
