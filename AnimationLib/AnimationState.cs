using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextureLib;

namespace ObstacleLib.SpriteLib.Animation;
public class AnimationState
{
    /// <summary>Index current frame</summary>
    internal int Index { get; set; } = 0;
    /// <summary>Frame counter</summary>
    internal int FrameCounter { get; set; } = 0;

    /// <summary>Number of frames</summary>
    public int AmountFrame { get; set; } = 0;
    /// <summary>Animation playback speed(The higher the value, the slower)</summary>
    public int Speed { get; set; } = 0;
    internal List<TextureObstacle> Frames { get; init; } = new();
    public TextureObstacle? CurrentFrame { get; internal set; }
    public bool IsAnimation { get; set; } = false;


    public AnimationState(TextureObstacle frame) 
    {
        AddFrame(frame);
    }
    public AnimationState(List<TextureObstacle> frames)
    {
        foreach (var frame in frames)
            AddFrame(frame);
    }
    public AnimationState()
    {}

    public void AddFrame(TextureObstacle frame)
    {
        Frames.Add(frame);
        AmountFrame++;
    }
    public bool RemoveFrame(TextureObstacle frame)
    {
        bool removed = Frames.Remove(frame);
        if (removed) AmountFrame--;
        return removed;
    }
    public TextureObstacle? GetFrame(int index)
    {
        if (index < 0 || index >= AmountFrame)
            return null;

        return Frames[index];
    }
}
