using System.Diagnostics;
using TextureLib.Textures;
using TextureLib.Loader;
using TextureLib.Loader.ImageProcessing;
using SFML.Graphics;

namespace AnimationLib;
/// <summary>
/// Represents an animation that consists of multiple frames (textures).
/// Provides functionality to add, remove and retrieve frames, 
/// control playback speed, and determine the current frame.
/// </summary>
public class AnimationState
{
    /// <summary>
    /// Index of the current frame.
    /// </summary>
    public int Index { get; set; } = 0;

    /// <summary>
    /// Total number of frames in the animation.
    /// </summary>
    public int CountFrame => Frames.Count;

    /// <summary>
    /// Animation playback speed. 
    /// The higher the value, the slower the playback.
    /// </summary>
    public int Speed { get; set; }

    /// <summary>
    /// Internal list of frames.
    /// </summary>
    internal List<TextureWrapper> Frames { get; init; } = new();

    /// <summary>
    /// The current frame of the animation.
    /// </summary>
    public TextureWrapper? CurrentFrame { get; set; }

    /// <summary>
    /// Bounding rectangle that contains all frames (maximal extents).
    /// </summary>
    public IntRect MaxFrameRect { get; private set; } = new IntRect(0, 0, 0, 0);

    /// <summary>
    /// Whether the animation is active (true) or static (false).
    /// </summary>
    public bool IsAnimation { get; set; } = false;

    /// <summary>
    /// Timer for tracking animation timing. 
    /// May be <c>null</c> if not used.
    /// </summary>
    public Stopwatch? Stopwatch = null;

    /// <summary>
    /// Timestamp of the last frame switch, in milliseconds.
    /// </summary>
    public long LastFrameTime = 0;

    /// <summary>
    /// Indicates whether all frames are loaded and ready.
    /// </summary>
    public bool IsLoaded { get; private set; } = false;

    /// <summary>
    /// Gets or sets the image loading options used to configure how images are processed and loaded.
    /// </summary>
    public ImageLoadOptions LoadOptions { get; set; } = new ImageLoadOptions();

    /// <summary>
    /// Initializes a new <see cref="AnimationState"/> by loading textures from file paths.
    /// Can load synchronously or asynchronously.
    /// </summary>
    /// <param name="options">Optional parameters for advanced loading behavior.</param>
    /// <param name="loadAsync">Whether to load frames asynchronously.</param>
    /// <param name="paths">File paths to load textures from.</param>
    public AnimationState(ImageLoadOptions? options = null, bool loadAsync = true, params string[] paths)
    {
        LoadOptions = options ?? new ImageLoadOptions();

        if (loadAsync)
            _ = AddFramesAsync(ImageLoader.LoadAsync(options, true, paths));
        else
        {
            AddFrames(ImageLoader.Load(options, true, paths));
            IsLoaded = true;
        }
    }

    /// <summary>
    /// Initializes a new <see cref="AnimationState"/> with a single frame.
    /// </summary>
    /// <param name="frame">Texture for the single frame.</param>
    public AnimationState(TextureWrapper frame) 
    {
        AddFrame(frame);
        IsLoaded = true;
    }

    /// <summary>
    /// Initializes a new <see cref="AnimationState"/> with a list of frames.
    /// </summary>
    /// <param name="frames">List of textures for the frames.</param>
    public AnimationState(List<TextureWrapper> frames)
    {
        AddFrames(frames);
        IsLoaded = true;
    }

    /// <summary>
    /// Initializes a new <see cref="AnimationState"/> by copying another instance.
    /// Can load frames synchronously or asynchronously.
    /// </summary>
    /// <param name="animationState">The animation state to copy.</param>
    /// <param name="loadAsync">Whether to load frames asynchronously.</param>
    public AnimationState(AnimationState animationState, bool loadAsync = true)
    {
        if (loadAsync)
            _ = LoadObjectAsync(animationState);
        else
        {
            AddFrames(animationState.GetFrames());
            IsLoaded = true;
        }

        IsAnimation = animationState.IsAnimation;
        Speed = animationState.Speed;
    }

