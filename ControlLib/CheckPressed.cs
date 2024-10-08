﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.InteropServices;
using static ControlLib.KeyCodes;

namespace ControlLib
{
    public class CheckPressed
    {
        public bool isForwardPressed;
        public bool isBackPressed;
        public bool isLeftPressed;
        public bool isRightPressed;
        public bool isTurnLeftPressed;
        public bool isTurnRightPressed;
        public bool isTurnBoost;

        public CheckPressed()
        { }

        public void check()
        {
            isForwardPressed = (GetAsyncKeyState(VK_FORWARD) & 0x8000) != 0;
            isBackPressed = (GetAsyncKeyState(VK_BACK) & 0x8000) != 0;
            isLeftPressed = (GetAsyncKeyState(VK_LEFT) & 0x8000) != 0;
            isRightPressed = (GetAsyncKeyState(VK_RIGHT) & 0x8000) != 0;
            isTurnLeftPressed = (GetAsyncKeyState(VK_TURN_LEFT) & 0x8000) != 0;
            isTurnRightPressed = (GetAsyncKeyState(VK_TURN_RIGHT) & 0x8000) != 0;
            isTurnBoost = (GetAsyncKeyState(VK_BOOST) & 0x8000) != 0;
        }
    }
}
