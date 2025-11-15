using AnimationLib.Core.Elements;
using AnimationLib.Core.Utils;
using NGenerics.DataStructures.General;
using TextureLib.Loader;
using TextureLib.Textures;

namespace AnimationLib.Core;
/// <summary>
/// Manages multiple <see cref="AnimationClip"/> instances and controls their playback.
/// Supports prioritization, switching between animations, and retrieving the current frame or texture.
/// </summary>
public class Animator
{
    /// <summary>
    /// Default name used for a base animation.
    /// </summary>
    public static readonly string BaseName = "Base";

    /// <summary>
    /// Default priority for a base animation.
    /// </summary>
    public static readonly int BasePriority = 0;


    private readonly Dictionary<string, AnimationEntry> animationEntries = new();

    /// <summary>
    /// Gets the currently active animation entry.
    /// </summary>
    public AnimationEntry? CurrentAnimationEntry { get; private set; }


    /// <summary>
    /// Gets the currently active <see cref="AnimationClip"/>.
    /// </summary>
    public AnimationClip? CurrentAnimation => CurrentAnimationEntry?.Animation;

    /// <summary>
    /// Gets the current <see cref="Frame"/> of the active animation.
    /// </summary>
    public Frame? CurrentFrame => CurrentAnimation?.CurrentElement;

    /// <summary>
    /// Gets the current <see cref="TextureWrapper"/> of the active frame.
    /// </summary>
    public TextureWrapper? CurrentTexture => CurrentFrame?.CurrentElement;


    /// <summary>
    /// Initializes a new <see cref="Animator"/> with a single base animation loaded from file paths.
    /// </summary>
    /// <param name="paths">Paths to textures for the base animation.</param>
    public Animator(params string[] paths)
        : this(null, paths)
    { }

    /// <summary>
    /// Initializes a new <see cref="Animator"/> with a single base animation using custom image load options.
    /// </summary>
    /// <param name="options">Options controlling image loading behavior.</param>
    /// <param name="paths">Paths to textures for the base animation.</param>
    public Animator(ImageLoadOptions? options, params string[] paths)
    {
        if (paths.Length == 0)
            return;

        AddAnimation(BaseName, new AnimationClip(options, paths), BasePriority);
        BasePlay();
    }

    /// <summary>
    /// Initializes a new <see cref="Animator"/> with a specific <see cref="AnimationClip"/>.
    /// </summary>
    /// <param name="animationClip">The animation clip to add.</param>
    /// <param name="name">Optional name for the animation.</param>
    /// <param name="priority">Optional priority for the animation.</param>
    /// <param name="options">Options controlling image loading behavior.</param>
    public Animator(AnimationClip animationClip, string? name = null, int? priority = null, ImageLoadOptions? options = null)
    {
        AddAnimation(name ?? BaseName, animationClip, priority ?? 0, options);
        BasePlay();
    }

    /// <summary>
    /// Initializes a new <see cref="Animator"/> with a specific <see cref="AnimationClip"/>.
    /// </summary>
    /// <param name="textures">The list of textures to add.</param>
    /// <param name="name">Optional name for the animation.</param>
    /// <param name="priority">Optional priority for the animation.</param>
    /// <param name="options">Options controlling image loading behavior.</param>
    public Animator(List<TextureWrapper> textures, string? name = null, int? priority = null, ImageLoadOptions? options = null)
    {
        AddAnimation(name ?? BaseName, new AnimationClip(textures, options), priority ?? 0);
        BasePlay();
    }

    /// <summary>
    /// Initializes a new <see cref="Animator"/> with a specific <see cref="AnimationClip"/>.
    /// </summary>
    /// <param name="frame">The frame to add.</param>
    /// <param name="name">Optional name for the animation.</param>
    /// <param name="priority">Optional priority for the animation.</param>
    /// <param name="options">Options controlling image loading behavior.</param>
    public Animator(Frame frame, string? name = null, int? priority = null, ImageLoadOptions? options = null)
    {
        AddAnimation(name ?? BaseName, new AnimationClip(frame, options), priority ?? 0);
        BasePlay();
    }

    /// <summary>
    /// Initializes a new <see cref="Animator"/> by copying animations from another animator.
    /// </summary>
    /// <param name="animator">The source animator to copy animations from.</param>
    /// <param name="options">Optional image load options.</param>
    public Animator(Animator animator, ImageLoadOptions? options = null)
    {
        AddAnimations(animator.GetAnimationEntries(), options);

        if (animator.CurrentAnimationEntry is not null)
           CurrentAnimationEntry = GetAnimationEntry(animator.CurrentAnimationEntry.Name);
        else
            BigPriorityPlay();
    }


