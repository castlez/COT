using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Cards
{
    public static class Meta
    {
        public static int MAXPLAYERS = 2;  // should be 4, set to 2 for testing
        public static int CURRENTPLAYERS = 2;  // TODO figure this out at run time
        public static int MAX_HAND_SIZE = 5;
        public static int STARTING_HAND_SIZE = 3;
    }
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
        ENEMY,
        ENEMYMULTIPLE
    }

    public enum StatusEffectTimes
    {
        STARTTURN,
        ENDTURN,
        ONATTACK,
        ONTAKEDAMAGE
    }
}
