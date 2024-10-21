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
        public DirectionFlags CurrentDirection { get; private set; } = new DirectionFlags();

        private bool IsKeyPressed(int keyCode)
        {
            return (GetAsyncKeyState(keyCode) & 0x8000) != 0;
        }
        public void check()
        {

            CurrentDirection.Forward = IsKeyPressed(VK_FORWARD);
            CurrentDirection.Backward = IsKeyPressed(VK_BACK);
            CurrentDirection.Left = IsKeyPressed(VK_LEFT);
            CurrentDirection.Right = IsKeyPressed(VK_RIGHT);
            CurrentDirection.TurnLeft = IsKeyPressed(VK_TURN_LEFT);
            CurrentDirection.TurnRight = IsKeyPressed(VK_TURN_RIGHT);

            CurrentDirection.ZoomMiniMap = IsKeyPressed(VK_ZOOM_MINIMAP);
            CurrentDirection.ReduceMiniMap = IsKeyPressed(VK_REDUCE_MINIMAP);

            CurrentDirection.Exit = IsKeyPressed(VK_EXIT);
        }
    }
}
