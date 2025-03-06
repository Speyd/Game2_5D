using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using NGenerics.DataStructures.Trees;


namespace ScreenLib.Output;
public class OutputPriority(RenderWindow window)
{
    public SortedDictionary<OutputPriorityType, List<(Drawable, RenderStates?)>> TreePriority { get; init; } = new();
    public void AddToPriority(OutputPriorityType priority, Drawable sprite, RenderStates? state = null)
    {
        if (!TreePriority.TryGetValue(priority, out var list))
        {
            list = new List<(Drawable, RenderStates?)>();
            TreePriority[priority] = list;
        }

        list.Add((sprite, state));
    }

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
