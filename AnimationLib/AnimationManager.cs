namespace AnimationLib;
/// <summary>
/// Class for handling the current frame. Defines the frame for animation
/// </summary>
public static class AnimationManager
{
    /// <summary>
    /// Selects a specific frame (animation) to render
    /// </summary>
    public static void TextureAnimation(AnimationState state)
    {
        state.FrameCounter -= 1;
        if (state.FrameCounter <= 0)
        {
            state.Index = (state.Index + 1) % state.AmountFrame;
            state.FrameCounter = state.Speed;
        }

        if (state.AmountFrame > 0)
        {
            state.CurrentFrame = state.Frames[state.Index];
        }
    }
    /// <summary>
    /// Selects a specific frame depending on the viewing angle between the observer and the object
    /// </summary>
    public static void TextureNonAnimation(AnimationState state, double spriteAngle)
    {
        double spriteDegreeAngle = spriteAngle * (180.0 / Math.PI);

        if (spriteDegreeAngle < 0)
            spriteDegreeAngle += 360;

        int totalDirections = state.AmountFrame;
        if (totalDirections == 0) return;

        double sectorSize = 360.0 / totalDirections;

        int textureIndex = (int)(spriteDegreeAngle / sectorSize) % totalDirections;
        state.CurrentFrame = state.Frames[(totalDirections - 1 - textureIndex + totalDirections) % totalDirections];
    }

    /// <summary>
    /// Defines which sprite should be used depending on the animation state.
    /// If the animation is active and contains multiple frames, the animation is performed.
    /// Otherwise, a static frame is selected depending on the sprite angle.
    /// </summary>
    /// <param name="state">The animation state containing the frames.</param>
    /// <param name="spriteAngle">The angle of rotation of the sprite (used for static display).</param>
    public static void DefiningDesiredSprite(AnimationState state, double spriteAngle)
    {
        if (state.IsAnimation && state.AmountFrame > 1)
            TextureAnimation(state);
        else if(!state.IsAnimation && state.AmountFrame > 0)
            TextureNonAnimation(state, spriteAngle);
    }
}
