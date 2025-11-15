using AnimationLib.Core;

namespace AnimationLib.Selector;
/// <summary>
/// Defines a strategy for selecting the next frame or element in an animation.
/// </summary>
public interface IElementSelector
{
    /// <summary>
    /// Determines and sets the next element in the given animation sequence.
    /// </summary>
    /// <typeparam name="T">Type of the elements in the animation.</typeparam>
    /// <param name="element">The animation to operate on.</param>
    /// <param name="spriteAngle">
    /// Optional parameter, e.g., the angle of a sprite, that can influence frame selection.
    /// Default is 0.
    /// </param>
    /// <returns>The next element selected for the animation, or <c>null</c> if none is selected.</returns>
    T? SetNextElement<T>(IAnimation<T> element, float spriteAngle = 0);
}
