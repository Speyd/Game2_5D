using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlLib.Pressed
{
    [Flags]
    public enum Direction
    {
        None = 0,
        Forward = 1,
        Backward = 2,
        Left = 4,
        Right = 8,
        TurnLeft = 16,
        TurnRight = 32,
        Exit = 64
    }
}
