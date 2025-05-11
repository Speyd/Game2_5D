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
/// <summary>
/// A static class that manages Z-buffer rendering by storing and sorting drawable objects based on their depth.
/// </summary>
public class ZBuffer
{
    /// <summary>
    /// A thread-safe collection that stores tuples of depth and drawable objects with optional render states.
    /// </summary>
    private static ConcurrentBag<(double, (Drawable, RenderStates?))> zBuffer = new();

    /// <summary>
    /// Sorts all stored drawables by their depth in descending order and adds them to the render pipeline.
    /// </summary>
    public static void Render()
    {
        var sortedBuffer = zBuffer.AsParallel().OrderByDescending(kv => kv.Item1);
        foreach (var kv in sortedBuffer)
        {
            Screen.OutputPriority?.AddToPriority(OutputPriorityType.ZBufferRender, kv.Item2.Item1, kv.Item2.Item2);
        }

        zBuffer.Clear();
    }

    /// <summary>
    /// Adds a drawable object to the Z-buffer with the specified depth and optional render state.
    /// </summary>
    /// <param name="drawable">The drawable object to be rendered.</param>
    /// <param name="depth">The depth value used for sorting. Higher values are rendered first.</param>
    /// <param name="renderStates">Optional rendering states associated with the drawable.</param>
    public static void AddToZBuffer(Drawable drawable, double depth, RenderStates? renderStates = null)
    {
        zBuffer.Add((depth, (drawable, renderStates)));
    }
}
