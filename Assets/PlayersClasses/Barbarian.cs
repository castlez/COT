using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Cards;
using Assets.Scripts;
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

        public override void DrawPhase()
        {
            if (regensAtStartOfTurn)
            {
                setResource(maxResource);
            }
            if (takenFirstTurn)
            {
                drawCard();
            }
            else
            {
                for (int i = 0; i < Settings.STARTING_HAND_SIZE; i++)
                {
                    drawCard();
                }
                takenFirstTurn = true;
            }
        }

        public override void drawCard()
        {
            if (hand.Count + 1 <= Settings.MAX_HAND_SIZE)
            {
                hand.Add(curDeck.Pop());
            }
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

        public override void TakeDamage(int amount, DamageTypes dType)
        {
            int damageTaken = amount;
            if (armours.ContainsKey(dType))
            {
                int result = armours[dType] - amount;
                if (result < 1)
                {
                    armours.Remove(dType);
                    GameObject plr = GameObject.Find($"Player{playerNum}");
                    GameObject physArm = plr.transform.Find("PhysicalArmour").gameObject;
                    physArm.GetComponent<SpriteRenderer>().enabled = false;
                    physArm.transform.Find("armAmount").GetComponent<MeshRenderer>().enabled = false;
                    damageTaken -= Math.Abs(result);
                }
                else
                {
                    armours[dType] -= amount;
                    GameObject plr = GameObject.Find($"Player{playerNum}").gameObject;
                    GameObject physArm = plr.transform.Find("PhysicalArmour").gameObject;
                    GameObject armAmount = physArm.transform.Find("armAmount").gameObject;

                    armAmount.GetComponent<TextMesh>().text = armours[dType].ToString();
                    return;
                }
            }
            Hp -= damageTaken;
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
