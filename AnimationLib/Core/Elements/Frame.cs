using AnimationLib.Enum;
using SFML.Graphics;
using TextureLib.Loader;
using TextureLib.Textures;

namespace AnimationLib.Core.Elements;

/// <summary>
/// Represents a frame-based animation composed of <see cref="TextureWrapper"/> elements.
/// Provides functionality for loading, adding, and managing frames, 
/// as well as handling asynchronous loading and playback state.
/// </summary>
public class Frame : AnimationBase<TextureWrapper>
{
    /// <summary>
    /// Interval in milliseconds between consecutive checks while waiting for another frame to load.
    /// If set to 0 or less, polling is continuous using <see cref="Task.Yield"/> to avoid CPU blocking.
    /// </summary>
    public int LoadingPollDelayMs { get; set; } = 500;

    /// <summary>
    /// Maximum time in milliseconds to wait for another frame to finish loading.
    /// If 0 or less, waiting is indefinite.
    /// </summary>
    public int LoadingTimeoutMs { get; set; } = 30000;

    /// <summary>
    /// Indicates whether this frame and its elements have finished loading.
    /// </summary>
    public bool IsLoaded { get; private set; } = false;


    private TextureWrapper? _currentElement = null;
    /// <summary>
    /// Returns the current <see cref="TextureWrapper"/> element.
    /// If the frame is not yet loaded, returns a placeholder texture.
    /// </summary>
    public override TextureWrapper? CurrentElement
    {
        get
        {
            if (!IsLoaded)
                return TextureWrapper.Placeholder;

            return _currentElement;
        }
        protected set
        {
            _currentElement = value;
        }
    }

    /// <summary>
    /// Maximum bounding rectangle containing all loaded frames.
    /// </summary>
    public IntRect MaxFrameRect { get; private set; } = new IntRect(0, 0, 0, 0);

    /// <summary>
    /// Defines how the frame rectangles are interpreted when rendering.
    /// </summary>
    public FrameRectMode RectMode { get; set; }

    /// <summary>
    /// Optional settings controlling image loading behavior for this frame.
    /// </summary>
    public ImageLoadOptions? LoadOptions { get; set; } = null;

    /// <summary>
    /// Initializes a new <see cref="Frame"/> by loading textures from file paths.
    /// </summary>
    /// <param name="paths">Paths to texture files.</param>
    public Frame(params string[] paths)
     : this(null, paths)
    {
        IsLoaded = true;
    }

    /// <summary>
    /// Initializes a new <see cref="Frame"/> with specified loading options and texture paths.
    /// </summary>
    /// <param name="options">Image load options.</param>
    /// <param name="paths">Paths to texture files.</param>
    public Frame(ImageLoadOptions? options, params string[] paths)
    {
        LoadOptions = options ?? new ImageLoadOptions();

        if (paths.Length == 0)
            return;

        if (LoadOptions.LoadAsync)
            _ = AddFramesAsync(ImageLoader.LoadAsync(LoadOptions, paths));
        else
            AddFrames(ImageLoader.Load(LoadOptions, paths));
    }

    /// <summary>
    /// Initializes a new <see cref="Frame"/> containing a single <see cref="TextureWrapper"/>.
    /// </summary>
    /// <param name="frame">The texture to add.</param>
    /// <param name="options">Optional image load options.</param>
    public Frame(TextureWrapper frame, ImageLoadOptions? options = null)
    {
        LoadOptions = options ?? new ImageLoadOptions();
        AddFrame(frame);
    }

    /// <summary>
    /// Initializes a new <see cref="Frame"/> containing multiple <see cref="TextureWrapper"/> elements.
    /// </summary>
    /// <param name="frames">List of textures to add.</param>
    /// <param name="options">Optional image load options.</param>
    public Frame(List<TextureWrapper> frames, ImageLoadOptions? options = null)
    {
        LoadOptions = options ?? new ImageLoadOptions();
        AddFrames(frames);
    }

    /// <summary>
    /// Creates a new <see cref="Frame"/> by copying another frame instance.
    /// Supports asynchronous loading if the source frame is still loading.
    /// </summary>
    /// <param name="frame">Source frame to copy.</param>
    /// <param name="options">Optional image load options.</param>
    public Frame(Frame frame, ImageLoadOptions? options = null)
        : base(frame, false)
    {
        LoadOptions = frame.LoadOptions ?? options;

        RectMode = frame.RectMode;
        LoadOptions = frame.LoadOptions;
        MaxFrameRect = frame.MaxFrameRect;

        if (LoadOptions is not null && LoadOptions.LoadAsync)
            _ = LoadObjectAsync(frame);
        else
            LoadObject(frame);
    }


