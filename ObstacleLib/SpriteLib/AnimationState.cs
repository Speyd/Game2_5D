using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ObstacleLib.SpriteLib.Animation;
public class AnimationState
{
    /// <summary>Current frame</summary>
    public int Index { get; set; } = 0;
    /// <summary>Number of frames</summary>
    public int Count { get; set; } = 0;
    /// <summary>Animation playback speed</summary>
    public int Speed { get; set; } = 0;

    public bool IsAnimation { get; set; } = false;
}
