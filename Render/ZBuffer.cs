using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Render.RenderText;
using ScreenLib;
using SFML.Graphics;
using SFML.Window;

namespace Render.ZBufferRender
{
    public class ZBuffer
    {
        private static ConcurrentDictionary<double, Drawable> zBuffer = new ConcurrentDictionary<double, Drawable>();
        static object lockObj = new object();
        
        public static void Render()
        {
            List<KeyValuePair<double, Drawable>> sortedList;

            lock (lockObj)
            {
                sortedList = zBuffer.OrderByDescending(kv => kv.Key).ToList();
            }

            foreach (var kv in sortedList)
            {
                Screen.OutputPriority.AddToPriority(2, kv.Value);
            }

            zBuffer.Clear();
        }

        public static void AddToZBuffer(Drawable drawable, double depth)
        {
            zBuffer[depth] = drawable;
        }
    }
}
