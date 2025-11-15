using AnimationLib.Core;
using AnimationLib.Core.Utils;
using AnimationLib.Enum;

namespace AnimationLib.Selector;
/// <summary>
/// Selects an animation frame based on the view angle of a sprite.
/// Useful for directional animations where each frame represents a different orientation.
/// </summary>
public class ViewAngleSelector : IElementSelector
{
    /// <summary>
    /// Determines and sets the next element in the animation based on the sprite's angle.
    /// </summary>
    /// <typeparam name="T">Type of elements in the animation.</typeparam>
    /// <param name="animation">The animation to operate on.</param>
    /// <param name="spriteAngle">
    /// The angle of the sprite in radians. Used to calculate which frame to select.
    /// </param>
    /// <returns>The current element after selecting the frame, or <c>null</c> if no advancement occurred.</returns>
    /// <remarks>
    /// Converts the sprite angle to degrees and maps it to one of the available frames.
    /// Accounts for animations with multiple directional frames and respects the <see cref="PlayMode"/> and <see cref="IAnimation{T}.IsFinishMode"/> logic via <see cref="AnimationHelper.ShouldAdvanceFrame"/>.
    /// </remarks>
    public T? SetNextElement<T>(IAnimation<T> animation, float spriteAngle = 0)
    {
        double spriteDegreeAngle = spriteAngle * (180.0 / Math.PI);

        if (spriteDegreeAngle < 0)
            spriteDegreeAngle += 360;

        int totalDirections = animation.CountElements;
        if (totalDirections == 0) return default;

        double sectorSize = 360.0 / totalDirections;

        int textureIndex = (int)(spriteDegreeAngle / sectorSize) % totalDirections;
        int newIndex = (totalDirections - 1 - textureIndex + totalDirections) % totalDirections;
        if (!AnimationHelper.ShouldAdvanceFrame(animation, newIndex))
            return default;

        var frame = animation.GetElement((totalDirections - 1 - textureIndex + totalDirections) % totalDirections);
        animation.SetCurrentElement(frame);

        return animation.CurrentElement;
    }
}