using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtoRender.RenderAlgorithm;
/// <summary>
/// Represents a thread-safe queue for scheduling and executing drawing operations.
/// </summary>
public static class DrawingQueue
{
    private static readonly Queue<(Action<Drawable>, Drawable)> drawQueue = new();
    private static readonly object locker = new();

    /// <summary>
    /// Adds a drawing action with its target drawable object to the queue.
    /// </summary>
    /// <param name="action">A tuple containing the drawing action and the drawable target.</param>
    public static void EnqueueDraw((Action<Drawable>, Drawable) action)
    {
        lock (locker)
        {
            drawQueue.Enqueue(action);
        }
    }

    /// <summary>
    /// Executes all drawing actions in the queue in a thread-safe manner.
    /// </summary>
    public static void ExecuteAll()
    {
        lock (locker)
        {
            while (drawQueue.Count > 0)
            {
                var drawAction = drawQueue.Dequeue();
                drawAction.Item1.Invoke(drawAction.Item2);
            }
        }
    }
}
