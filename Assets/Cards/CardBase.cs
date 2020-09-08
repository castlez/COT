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

        // can this card appear in card rewards
        public bool isReward = true;

        // is one time use
        public bool isOneShot = false;

        public CardBase()
        {
            //cardText = cardText.Replace("{}", baseDamage.ToString());
        }
    }
}
