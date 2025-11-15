using ScreenLib;
using SFML.System;
using ProtoRender.RenderAlgorithm;
using ProtoRender.Object;
using EffectLib.EffectCore;
using SFML.Graphics;
using TextureLib.Textures;
using AnimationLib.Enum;


namespace ObstacleLib.SpriteLib.Render;
/// <summary>
/// Provides rendering operations for sprites, including screen position 
/// calculation, texture selection, transformation, and Z-buffer management.
/// </summary>
public static class RenderOperation
{ 
    /// <summary>
    /// Calculates the on-screen position of a sprite relative to the observer unit.
    /// Applies perspective correction, vertical positioning, and height adjustment.
    /// </summary>
    /// <param name="sprite">The sprite obstacle to project.</param>
    /// <param name="unit">The observer (player or camera) unit.</param>
    /// <param name="height">The calculated on-screen sprite height.</param>
    /// <returns>The 2D position of the sprite on the screen.</returns>
    public static Vector2f GetPositionOnScreen(SpriteObstacle sprite, IUnit unit, float height)
    {
        float distance = (float)(sprite.ViewDistance / Screen.Setting.Tile);

        float x = sprite.WorldToScreenX(sprite.AngleToObserver, unit.DeltaAngle);
        float differentHeight = (float)unit.Z.Axis * HitBoxLib.Operations.Render.MultHeight / distance;
        float y = sprite.WorldToScreenY(unit.VerticalAngle) - (float)(sprite.Z.Axis * HitBoxLib.Operations.Render.MultHeight / distance) + differentHeight;

        return new Vector2f(x, y);
    }

    /// <summary>
    /// Selects the appropriate texture frame for the sprite, 
    /// applies rendering effects (such as lighting or color tint),
    /// and updates the sprite's texture reference if necessary.
    /// </summary>
    /// <param name="sprite">The sprite obstacle whose texture should be updated.</param>
    /// <returns>The currently selected texture frame, or null if none.</returns>
    private static TextureWrapper? SelectTexture(SpriteObstacle sprite)
    {
        float normalizedDistance = (float)sprite.Distance / Screen.Setting.Tile;

        SFML.Graphics.Color effectColor = 
            EffectUtils.ApplyEffect(sprite.Effect, Obstacle.BaseEffectColor, normalizedDistance) ??
            SFML.Graphics.Color.White;

        sprite.RenderSprite.Color = effectColor;

        var texture = sprite.Animation.CurrentTexture;
        if (sprite.RenderSprite.Texture != texture?.Texture)
            sprite.RenderSprite.Texture = texture?.Texture;

        return texture;
    }

    /// <summary>
    /// Updates the sprite's texture rectangle and origin point 
    /// based on the current animation frame or maximum frame rectangle.
    /// Ensures that the sprite is drawn centered.
    /// </summary>
    /// <param name="sprite">The sprite obstacle to transform.</param>
    /// <param name="frameRect">The rectangle of the current animation frame.</param>
    /// <param name="widthTexture">The width of the texture frame.</param>
    /// <param name="heightTexture">The height of the texture frame.</param>
    private static void Transformation(SpriteObstacle sprite, IntRect frameRect, uint widthTexture, uint heightTexture)
    {
        IntRect targetRect = sprite.Animation.CurrentFrame?.RectMode == FrameRectMode.UseMaxFrameRect
             ? sprite.Animation.CurrentFrame.MaxFrameRect
             : frameRect;

        if (sprite.RenderSprite.TextureRect != targetRect)
            sprite.RenderSprite.TextureRect = targetRect;

        Vector2f origin = new Vector2f(widthTexture / 2f, heightTexture / 2f);
        if (sprite.RenderSprite.Origin != origin)
            sprite.RenderSprite.Origin = origin;
    }

    /// <summary>
    /// Renders a sprite on the screen by selecting its texture frame, 
    /// transforming its geometry, calculating screen position, 
    /// scaling based on distance, and adding it to the Z-buffer.
    /// </summary>
    /// <param name="sprite">The sprite obstacle to draw.</param>
    /// <param name="unit">The observer (player or camera) unit.</param>
    /// <param name="height">The calculated on-screen sprite height.</param>
    public static void DrawSprite(SpriteObstacle sprite, IUnit unit, float height)
    {
        if (sprite.Animation.CurrentTexture is null)
            return;

        uint widthTexture = sprite.Animation.CurrentTexture.Width;
        uint heightTexture = sprite.Animation.CurrentTexture.Height;

        var currentFrame = SelectTexture(sprite);
        Transformation(sprite, currentFrame?.Rect ?? new IntRect(), widthTexture, heightTexture);

        sprite.RenderSprite.Position = GetPositionOnScreen(sprite, unit, height);      
        sprite.RenderSprite.Scale = new Vector2f
            (
            (float)height / widthTexture,
            (float)height / heightTexture
            );

        ZBuffer.AddToZBuffer(sprite.RenderSprite, sprite.ViewDistance);
    }

}