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
using ProtoRender.RenderAlgorithm;
using ProtoRender.Object;

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


        sprite.RenderSprite = new SFML.Graphics.Sprite(sprite.Animation.CurrentFrame.Texture);
        sprite.RenderSprite.Color = VisualEffectHelper.VisualEffect.TransformationColor(sprite.Distance);
        sprite.RenderSprite.Origin = new Vector2f(widthTexture / 2, heightTexture / 2);
        sprite.RenderSprite.Position = GetPositionOnScreen(sprite, unit, height);
        
        sprite.RenderSprite.Scale = new Vector2f
            (
            (float)height / widthTexture,
            (float)height / heightTexture
            );
        ZBuffer.AddToZBuffer(sprite.RenderSprite, sprite.Distance);
    }

}
