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
        }

        public override void Init()
        {
            curDeck = new Stack<CardBase>(deck);
            Debug.Log($"curdeck size = {curDeck.Count}");
            GameObject Player1 = GameObject.Find("Player" + playerNum);
            Player1.GetComponent<SpriteRenderer>().sprite = GetSprite();
        }

        public override void drawCard()
        {
            hand.Add(curDeck.Pop());
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

        public override void TakeDamage(int amount)
        {
            Hp -= amount;
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
