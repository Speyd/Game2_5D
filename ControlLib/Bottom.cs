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
        /// <summary> Max delay between clicks </summary>
        public long WaitingTimeMilliseconds { get; set; }
        /// <summary> Is the object pending? </summary>
        public bool IsWaiting {  get; private set; } = false;

        private Stopwatch stopwatch = new Stopwatch();


        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(int vKey);

        public bool IsKeyPressed()
        {
            bool isPress = (GetAsyncKeyState((int)Key) & 0x8000) != 0;

            if (!stopwatch.IsRunning && isPress && WaitingTimeMilliseconds > 0)
            {
                IsWaiting = true;
                stopwatch.Start();
            }

            if (stopwatch.ElapsedMilliseconds >= WaitingTimeMilliseconds)
            {
                IsWaiting = false;

                stopwatch.Stop();
                stopwatch.Reset();
            }

            return !IsWaiting && isPress;
        }

        public Bottom(VirtualKey key, long waitingTimeMilliseconds)
        {
            this.Key = key;
            this.WaitingTimeMilliseconds = waitingTimeMilliseconds;
        }
        public Bottom(VirtualKey key)
            :this(key, 0)
        {}
    }
}
