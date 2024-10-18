using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScreenLib;
using SFML.Graphics;
using SFML.Window;

namespace Render.ZBufferRender
{
    public class ZBuffer(Screen screen)
    {
        static public List<(Drawable, double)> zBuffer = new List<(Drawable, double)>();

        public void render()
        {
            zBuffer.Sort((a, b) => b.Item2.CompareTo(a.Item2));


            foreach (var (drawable, _) in zBuffer)
            {
                screen.Window.Draw(drawable);
            }

            zBuffer.Clear();
        }
    }
}
