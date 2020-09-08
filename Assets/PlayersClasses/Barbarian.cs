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
        public Barbarian(string ctrl)
        {
            classDescription = "A powerful warrior, the\n" +
                               "barbarian uses physical\n" +
                               "damage to brutalize their\n" +
                               "enemy, revels in the pain\n" +
                               "that comes with combat,\n" +
                               "and can protect allies\n" +
                               "from physical harm.";
            ctrlNum = ctrl;
            maxHp = 80;
            cardHandler = new BarbarianCards();
            deck = cardHandler.GetStartingDeck();
            hand = new List<CardBase>();

            resourceName = "Action Points";
            maxResource = 3;
            currentResource = 3;
            regensAtStartOfTurn = true;
            takenFirstTurn = false;

            statusPrefabs = new List<GameObject>();
        }

        public override void Init(GameObject statusPrefTemp)
        {
            BaseInit(statusPrefTemp);
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
