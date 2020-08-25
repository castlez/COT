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
            GameObject me = GameObject.Find($"Enemy{enemyNum}");
            GameObject cvs = me.transform.Find("Canvas").gameObject;
            GameObject targInd = cvs.transform.Find("TargetInd").gameObject;
            targInd.GetComponent<SpriteRenderer>().enabled = targetted;
        }
    }
}
