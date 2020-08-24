using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Cards
{
    public enum CardTypes
    {
        ATTACK,
        SKILLS,
        AURA,
        PREPARE
    }

    public enum DamageTypes
    {
        FIRE,
        ICE,
        PHYSICAL,
        SLASHING,
        CRUSHING,
        HOLY
    }

    public enum TargetTypes
    {
        PLAYER,
        SELF,
        ENEMY
    }
}
