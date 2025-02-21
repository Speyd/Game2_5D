using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using NGenerics.DataStructures.Trees;

namespace ScreenLib
{
    public class OutputPriority(RenderWindow window)
    {
        public SortedDictionary<int, List<(Drawable, RenderStates?)>> TreePriority { get; init; } = new();
        public void AddToPriority(int priority, Drawable sprite, RenderStates? state = null)
        {
            if (!TreePriority.ContainsKey(priority))
            {
                TreePriority[priority] = new List<(Drawable, RenderStates?)>();
            }

            TreePriority[priority].Add((sprite, state));
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
}
