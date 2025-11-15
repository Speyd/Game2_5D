using AnimationLib.Enum;
using AnimationLib.Selector;
using System.Diagnostics;


namespace AnimationLib.Core.Elements;
/// <summary>
/// Provides a base implementation for animations composed of a sequence of elements.
/// Handles indexing, playback mode, speed, timing, and managing animation frames.
/// </summary>
/// <typeparam name="T">The type of elements used in the animation.</typeparam>
public abstract class AnimationBase<T> : IAnimation<T>
{
    /// <summary>
    /// List of animation elements. Initialized once and can be modified by derived classes.
    /// </summary>
    public virtual List<T> Elements { protected get; init; } = new();

    /// <summary>
    /// Gets the number of elements in the animation.
    /// </summary>
    public int CountElements => Elements.Count;

    /// <summary>
    /// Gets the currently active animation element.
    /// </summary>
    public virtual T? CurrentElement { get; protected set; }


    /// <summary>
    /// Backing field for <see cref="Index"/> property.
    /// </summary>
    protected int _index = 0;

    /// <summary>
    /// Gets or sets the index of the current animation element.
    /// Setting this value updates <see cref="CurrentElement"/>.
    /// </summary>
    public virtual int Index
    {
        get => _index;
        set
        {
            _index = value;
            SetCurrentElement(value);
        }
    }

    /// <summary>
    /// Playback speed of the animation, typically frames per update cycle.
    /// </summary>
    public virtual int SpeedAnimation { get; set; } = 5;

    /// <summary>
    /// Optional stopwatch for time-based animation control.
    /// </summary>
    public Stopwatch? Stopwatch { get; set; } = null;

    /// <summary>
    /// Stores the timestamp of the last frame update.
    /// </summary>
    public long LastFrameTime { get; set; } = 0;

    /// <summary>
    /// Backing field for <see cref="PlayMode"/> property.
    /// </summary>
    protected PlayMode _playMode = PlayMode.Loop;

    /// <summary>
    /// Controls how the animation behaves when reaching the end of the sequence.
    /// Changing this resets <see cref="IsFinishMode"/>.
    /// </summary>
    public virtual PlayMode PlayMode
    {
        get => _playMode;
        set
        {
            _playMode = value;
            IsFinishMode = false;
        }
    }

    /// <summary>
    /// Indicates whether the animation has finished based on the current play mode.
    /// </summary>
    public virtual bool IsFinishMode { get; set; } = false;

    /// <summary>
    /// Optional frame selector used to refine frame selection within the current frame.
    /// </summary>
    public virtual IElementSelector? BaseSelector { get; set; } = null;

    /// <summary>
    /// Creates a new animation by copying configuration from an existing instance.
    /// Elements are duplicated into the new instance.
    /// </summary>
    /// <param name="animationBase">The existing animation to copy from.</param>
    public AnimationBase(AnimationBase<T> animationBase, bool addElements = true)
    {
        SpeedAnimation = animationBase.SpeedAnimation;
        PlayMode = animationBase.PlayMode;
        BaseSelector = animationBase.BaseSelector;

        if(addElements)
            AddElements(Elements);
    }

    /// <summary>
    /// Initializes an empty animation.
    /// </summary>
    public AnimationBase()
    { }

    /// <summary>
    /// Sets the current animation element by index.
    /// Index is clamped to valid range; if no elements exist, resets to default.
    /// </summary>
    public virtual void SetCurrentElement(int index)
    {
        if (CountElements == 0)
        {
            _index = -1;
            CurrentElement = default;
            return;
        }

        if (index < 0 || index >= CountElements)
            index = 0;

        _index = index;
        CurrentElement = Elements[index];
    }

    /// <summary>
    /// Sets the current element by matching the given value within the elements list.
    /// If not found, the current element becomes <c>null</c>.
    /// </summary>
    public virtual void SetCurrentElement(T? element)
    {
        int index = Elements.FindIndex(el => Equals(el, element));

        if (index != -1)
        {
            _index = index;
            CurrentElement = Elements[index];
        }
        else
        {
            _index = -1;
            CurrentElement = default;
        }
    }

    /// <summary>
    /// Returns the element at the specified index, or <c>null</c> if out of range.
    /// </summary>
    public virtual T? GetElement(int index)
    {
        if (index < 0 || index >= CountElements)
            return default;

        return Elements[index];
    }

    /// <summary>
    /// Returns a list of all animation elements.
    /// </summary>
    public virtual List<T> GetElements() => Elements;

    /// <summary>
    /// Adds a new element to the animation.
    /// </summary>
    public virtual void AddElement(T element)
    {
        Elements.Add(element);
    }

    /// <summary>
    /// Adds multiple elements to the animation.
    /// </summary>
    public virtual void AddElements(IEnumerable<T> elements)
    {
        Elements.AddRange(elements);
    }

    /// <summary>
    /// Updates the animation state for the current tick, applying
    /// time-based playback, frame progression, and selector logic.
    /// Derived classes must implement how the animation advances.
    /// </summary>
    public abstract void Update();
}