    /// <summary>
    /// Initializes a new empty <see cref="AnimationState"/>.
    /// </summary>
    public AnimationState()
    {}



    /// <summary>
    /// Waits for another animation state to finish loading, 
    /// then copies its frames into this instance.
    /// </summary>
    /// <param name="animationState">The animation state to load from.</param>
    public async Task LoadObjectAsync(AnimationState animationState)
    {
        await Task.Run(async () =>
        {
            while (!animationState.IsLoaded)
            {
                await Task.Delay(500);
            }

        });

        AddFrames(animationState.GetFrames());
        IsLoaded = true;
    }

    /// <summary>
    /// Adds a single frame to the animation.
    /// </summary>
    /// <param name="frame">Texture to add.</param>
    public void AddFrame(TextureWrapper frame)
    {
        Frames.Add(frame);
        UpdateMaxRect(frame.Rect);
    }

    /// <summary>
    /// Asynchronously adds a single frame to the animation.
    /// </summary>
    /// <param name="frameTask">Task returning the texture to add.</param>
    public async Task AddFrame(Task<TextureWrapper> frameTask)
    {
        var frame = await frameTask;
        Frames.Add(frame);

        UpdateMaxRect(frame.Rect);
    }

    /// <summary>
    /// Adds multiple frames to the animation.
    /// </summary>
    /// <param name="frames">List of textures to add.</param>
    public void AddFrames(List<TextureWrapper> frames)
    {
        Frames.AddRange(frames);

        foreach (var frame in frames)
            UpdateMaxRect(frame.Rect);
    }

    /// <summary>
    /// Asynchronously adds multiple frames to the animation.
    /// </summary>
    /// <param name="framesTask">Task returning a list of textures to add.</param>
    public async Task AddFramesAsync(Task<List<TextureWrapper>> framesTask)
    {
        var frames = await framesTask;
        Frames.AddRange(frames);

        foreach (var frame in frames)
            UpdateMaxRect(frame.Rect);

        IsLoaded = true;
    }

    /// <summary>
    /// Removes a frame from the animation.
    /// </summary>
    /// <param name="frame">Texture to remove.</param>
    /// <returns><c>true</c> if the frame was removed, otherwise <c>false</c>.</returns>
    public bool RemoveFrame(TextureWrapper frame)
    {
        bool removed = Frames.Remove(frame);
        return removed;
    }

    /// <summary>
    /// Returns the frame at the specified index.
    /// </summary>
    /// <param name="index">Index of the frame.</param>
    /// <returns>The texture at the given index, or <c>null</c> if index is out of bounds.</returns>
    public TextureWrapper? GetFrame(int index)
    {
        if (index < 0 || index >= CountFrame)
            return null;

        return Frames[index];
    }

    /// <summary>
    /// Returns a list of all frames.
    /// </summary>
    /// <returns>List of all textures in the animation.</returns>
    public List<TextureWrapper> GetFrames() => Frames;

    /// <summary>
    /// Replaces all frames with the specified list.
    /// </summary>
    /// <param name="textureObstacles">New list of textures.</param>
    public void SetFrames(List<TextureWrapper> textureObstacles)
    {
        Frames.Clear();
        Frames.AddRange(textureObstacles);
    }

    /// <summary>
    /// Updates the maximal bounding rectangle to include the specified frame.
    /// </summary>
    /// <param name="newRect">Rectangle of the added frame.</param>
    private void UpdateMaxRect(IntRect newRect)
    {
        if (CountFrame == 1)
        {
            MaxFrameRect = newRect;
            return;
        }

        int left = System.Math.Min(MaxFrameRect.Left, newRect.Left);
        int top = System.Math.Min(MaxFrameRect.Top, newRect.Top);

        int right = System.Math.Max(MaxFrameRect.Left + MaxFrameRect.Width, newRect.Left + newRect.Width);
        int bottom = System.Math.Max(MaxFrameRect.Top + MaxFrameRect.Height, newRect.Top + newRect.Height);

        MaxFrameRect = new IntRect(left, top, right - left, bottom - top);
    }
}
