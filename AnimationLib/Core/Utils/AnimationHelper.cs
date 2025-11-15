using AnimationLib.Enum;

namespace AnimationLib.Core.Utils;
/// <summary>
/// Provides helper methods for animation logic.
/// </summary>
public static class AnimationHelper
{
    /// <summary>
    /// Determines whether an animation should advance to the next frame based on its play mode and state.
    /// </summary>
    /// <typeparam name="T">Type of elements in the animation.</typeparam>
    /// <param name="animation">The animation to check.</param>
    /// <param name="newIndex">The proposed next frame index.</param>
    /// <returns>
    /// <c>true</c> if the animation should advance to the new frame; 
    /// <c>false</c> if it should remain on the current frame.
    /// </returns>
    /// <remarks>
    /// Behavior varies by <see cref="PlayMode"/>:
    /// <list type="bullet">
    /// <item><see cref="PlayMode.Pause"/>: never advances, sets <see cref="IAnimation{T}.IsFinishMode"/> to true.</item>
    /// <item><see cref="PlayMode.Loop"/>: always advances, <see cref="IAnimation{T}.IsFinishMode"/> is false.</item>
    /// <item><see cref="PlayMode.Once"/>: advances until the last frame, then sets <see cref="IAnimation{T}.IsFinishMode"/> to true and stops.</item>
    /// </list>
    /// </remarks>
    public static bool ShouldAdvanceFrame<T>(IAnimation<T> animation, int newIndex)
    {
        if (animation.IsFinishMode)
            return false;

        switch (animation.PlayMode)
        {
            case PlayMode.Pause:
                animation.IsFinishMode = true;
                return false;

            case PlayMode.Loop:
                animation.IsFinishMode = false;
                return true;

            case PlayMode.Once:
                if (animation.Index == animation.CountElements - 1 &&
                   animation.Index != newIndex)
                {
                    animation.SetCurrentElement(newIndex);
                    animation.IsFinishMode = true;
                    return false;
                }
                break;
        }

        return true;
    }
}