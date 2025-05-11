using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using NGenerics.DataStructures.Trees;


namespace ScreenLib.Output;
/// <summary>A class that collects or sorts all objects on the screen.</summary>
public class OutputPriority(RenderWindow window)
{
    /// <summary>Object tree</summary>
    public SortedDictionary<OutputPriorityType, List<(Drawable, RenderStates?)>> TreePriority { get; init; } = new();

    /// <summary>
    /// Adds an object to the rendering order
    /// </summary>
    /// <param name="priority">Object priority in the object tree</param>
    /// <param name="drawObject">The object that will be drawn</param>
    /// <param name="state">State of the object being rendered</param>
    public void AddToPriority(OutputPriorityType priority, Drawable drawObject, RenderStates? state = null)
    {
        if (!TreePriority.TryGetValue(priority, out var list))
        {
            list = new List<(Drawable, RenderStates?)>();
            TreePriority[priority] = list;
        }

        list.Add((drawObject, state));
    }
    /// <summary> Drawing the render tree </summary>
    public void DrawingByPriority()
    {
        foreach (var pair in TreePriority)
        {
            foreach (var sprite in pair.Value)
            {
                if (sprite.Item2.HasValue)
                    window.Draw(sprite.Item1, sprite.Item2.Value);
                else
                    window.Draw(sprite.Item1);
            }
        }

        TreePriority.Clear();
    }
}
