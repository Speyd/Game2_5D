using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ControlLib.Pressed.Key.KeyCodes;

namespace ControlLib.Pressed
{
    internal class CheckPressed
    {
        public Direction CurrentDirection { get; private set; }

        public CheckPressed()
        { }

        private bool IsKeyPressed(int keyCode)
        {
            return (GetAsyncKeyState(keyCode) & 0x8000) != 0;
        }
        public void check()
        {
            CurrentDirection = Direction.None;

            if (IsKeyPressed(VK_FORWARD)) CurrentDirection |= Direction.Forward;
            if (IsKeyPressed(VK_BACK)) CurrentDirection |= Direction.Backward;
            if (IsKeyPressed(VK_LEFT)) CurrentDirection |= Direction.Left;
            if (IsKeyPressed(VK_RIGHT)) CurrentDirection |= Direction.Right;
            if (IsKeyPressed(VK_TURN_LEFT)) CurrentDirection |= Direction.TurnLeft;
            if (IsKeyPressed(VK_TURN_RIGHT)) CurrentDirection |= Direction.TurnRight;
            if (IsKeyPressed(VK_EXIT)) CurrentDirection |= Direction.Exit;
        }
    }
}
