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
    public abstract class EnemyBase
    {
        public string Name;
        public int maxHp;
        public int Hp;
        public string enemyNum;

        public abstract void Init();

        public abstract void TakeDamage(int amount, DamageTypes dType);

        public abstract Sprite GetSprite();

        public abstract bool TakeTurn(List<PlayerClassBase> players, List<EnemyBase> enemies);

        public void SetTargetted(bool targetted)
        {
            GameObject cvs = GameObject.Find("Canvas").gameObject;
            GameObject me = cvs.transform.Find($"Enemy{enemyNum}").gameObject;
            GameObject targInd = me.transform.Find("TargetInd").gameObject;
            targInd.GetComponent<SpriteRenderer>().enabled = targetted;
        }

        public void SetIntent(bool hasIntent)
        {
            GameObject cvs = GameObject.Find("Canvas").gameObject;
            GameObject me = cvs.transform.Find($"Enemy{enemyNum}").gameObject;
            GameObject intent = me.transform.Find("Intent").gameObject;
            GameObject symb = intent.transform.Find("symbol").gameObject;
            GameObject val = intent.transform.Find("value").gameObject;
            if (!hasIntent)
            {
                symb.SetActive(false);
                val.SetActive(false);
            }
            else
            {
                symb.SetActive(true);
                val.SetActive(true);
                // TODO get intent from self somehow?
                symb.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/AttackIntent");
                val.GetComponent<TextMesh>().text = "6";
            }
        }
    }
}
