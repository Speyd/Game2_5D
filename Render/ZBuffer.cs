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
        private static ConcurrentDictionary<double, (Drawable, RenderStates?)> zBuffer = new();
        static object lockObj = new object();
        
        public static void Render()
        {
            List<KeyValuePair<double, (Drawable, RenderStates?)>> sortedList;

            lock (lockObj)
            {
                sortedList = zBuffer.OrderByDescending(kv => kv.Key).ToList();
            }

            foreach (var kv in sortedList)
            {
                Screen.OutputPriority.AddToPriority(RenderPriority.Background, kv.Value.Item1, kv.Value.Item2);
            }

            zBuffer.Clear();
        }

        public static void AddToZBuffer(Drawable drawable, double depth, RenderStates? renderStates = null)
        {
            zBuffer[depth] = (drawable, renderStates);
        }
    }
}
