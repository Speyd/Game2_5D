using ScreenLib;
using SFML.System;
using EffectLib;
using ProtoRender.RenderAlgorithm;
using ProtoRender.Object;
using EffectLib.EffectCore;
using System.Drawing;
using SFML.Graphics;


namespace ObstacleLib.SpriteLib.Render;
internal static class RenderOperation
{
    public static Vector2f GetPositionOnScreen(SpriteObstacle sprite, IUnit unit, float height)
    {
        float distance = (float)(sprite.Distance / Screen.Setting.Tile);

        float x = sprite.WorldToScreenX(sprite.AngleToObserver, unit.DeltaAngle);
        float differentHeight = (float)unit.Z.Axis * HitBoxLib.Operations.Render.MultHeight / distance;
        float y = sprite.WorldToScreenY(unit.VerticalAngle) - (float)(sprite.Z.Axis * HitBoxLib.Operations.Render.MultHeight / distance) + differentHeight;

        return new Vector2f(x, y);
    }
    public static void DrawSprite(SpriteObstacle sprite, IUnit unit, float height)
    {
        if (sprite.Animation.CurrentFrame is null || sprite.Animation.CurrentFrame.Texture is null)
            return;

        uint widthTexture = sprite.Animation.CurrentFrame.Width;
        uint heightTexture = sprite.Animation.CurrentFrame.Height;


        SFML.Graphics.Color effectColor = EffectUtils.ApplyEffect(sprite.Effect, Obstacle.BaseEffectColor, (float)sprite.Distance / Screen.Setting.Tile) ?? SFML.Graphics.Color.White;
        sprite.RenderSprite.Color = effectColor;

        var frame = sprite.Animation.CurrentFrame;
        if (sprite.RenderSprite.Texture != frame.Texture)
            sprite.RenderSprite.Texture = frame.Texture;

        if (sprite.RenderSprite.TextureRect != sprite.Animation.MaxFrameRect)
            sprite.RenderSprite.TextureRect = sprite.Animation.MaxFrameRect;

        Vector2f origin = new Vector2f(widthTexture / 2f, heightTexture / 2f);
        if (sprite.RenderSprite.Origin != origin)
            sprite.RenderSprite.Origin = origin;

        sprite.RenderSprite.Position = GetPositionOnScreen(sprite, unit, height);
        
        sprite.RenderSprite.Scale = new Vector2f
            (
            (float)height / widthTexture,
            (float)height / heightTexture
            );
        ZBuffer.AddToZBuffer(sprite.RenderSprite, sprite.Distance);
    }

}