using Assets.Cards;
using Assets.PlayersClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Enemies
{
    class Bandit : EnemyBase
    {
        string Name = "Bandit";

        public Bandit(string eNum)
        {
            enemyNum = eNum;
            GameObject Player1 = GameObject.Find("Enemy" + enemyNum);
            Player1.GetComponent<SpriteRenderer>().sprite = GetSprite();
        }
        public override void Init()
        {
            maxHp = 15;
            Hp = maxHp;
        }

        public override bool TakeTurn(List<PlayerClassBase> players, List<EnemyBase> enemies)
        {
            // bandits just do one thing and then pass, so they always return true for passing (see bottom)

            int playerTarget = 0;
            if (players.Count > 1)
            {
                System.Random rnd = new System.Random();
                playerTarget = rnd.Next(0, players.Count);
            }

            // todo they should buff eachother if there are more, right now just attack
            players[playerTarget].TakeDamage(CardBase.RollDamage(1,6,0), DamageTypes.PHYSICAL);

            return true;
        }

        public override void TakeDamage(int amount)
        {
            Hp -= amount;
            GameObject me = GameObject.Find($"Enemy{enemyNum}");
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

        public override Sprite GetSprite()
        {
            try
            {
                Sprite sp =  Resources.Load<Sprite>("Images/Cultist");
                return sp;
                //float myScale = 0.5f;
                //sp.transform.localScale = new Vector3(myScale, myScale, myScale);

            }
            catch (Exception)
            {
                Debug.Log("Failed to load sprite!");
                return null;
            }
        }
    }
}
