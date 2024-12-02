using System;
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
        private static PriorityQueue<Drawable, double> zBuffer = new PriorityQueue<Drawable, double>();

        public void Render()
        {
            while (zBuffer.Count > 0)
            {
                var drawable = zBuffer.Dequeue();
                Screen.OutputPriority.AddToPriority(2, drawable);
            }
        }

        public static void AddToZBuffer(Drawable drawable, double depth)
        {
            zBuffer.Enqueue(drawable, depth);
        }
    }
}
