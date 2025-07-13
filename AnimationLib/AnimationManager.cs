namespace AnimationLib;
using System.Diagnostics;
using TextureLib.Textures;

/// <summary>
/// Class for handling the current frame. Defines the frame for animation
/// </summary>
public static class AnimationManager
{
    /// Base time unit in milliseconds used for animation timing calculations.
    /// Typically represents the duration of one second (1000 milliseconds).
    /// </summary>
    public const float baseMilliseconds = 1000.0f;

    /// <summary>
    /// Selects a specific frame (animation) to render
    /// </summary>
    public static void TextureAnimation(AnimationState state)
    {
        if (state.Stopwatch == null)
        {
            state.Stopwatch = Stopwatch.StartNew();
            state.LastFrameTime = 0;
        }

        double millisecondsPerFrame = baseMilliseconds / state.Speed;

        long elapsed = state.Stopwatch.ElapsedMilliseconds;
        if (elapsed - state.LastFrameTime >= millisecondsPerFrame)
        {
            state.Index = (state.Index + 1) % state.CountFrame;
            state.LastFrameTime = elapsed;
            if (state.CountFrame > 0)
            {
                var frame = state.Frames[state.Index];
                state.CurrentFrame = !frame.IsLoaded ? TextureWrapper.Placeholder : state.Frames[state.Index];
            }
            else
                state.CurrentFrame = TextureWrapper.Placeholder;
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

        int totalDirections = state.CountFrame;
        if (totalDirections == 0) return;

        double sectorSize = 360.0 / totalDirections;

        int textureIndex = (int)(spriteDegreeAngle / sectorSize) % totalDirections;

        var frame = state.Frames[(totalDirections - 1 - textureIndex + totalDirections) % totalDirections];
        state.CurrentFrame = !frame.IsLoaded? TextureWrapper.Placeholder: frame;

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
        if (state.IsAnimation && state.CountFrame > 1)
            TextureAnimation(state);
        else if(!state.IsAnimation && state.CountFrame > 0)
            TextureNonAnimation(state, spriteAngle);
        else
            state.CurrentFrame = TextureWrapper.Placeholder;
    }
}
