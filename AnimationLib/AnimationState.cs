using TextureLib;

namespace AnimationLib;
/// <summary>
/// Class for managing animation consisting of several frames (textures).
/// Allows you to add, remove and get frames, as well as control the playback speed.
/// </summary>
public class AnimationState
{
    /// <summary>Index current frame</summary>
    public int Index { get; set; } = 0;
    /// <summary>Frame counter</summary>
    internal int FrameCounter { get; set; } = 0;

    /// <summary>Number of frames</summary>
    public int AmountFrame { get; set; } = 0;
    private int _speed = 0;
    /// <summary>Animation playback speed(The higher the value, the slower)</summary>
    public int Speed 
    {
        get => _speed;
        set
        {
            _speed = value;
            FrameCounter = value;
        }
    } 
    internal List<TextureObstacle> Frames { get; init; } = new();
    /// <summary>Current frame</summary>
    public TextureObstacle? CurrentFrame { get; set; }
    /// <summary>true - animation is used, false - animation is not used</summary>
    public bool IsAnimation { get; set; } = false;


    /// <summary>Constructor class AnimationState</summary>
    /// <param name="paths">File paths</param>
    public AnimationState(params string[] paths)
    {
        AddFrames(ImageLoader.TexturesLoad(paths));

    }

    /// <summary>Constructor class AnimationState</summary>
    /// <param name="path">File path</param>
    public AnimationState(string path)
    {
        AddFrames(ImageLoader.TexturesLoad(path));
    }

    /// <summary>Constructor class AnimationState</summary>
    /// <param name="frame">instance TextureObstacle</param>
    public AnimationState(TextureObstacle frame) 
    {
        AddFrame(frame);
    }

    /// <summary>Constructor class AnimationState</summary>
    /// <param name="frames">list of instance TextureObstacle</param>
    public AnimationState(List<TextureObstacle> frames)
    {
        foreach (var frame in frames)
            AddFrame(frame);
    }

    /// <summary>Constructor class AnimationState</summary>
    /// <param name="animationState">Object of AnimationState</param>
    public AnimationState(AnimationState animationState)
        :this(animationState.GetFrames())
    {
        IsAnimation = animationState.IsAnimation;
        Speed = animationState.Speed;
    }
    /// <summary>Constructor class AnimationState</summary>
    public AnimationState()
    {}

    /// <summary>Adding a frame to list of frames </summary>
    public void AddFrame(TextureObstacle frame)
    {
        Frames.Add(frame);
        AmountFrame++;
    }
    /// <summary>Adding a frames to list of frames </summary>
    public void AddFrames(List<TextureObstacle> frames)
    {
        Frames.AddRange(frames);
        AmountFrame += frames.Count;
    }
    /// <summary>Remove a frame in list of frames </summary>
    public bool RemoveFrame(TextureObstacle frame)
    {
        bool removed = Frames.Remove(frame);
        if (removed) AmountFrame--;
        return removed;
    }
    /// <summary>Get a frame from list of frames </summary>
    public TextureObstacle? GetFrame(int index)
    {
        if (index < 0 || index >= AmountFrame)
            return null;

        return Frames[index];
    }
    /// <summary>Get all the frames</summary>
    public List<TextureObstacle> GetFrames() => Frames;
    /// <summary>Set frames</summary>
    public void SetFrames(List<TextureObstacle> textureObstacles)
    {
        Frames.Clear();
        Frames.AddRange(textureObstacles);

        AmountFrame = Frames.Count;
    }
}
