using AnimationLib.Selector;
using TextureLib.Loader;
using TextureLib.Textures;


namespace AnimationLib.Core.Elements;
/// <summary>
/// Represents a sequence of <see cref="Frame"/> objects that form an animation clip.
/// Provides functionality for resolving frames based on selectors and accessing textures.
/// </summary>
public class AnimationClip : AnimationBase<Frame>
{
    /// <summary>
    /// Gets the current <see cref="TextureWrapper"/> from the active frame.
    /// </summary>
    public TextureWrapper? CurrentTexture => CurrentElement?.CurrentElement;

    /// <summary>
    /// Optional frame selector used to refine frame selection within the current frame.
    /// </summary>
    public IElementSelector? FrameSelector { get; set; } = null;

    /// <summary>
    /// Initializes a new <see cref="AnimationClip"/> by loading frames from file paths.
    /// </summary>
    /// <param name="paths">Paths to frame textures.</param>
    public AnimationClip(params string[] paths)
        : this(null, paths)
    { }

    /// <summary>
    /// Initializes a new <see cref="AnimationClip"/> with specified loading options and file paths.
    /// </summary>
    /// <param name="options">Options controlling image loading behavior.</param>
    /// <param name="paths">Paths to frame textures.</param>
    public AnimationClip(ImageLoadOptions? options, params string[] paths)
    {
        if(paths.Length > 0)
            AddElement(new Frame(options, paths));
    }

    /// <summary>
    /// Initializes a new <see cref="AnimationClip"/> with specified loading options and file paths.
    /// </summary>
    /// <param name="textures">List of TextureWrapper to frame textures.</param>
    /// <param name="options">Options controlling image loading behavior.</param>
    public AnimationClip(List<TextureWrapper> textures, ImageLoadOptions? options = null)
    {
        AddElement(new Frame(textures, options));
    }

    /// <summary>
    /// Initializes a new <see cref="AnimationClip"/> with specified loading options and file paths.
    /// </summary>
    /// <param name="frame">class Frame</param>
    /// <param name="options">Options controlling image loading behavior.</param>
    public AnimationClip(Frame frame, ImageLoadOptions? options = null)
    {
        AddElement(new Frame(frame, options));
    }

    /// <summary>
    /// Creates a new <see cref="AnimationClip"/> by copying another clip.
    /// Frame selectors and elements are duplicated, and the current index is preserved.
    /// </summary>
    /// <param name="animationClip">The source animation clip to copy.</param>
    /// <param name="options">Optional image load options.</param>
    public AnimationClip(AnimationClip animationClip, ImageLoadOptions? options = null)
        : base(animationClip, false)
    {
        FrameSelector = animationClip.FrameSelector;

        AddElements(animationClip.GetElements(), options);
        Index = animationClip.Index;
    }

    /// <summary>
    /// Resolves frames based on <see cref="BaseSelector"/> and <see cref="FrameSelector"/>.
    /// If the selected frame is not loaded, the placeholder texture is used.
    /// </summary>
    public override void Update()
    {
        Update(0);
    }

    /// <summary>
    /// Resolves frames based on <see cref="BaseSelector"/> and <see cref="FrameSelector"/>.
    /// If the selected frame is not loaded, the placeholder texture is used.
    /// </summary>
    /// <param name="spriteAngle">Optional angle parameter for selectors.</param>
    public void Update(float spriteAngle = 0)
    {
        if (!UpdateCurrentElement(spriteAngle))
            return;

        if(CurrentElement.BaseSelector is not null)
        {
            CurrentElement.Update(spriteAngle);
            return;
        }

        var selectFrame = FrameSelector?.SetNextElement(CurrentElement, spriteAngle);
        if (selectFrame != null && !selectFrame.IsLoaded)
            CurrentElement.SetCurrentElement(TextureWrapper.Placeholder);
    }

    private bool UpdateCurrentElement(float spriteAngle)
    {
        if (BaseSelector is null)
            CurrentElement = GetElement(0);
        else
            BaseSelector?.SetNextElement(this, spriteAngle);

        if (CurrentElement is null)
            return false;

        return true;
    }

    /// <summary>
    /// Returns the texture at the specified index within the current frame.
    /// </summary>
    /// <param name="index">Index of the texture within the current frame.</param>
    /// <returns>The corresponding <see cref="TextureWrapper"/>, or <c>null</c> if unavailable.</returns>
    public TextureWrapper? GetTexture(int index) => CurrentElement?.GetElement(index);

    /// <summary>
    /// Adds a single frame to the clip using the specified <see cref="ImageLoadOptions"/>.
    /// If <c>CreateNew</c> is true in options, a new copy of the frame is created.
    /// </summary>
    /// <param name="frame">Frame to add.</param>
    /// <param name="options">Image loading options controlling behavior.</param>
    public void AddElement(Frame frame, ImageLoadOptions? options)
    {
        if (options is null || !options.CreateNew)
            Elements.Add(frame);
        else
            Elements.Add(new Frame(frame, options));
    }

    /// <summary>
    /// Adds multiple frames to the clip using the specified <see cref="ImageLoadOptions"/>.
    /// Each frame is added according to the <c>CreateNew</c> flag in options.
    /// </summary>
    /// <param name="frames">List of frames to add.</param>
    /// <param name="options">Image loading options controlling behavior.</param>
    public void AddElements(List<Frame> frames, ImageLoadOptions? options)
    {
        foreach (var frame in frames)
        {
            AddElement(frame, options);
        }
    }

}
