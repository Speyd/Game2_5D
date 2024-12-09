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
        private static SortedList<double, Drawable> zBuffer = new SortedList<double, Drawable>(Comparer<double>.Create((x, y) => y.CompareTo(x)));

        public void Render()
        {
            foreach (var drawable in zBuffer.Values)
            {
                Screen.OutputPriority.AddToPriority(2, drawable);
            }
            zBuffer.Clear();
        }

        public static void AddToZBuffer(Drawable drawable, double depth)
        {
            zBuffer[depth] = drawable;
        }
    }
}
