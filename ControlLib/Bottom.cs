using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ControlLib
{
    public class Bottom
    {
        /// <summary> Virtual key </summary>
        public VirtualKey Key { get; init; }


        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(int vKey);

        public bool IsKeyPressed()
        {
            return Key == VirtualKey.None || (GetAsyncKeyState((int)Key) & 0x8000) != 0; 
           
        }

        public Bottom(VirtualKey key)
        {
            this.Key = key;
        }
    }
}
