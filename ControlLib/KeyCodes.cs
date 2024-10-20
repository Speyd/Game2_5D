using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ControlLib.Pressed.Key
{
    internal class KeyCodes
    {
        public const int VK_FORWARD = 0x57;     //W
        public const int VK_BACK = 0x53;        //S
        public const int VK_LEFT = 0x41;        //A
        public const int VK_RIGHT = 0x44;       //D
        public const int VK_TURN_LEFT = 0x25;   //left arrow
        public const int VK_TURN_RIGHT = 0x27;  //right arrow
        public const int VK_EXIT = 0x51;        //Q


        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(int key);
        public KeyCodes()
        { }
    }
}
