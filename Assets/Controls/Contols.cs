﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Controls
{
    public static class Cin
    {
        // DEBUGGING(?)
        public static bool USE_KEYBOARD = true;  // use keyboard to control player 1
        public static bool singlePlayer;  // use keyboard to control all players, basically single player

        public delegate bool COperator(float a, float b);
        public static COperator lessThan = delegate (float a, float b)
        {
            return a < b;
        };
        public static COperator greaterThan = delegate (float a, float b)
        {
            return a > b;
        };

        public static bool CheckKey(string keyName, string pNum, COperator op)
        {
            float keyCheck;
            if (USE_KEYBOARD)
            {
                if (pNum == "1" || singlePlayer)
                {
                    // if the keyboard is in use, shift controllers down
                    if (keyName == "XBoxPassTurn")
                    {
                        keyCheck = Input.GetAxisRaw("KeyPass");
                    }
                    else if (keyName == "XBoxAction")
                    {
                        keyCheck = Input.GetAxisRaw("KeyAction");
                    }
                    else if (keyName == "XBoxHoriz")
                    {
                        keyCheck = Input.GetAxisRaw("Horizontal");
                    }
                    else if (keyName == "XBoxVert")
                    {
                        keyCheck = Input.GetAxisRaw("Vertical");
                    }
                    else if (keyName == "XBoxBack")
                    {
                        keyCheck = Input.GetAxisRaw("KeyBack");
                    }
                    else
                    {
                        Debug.LogError($"CheckKey recieved unknown key '{keyName}' with keyboard enabled!");
                        return false;
                    }
                }
                else
                {
                    int num = int.Parse(pNum) - 1;
                    keyCheck = Input.GetAxisRaw($"{keyName}{num}");
                }
            }
            else  // shows as unreachable if USE_KEYBOARD is true
            {
                keyCheck = Input.GetAxisRaw($"{keyName}{pNum}");
            }
            if (op(keyCheck, 0))
            {
                return true;
            }
            return false;
        }
    }
}
