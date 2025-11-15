using AnimationLib.Core;
using AnimationLib.Core.Utils;
using AnimationLib.Enum;
using System.Diagnostics;

namespace AnimationLib.Selector;
/// <summary>
/// Implements a simple time-based frame selector for animations.
/// Advances frames according to the animation's speed and play mode.
/// </summary>
public class AnimationSelector : IElementSelector
{
    /// <summary>
    /// Base time in milliseconds used to calculate frame duration relative to <see cref="IAnimation{T}.SpeedAnimation"/>.
    /// </summary>
    public const float baseMilliseconds = 1000.0f;

    /// <summary>
    /// Selects and sets the next element in the animation based on elapsed time and play mode.
    /// </summary>
    /// <typeparam name="T">Type of elements in the animation.</typeparam>
    /// <param name="animation">The animation to advance.</param>
    /// <param name="spriteAngle">
    /// Optional parameter, e.g., the angle of a sprite, that can influence frame selection. Default is 0.
    /// </param>
    /// <returns>The current element after attempting to advance, or <c>null</c> if no advancement occurred.</returns>
    public T? SetNextElement<T>(IAnimation<T> animation, float spriteAngle = 0)
    {
        if (animation.Stopwatch == null)
        {
            animation.Stopwatch = Stopwatch.StartNew();
            animation.LastFrameTime = 0;
        }
        if (animation.SpeedAnimation <= 0)
        {
            return animation.CurrentElement;
        }

        double millisecondsPerFrame = baseMilliseconds / animation.SpeedAnimation;

        long elapsed = animation.Stopwatch.ElapsedMilliseconds;
        if (elapsed - animation.LastFrameTime >= millisecondsPerFrame)
        {
            int newIndex = (animation.Index + 1) % animation.CountElements;
            if (!AnimationHelper.ShouldAdvanceFrame(animation, newIndex))
                return default;

            animation.Index = newIndex;
            animation.LastFrameTime = elapsed;
            if (animation.CountElements > 0)
            {
                animation.SetCurrentElement(animation.Index);
            }
            else
                animation.SetCurrentElement(default(T));
        }

        return animation.CurrentElement;
    }
}