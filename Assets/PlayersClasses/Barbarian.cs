using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Cards;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.PlayersClasses
{
    public class Barbarian : PlayerClassBase
    {
        public Barbarian(string pNum)
        {
            playerNum = pNum;
            maxHp = 40;
            Hp = maxHp;
            cardHandler = new BarbarianCards();
            deck = cardHandler.GetStartingDeck();
            hand = new List<CardBase>();

            resourceName = "Action Points";
            maxResource = 3;
            currentResource = 3;
            regensAtStartOfTurn = true;
            takenFirstTurn = false;
        }

        public override void Init()
        {
            BaseInit();
        }

        public override Sprite GetSprite()
        {
            try
            {
                return Resources.Load<Sprite>("Images/Ironclad"); ;

            }
            catch (Exception)
            {
                Debug.Log("Failed to load sprite!");
                return null;
            }
        }
    }
}
