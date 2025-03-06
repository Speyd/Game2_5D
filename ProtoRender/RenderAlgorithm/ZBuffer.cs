using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScreenLib;
using ScreenLib.Output;
using SFML.Graphics;
using SFML.Window;


namespace ProtoRender.RenderAlgorithm;
public class ZBuffer
{
    private static ConcurrentBag<(double, (Drawable, RenderStates?))> zBuffer = new();
    public static void Render()
    {
        var sortedBuffer = zBuffer.AsParallel().OrderByDescending(kv => kv.Item1);
        foreach (var kv in sortedBuffer)
        {
            Screen.OutputPriority?.AddToPriority(OutputPriorityType.ZBufferRender, kv.Item2.Item1, kv.Item2.Item2);
        }

        zBuffer.Clear();
    }

    public static void AddToZBuffer(Drawable drawable, double depth, RenderStates? renderStates = null)
    {
        zBuffer.Add((depth,(drawable, renderStates)));
    }
}
