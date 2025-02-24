using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScreenLib;
using SFML.Graphics;
using SFML.Window;


namespace Render.RenderAlgorithm;
public class ZBuffer
{
    private static ConcurrentDictionary<double, (Drawable, RenderStates?)> zBuffer = new();
    public static void Render()
    {
        foreach (var kv in zBuffer.OrderByDescending(kv => kv.Key))
        {
            Screen.OutputPriority.AddToPriority(RenderPriority.ZBufferRender, kv.Value.Item1, kv.Value.Item2);
        }

        zBuffer.Clear();
    }

    public static void AddToZBuffer(Drawable drawable, double depth, RenderStates? renderStates = null)
    {
        zBuffer[depth] = (drawable, renderStates);
    }
}
