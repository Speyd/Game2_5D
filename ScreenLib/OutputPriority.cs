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
        public SortedDictionary<int, List<Drawable>> TreePriority { get; init; } = new SortedDictionary<int, List<Drawable>>();

        public void AddToPriority(int priority, Drawable sprite)
        {
            if (!TreePriority.ContainsKey(priority))
            {
                TreePriority[priority] = new List<Drawable>();
            }

            TreePriority[priority].Add(sprite);
        }

        public void DrawingByPriority()
        {
            foreach (var pair in TreePriority)
            {
                foreach (var sprite in pair.Value)
                {
                    window.Draw(sprite);
                }
            }

            TreePriority.Clear();
        }

        public void DrawingByPriorityNo()
        {
            foreach (var pair in TreePriority)
            {
                foreach (var sprite in pair.Value)
                {
                    window.Draw(sprite);
                }
            }
        }
    }
}