    /// <summary>
    /// Updates the <see cref="MaxFrameRect"/> to include the specified rectangle.
    /// Ensures that <see cref="MaxFrameRect"/> always encompasses all frames in the animation.
    /// </summary>
    /// <param name="newRect">The rectangle of a newly added or updated frame to include.</param>
    public void UpdateMaxRect(IntRect newRect)
    {
        if (CountElements == 1)
        {
            MaxFrameRect = newRect;
            return;
        }

        int left = Math.Min(MaxFrameRect.Left, newRect.Left);
        int top = Math.Min(MaxFrameRect.Top, newRect.Top);

        int right = Math.Max(MaxFrameRect.Left + MaxFrameRect.Width, newRect.Left + newRect.Width);
        int bottom = Math.Max(MaxFrameRect.Top + MaxFrameRect.Height, newRect.Top + newRect.Height);

        MaxFrameRect = new IntRect(left, top, right - left, bottom - top);
    }

    #region Load

    /// <summary>
    /// Waits asynchronously for another frame to finish loading, then copies its elements into this instance.
    /// Returns immediately if the source frame is already loaded or on timeout.
    /// </summary>
    /// <param name="frame">The source frame to load from.</param>
    public async Task LoadObjectAsync(Frame frame)
    {
        await Task.Run(async () =>
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();

            while (!frame.IsLoaded)
            {
                if (LoadingTimeoutMs > 0 && sw.ElapsedMilliseconds >= LoadingTimeoutMs)
                    return;

                if (LoadingPollDelayMs > 0)
                    await Task.Delay(LoadingPollDelayMs);
                else
                    await Task.Yield();
            }
        });

        if (!frame.IsLoaded)
            return;

        AddFrames(frame.GetElements());
        IsLoaded = true;
    }

    /// <summary>
    /// Loads elements from another frame synchronously, or asynchronously if not yet loaded.
    /// </summary>
    /// <param name="frame">The source frame to load from.</param>
    public void LoadObject(Frame frame)
    {
        if (!frame.IsLoaded)
        {
            _ = LoadObjectAsync(frame);
            return;
        }

        AddFrames(frame.GetElements());
        IsLoaded = true;
    }

    #endregion

    #region Add

    #region Frame
    /// <summary>
    /// Adds a single texture to the frame.
    /// </summary>
    /// <param name="frame">Texture to add.</param>
    public void AddFrame(TextureWrapper frame)
    {
        Elements.Add(frame);
        UpdateMaxRect(frame.Rect);

        SetCurrentElement(0);
        IsLoaded = true;
    }

    /// <summary>
    /// Asynchronously adds a single texture to the frame.
    /// </summary>
    /// <param name="frameTask">Task that resolves to a texture.</param>
    public async Task AddFrameAsync(Task<TextureWrapper> frameTask)
    {
        var frame = await frameTask;
        AddFrame(frame);
    }
    #endregion

    #region Frames

    /// <summary>
    /// Adds multiple textures to the frame.
    /// </summary>
    /// <param name="frames">List of textures to add.</param>
    public void AddFrames(List<TextureWrapper> frames)
    {
        Elements.AddRange(frames);

        foreach (var frame in frames)
            UpdateMaxRect(frame.Rect);

        SetCurrentElement(0);
        IsLoaded = true;
    }

    /// <summary>
    /// Asynchronously adds multiple textures to the frame.
    /// </summary>
    /// <param name="framesTask">Task that resolves to a list of textures.</param>
    public async Task AddFramesAsync(Task<List<TextureWrapper>> framesTask)
    {
        var frames = await framesTask;
        AddFrames(frames);
    }

    #endregion

    /// <summary>
    /// Loads textures from file paths and adds them to the frame.
    /// </summary>
    /// <param name="imageLoader">Optional image loading options.</param>
    /// <param name="elements">Paths to texture files.</param>
    public void AddFromFile(ImageLoadOptions? imageLoader = null, params string[] elements)
    {
        foreach (var element in elements)
        {
            var textures = ImageLoader.Load(imageLoader, element);
            foreach (var texture in textures)
                UpdateMaxRect(texture.Rect);

            AddElements(textures);
        }
    }

    #endregion


    /// <summary>
    /// Updates the animation state using a default sprite angle of <c>0</c>.
    /// This method is typically used when the animation does not depend on
    /// the object's viewing direction.
    /// </summary>
    public override void Update()
    {
        Update(0);
    }

    /// <summary>
    /// Updates the animation state based on the provided sprite angle.
    /// Uses the configured <see cref="BaseSelector"/> to determine the next frame.
    /// If the selected frame is not yet loaded, a placeholder texture is applied.
    /// </summary>
    /// <param name="spriteAngle">
    /// The viewing angle of the sprite in radians, used by selectors
    /// to determine which frame should be displayed.
    /// </param>
    public void Update(float spriteAngle)
    {
        var selectFrame = BaseSelector?.SetNextElement(this, spriteAngle);
        if (selectFrame != null && !selectFrame.IsLoaded)
            CurrentElement = TextureWrapper.Placeholder;
    }
}
