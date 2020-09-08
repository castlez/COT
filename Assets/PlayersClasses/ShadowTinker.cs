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
    public class ShadowTinker : PlayerClassBase
    {
        public ShadowTinker(string pNum) : base()
        {
            // TODO finish this class (copy of barb rn)
            classDescription = "Robots in the shadows!\n" +
                               "The Shadow Tinker is a\n" +
                               "master of subterfuge and\n" +
                               "centrifuges. Can use\n" +
                               "stealthy approaches and \n" +
                               "deploy robots to aid their\n" +
                               "allies";
            ctrlNum = pNum;
            maxHp = 65;
            cardHandler = new ShadowTinkerCards();
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
                return Resources.Load<Sprite>("Images/Silent"); ;

            }
            catch (Exception)
            {
                Debug.Log("Failed to load sprite!");
                return null;
            }
        }
    }
}
