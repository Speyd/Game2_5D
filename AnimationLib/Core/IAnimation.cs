using AnimationLib.Enum;
using AnimationLib.Selector;
using System.Diagnostics;

namespace AnimationLib.Core;
/// <summary>
/// Defines the basic structure and behavior of an animation consisting of a sequence of elements.
/// Provides functionality for managing frames, playback state, speed,
/// and accessing or modifying the current animation element.
/// </summary>
/// <typeparam name="T">The type of elements contained in the animation.</typeparam>
public interface IAnimation<T>
{
    /// <summary>
    /// Collection of animation elements. Set during initialization.
    /// </summary>
    List<T> Elements { init; }

    /// <summary>
    /// Gets the total number of elements in the animation.
    /// </summary>
    int CountElements { get; }

    /// <summary>
    /// Gets the currently active element in the animation sequence.
    /// </summary>
    T? CurrentElement { get; }

    /// <summary>
    /// Gets or sets the index of the current animation element.
    /// </summary>
    int Index { get; internal set; }

    /// <summary>
    /// Playback speed of the animation, usually measured as frames per update cycle.
    /// </summary>
    int SpeedAnimation { get; set; }

    /// <summary>
    /// Optional stopwatch used for time-based animation control.
    /// </summary>
    Stopwatch? Stopwatch { get; set; }

    /// <summary>
    /// Timestamp of when the last frame was updated.
    /// </summary>
    long LastFrameTime { get; set; }

    /// <summary>
    /// Mode that determines how the animation behaves when reaching the last frame.
    /// </summary>
    PlayMode PlayMode { get; set; }

    /// <summary>
    /// Indicates whether the animation has entered its final state (used in Once or Pause modes).
    /// </summary>
    bool IsFinishMode { get; set; }

    /// <summary>
    /// Optional frame selector used to refine frame selection within the current frame.
    /// </summary>
    public IElementSelector? BaseSelector { get; set; }


    /// <summary>
    /// Returns an animation element at the specified index, or <c>null</c> if out of range.
    /// </summary>
    T? GetElement(int index);

    /// <summary>
    /// Returns a list containing all animation elements.
    /// </summary>
    List<T> GetElements();

    /// <summary>
    /// Adds a single element to the animation.
    /// </summary>
    void AddElement(T element);

    /// <summary>
    /// Adds a collection of elements to the animation.
    /// </summary>
    void AddElements(IEnumerable<T> elements);

    /// <summary>
    /// Sets the current element by index.
    /// </summary>
    void SetCurrentElement(int index);

    /// <summary>
    /// Sets the current element by value.
    /// </summary>
    void SetCurrentElement(T? element);

    /// <summary>
    /// Executes a full animation update cycle, including time tracking,
    /// frame progression, playback-mode handling, and selector-based
    /// element switching. Called once per animation tick.
    /// </summary>
    void Update();
}