    private void BasePlay()
    {
        if (animationEntries.Count != 1)
            return;

        CurrentAnimationEntry = animationEntries.Values.FirstOrDefault();

        if (CurrentAnimationEntry is not null)
            Play(CurrentAnimationEntry.Name);
    }
    private void BigPriorityPlay()
    {
        if (animationEntries.Count == 0)
            return;
        else if (animationEntries.Count == 1)
        {
            BasePlay();
            return;
        }

        var highestPair = animationEntries
          .OrderByDescending(e => e.Value.Priority)
          .FirstOrDefault();

        CurrentAnimationEntry = highestPair.Value;
    }


    /// <summary>
    /// Adds a new animation to the animator.
    /// </summary>
    /// <param name="name">Name of the animation.</param>
    /// <param name="animation">The <see cref="AnimationClip"/> instance.</param>
    /// <param name="priority">Priority of the animation.</param>
    public void AddAnimation(string name, AnimationClip animation, int priority, ImageLoadOptions? options = null)
    {
        animationEntries[name] = new AnimationEntry(name, animation, priority, options);
    }

    /// <summary>
    /// Adds an <see cref="AnimationEntry"/> to the animator, optionally creating a new copy if <see cref="ImageLoadOptions.CreateNew"/> is true.
    /// </summary>
    /// <param name="animationEntry">The animation entry to add.</param>
    /// <param name="options">Optional image load options.</param>
    public void AddAnimation(AnimationEntry animationEntry, ImageLoadOptions? options = null)
    {
        if (options is null || options is not null && !options.CreateNew)
            animationEntries[animationEntry.Name] = animationEntry;
        else if (options is not null)
            animationEntries[animationEntry.Name] = new AnimationEntry(animationEntry, options);
    }

    /// <summary>
    /// Adds multiple <see cref="AnimationEntry"/> objects to the animator.
    /// </summary>
    /// <param name="animationEntry">List of animation entries.</param>
    /// <param name="options">Optional image load options.</param>
    public void AddAnimations(List<AnimationEntry> animationEntry, ImageLoadOptions? options = null)
    {
        foreach (var animation in animationEntry)
        {
            AddAnimation(animation, options);
        }
    }

    /// <summary>
    /// Returns the <see cref="AnimationClip"/> with the specified name, or null if not found.
    /// </summary>
    public AnimationClip? GetAnimation(string name)
        => animationEntries.GetValueOrDefault(name)?.Animation;

    /// <summary>
    /// Returns the <see cref="AnimationEntry"/> with the specified name, or null if not found.
    /// </summary>
    public AnimationEntry? GetAnimationEntry(string name)
        => animationEntries.GetValueOrDefault(name);

    /// <summary>
    /// Returns all <see cref="AnimationEntry"/> objects managed by this animator.
    /// </summary>
    public List<AnimationEntry> GetAnimationEntries()
        => animationEntries.Values.ToList();


    /// <summary>
    /// Starts playing the animation with the given name if its priority allows.
    /// </summary>
    /// <param name="name">Name of the animation to play.</param>
    public void Play(string name)
    {
        if (!animationEntries.TryGetValue(name, out var entry))
            return;

        if (entry.IsPlaying || entry.Priority < CurrentAnimationEntry?.Priority)
            return;

        CurrentAnimationEntry = entry;

        entry.IsPlaying = true;
        CurrentAnimation?.SetCurrentElement(0);
    }

    /// <summary>
    /// Stops the animation with the given name and optionally switches to the next highest-priority playing animation.
    /// </summary>
    /// <param name="name">Name of the animation to stop.</param>
    public void Stop(string name)
    {
        if (!animationEntries.TryGetValue(name, out var entry))
            return;


        entry.IsPlaying = false;
        if (CurrentAnimationEntry is null || entry.Priority < CurrentAnimationEntry?.Priority)
            return;


        var nextEntry = animationEntries.Values
         .Where(e => e.IsPlaying && e.Priority < entry.Priority)
         .OrderByDescending(e => e.Priority)
         .FirstOrDefault();


        CurrentAnimationEntry = nextEntry;
    }

    /// <summary>
    /// Updates the current animation by resolving its frames.
    /// </summary>
    /// <param name="spriteAngle">Optional angle parameter passed to frame selectors.</param>
    public void Update(float spriteAngle = 0)
    {
        CurrentAnimation?.Update(spriteAngle);
    }
}