using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ControlLib
{
    public class Bottom
    {
        public VirtualKey Key {  get; init; }


        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(int vKey);

        public Bottom(VirtualKey key)
        {
            Key = key;
        }
    }
}
