using AnimationLib.Core.Elements;
using TextureLib.Loader;

namespace AnimationLib.Core.Utils;

/// <summary>
/// Represents a single animation instance along with its playback metadata.
/// Tracks the animation clip, its priority, and whether it is currently playing.
/// </summary>
public class AnimationEntry
{
    /// <summary>
    /// Gets the <see cref="AnimationClip"/> associated with this entry.
    /// </summary>
    public AnimationClip Animation { get; internal set; }

    /// <summary>
    /// Gets the priority of this animation entry.
    /// Higher priority animations can override lower priority ones.
    /// </summary>
    public int Priority { get; internal set; }

    /// <summary>
    /// Indicates whether this animation is currently playing.
    /// </summary>
    public bool IsPlaying { get; set; } = false;

    /// <summary>
    /// Gets the name of this animation entry.
    /// </summary>
    public string Name { get; internal set; }

    /// <summary>
    /// Initializes a new <see cref="AnimationEntry"/> with a name, animation clip, and priority.
    /// </summary>
    /// <param name="name">Name of the animation entry.</param>
    /// <param name="animation">The animation clip to associate with this entry.</param>
    /// <param name="priority">Priority of the animation entry.</param>
    /// <param name="options">Image load options controlling whether to create a new animation instance.</param>
    public AnimationEntry(string name, AnimationClip animation, int priority, ImageLoadOptions? options = null)
    {
        Name = name;
        Animation = options is not null && options.CreateNew ? new AnimationClip(animation, options) : animation;
        Priority = priority;
    }

    /// <summary>
    /// Initializes a new <see cref="AnimationEntry"/> by copying an existing entry.
    /// Optionally creates a new <see cref="AnimationClip"/> instance based on <see cref="ImageLoadOptions"/>.
    /// </summary>
    /// <param name="animationEntry">The existing animation entry to copy.</param>
    /// <param name="options">Image load options controlling whether to create a new animation instance.</param>
    public AnimationEntry(AnimationEntry animationEntry, ImageLoadOptions options)
    {
        Name = animationEntry.Name;
        Animation = options.CreateNew ? new AnimationClip(animationEntry.Animation, options) : animationEntry.Animation;
        Priority = animationEntry.Priority;
    }

    /// <summary>
    /// Initializes a new <see cref="AnimationEntry"/> by copying an existing entry.
    /// Optionally creates a new <see cref="AnimationClip"/> instance based on <see cref="ImageLoadOptions"/>.
    /// </summary>
    internal AnimationEntry()
    {}
}