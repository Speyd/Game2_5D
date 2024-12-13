using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObstacleLib.SpriteLib.Animation
{
    public class AnimationState
    {
        public int Index { get; set; } = 0;
        public int Count { get; set; } = 0;
        public int Speed { get; set; } = 0;

        public bool IsAnimation { get; set; } = false;
    }
}
