using AnimationLib.Core;

namespace AnimationLib;
/// <summary>
/// Defines a contract for objects that can be animated.
/// Provides access to an <see cref="Animator"/> to control
/// animation playback, current frames, and rendering behavior.
/// </summary>
public interface IAnimatable
{
    /// <summary>
    /// Gets the animator responsible for managing animations for this object.
    /// </summary>
    Animator Animation { get; }
}
