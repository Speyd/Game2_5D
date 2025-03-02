using EntityLib;
using HitBoxLib.HitBoxSegment;
using ScreenLib;
using SFML.Graphics;
using SFML.System;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using EffectLib;
using static SFML.Window.Mouse;
using static System.Formats.Asn1.AsnWriter;
using Render.RenderAlgorithm;


namespace ObstacleLib.SpriteLib.Render;
internal static class RenderOperation
{
    public static void TextureAnimation(SpriteObstacle sprite)
    {
        var animation = sprite.CurrentAnimation;

        animation.Count -= 1;
        if (animation.Count <= 0)
        {
            animation.Index = (animation.Index + 1) % sprite.Textures.Count;
            animation.Count = sprite.CurrentAnimation.Speed;
        }

        if (sprite.Textures.Count > 0)
        {
            sprite.CurrentRenderTexture = sprite.Textures[animation.Index];
        }

        sprite.CurrentAnimation = animation;
    }
    public static void TextureNonAnimation(SpriteObstacle sprite, double spriteAngle)
    {
        double spriteDegreeAngle = spriteAngle * (180.0 / Math.PI);

        if (spriteDegreeAngle < 0)
            spriteDegreeAngle += 360;

        int totalDirections = sprite.Textures.Count;
        if (totalDirections == 0) return;

        double sectorSize = 360.0 / totalDirections;

        int textureIndex = (int)(spriteDegreeAngle / sectorSize) % totalDirections;
        sprite.CurrentRenderTexture = sprite.Textures[(totalDirections - 1 - textureIndex + totalDirections) % totalDirections];
    }
    public static void DefiningDesiredSprite(SpriteObstacle sprite, double spriteAngle)
    {
        if (sprite.CurrentAnimation.IsAnimation)
            TextureAnimation(sprite);
        else
            TextureNonAnimation(sprite, spriteAngle);
    }

    public static Vector2f GetPositionOnScreen(SpriteObstacle sprite, Entity entity, float height)
    {
        float x = sprite.WorldToScreenX(sprite.Angle, entity.DeltaAngle);
        float y = sprite.WorldToScreenY(entity.VerticalAngle) - (float)(sprite.Z.Axis / (sprite.Distance / Screen.Setting.Tile));

        return new Vector2f(x, y);
    }
    public static void DrawSprite(SpriteObstacle sprite, Entity entity, float height)
    {
        if (sprite.CurrentRenderTexture is null)
            return;

        sprite.RenderSprite = new SFML.Graphics.Sprite(sprite.CurrentRenderTexture.Texture);
        sprite.RenderSprite.Color = VisualEffectHelper.VisualEffect.TransformationColor(sprite.Distance);
        sprite.RenderSprite.Origin = new Vector2f(sprite.CurrentRenderTexture.Width / 2, sprite.CurrentRenderTexture.Height / 2);
        sprite.RenderSprite.Position = GetPositionOnScreen(sprite, entity, height);
        
        sprite.RenderSprite.Scale = new Vector2f
            (
            (float)height / sprite.CurrentRenderTexture.Width,
            (float)height / sprite.CurrentRenderTexture.Height
            );
        ZBuffer.AddToZBuffer(sprite.RenderSprite, sprite.Distance);
    }

}
