using EntityLib;
using Render.InterfaceRender;
using Render.ZBufferRender;
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

namespace MapLib.Obstacles.DiversityObstacle.SpriteLib.Render
{
    internal static class RenderSpriteOpertion
    {

        public static double CalculationSpriteAngle(double playerAngle, double spriteAngle)
        {
            double angleDifference = spriteAngle - playerAngle;

            if (angleDifference > Math.PI)
                angleDifference -= 2 * Math.PI;
            if (angleDifference < -Math.PI)
                angleDifference += 2 * Math.PI;

            return angleDifference;
        }
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
        public static double CalculationAngularDistance(SpriteObstacle sprite, Entity player)
        {
            double dx = sprite.X - player.X;
            double dy = sprite.Y - player.Y;

            sprite.Distance = Math.Sqrt(dx * dx + dy * dy);
            return Math.Atan2(dy, dx); ;
        }

        public static void DrawSprite(SpriteObstacle sprite, double verticalAngle, double x, float height)
        {
            if (sprite.CurrentRenderTexture is null)
                return;
   
            float y = sprite.NormalizePositionY(verticalAngle, height / 2)
                - (float)(sprite.Z * Screen.ScreenHeight / Math.Max(sprite.Distance, 0.1));

            sprite.RenderSprite = new SFML.Graphics.Sprite(sprite.CurrentRenderTexture.Texture);
            sprite.BlackoutObstacle(sprite.Distance);

            sprite.RenderSprite.Origin = new Vector2f(sprite.CurrentRenderTexture.Width / 2, sprite.CurrentRenderTexture.Height / 2);
            sprite.RenderSprite.Position = new Vector2f((float)x, y);

            // Применяем масштабирование с учетом множителя масштаба
            sprite.RenderSprite.Scale = new Vector2f
                (
                (float)height / sprite.CurrentRenderTexture.Width,
                (float)height / sprite.CurrentRenderTexture.Height
                );

            // Добавляем спрайт в ZBuffer для правильного наложения объектов
            ZBuffer.AddToZBuffer(sprite.RenderSprite, sprite.Distance);
        }

    }
}
