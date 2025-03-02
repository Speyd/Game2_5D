using ObstacleLib.SpriteLib.Animation;

namespace AnimationLib;
public static class AnimationManager
{
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

    public static void DefiningDesiredSprite(AnimationState state, double spriteAngle)
    {
        if (state.IsAnimation && state.AmountFrame > 1)
            TextureAnimation(state);
        else if(!state.IsAnimation && state.AmountFrame > 0)
            TextureNonAnimation(state, spriteAngle);
    }
}
