using EntityLib;
using ObstacleLib.Render.Texture;
using Render.InterfaceRender;
using Render.ZBufferRender;
using ScreenLib;
using SFML.Graphics;
using SFML.System;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapLib.Obstacles.DiversityObstacle.SpriteLib.Render
{
    internal class RenderSpriteOpertion
    {

        public double CalculationSpriteAngle(double playerAngle, double spriteAngle)
        {
            double angleDifference = spriteAngle - playerAngle;
            if (angleDifference > Math.PI)
                angleDifference -= 2 * Math.PI;
            if (angleDifference < -Math.PI)
                angleDifference += 2 * Math.PI;

            return angleDifference;
        }
        public void TextureAnimation(SpriteObstacle sprite)
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

        public void TextureNonAnimation(SpriteObstacle sprite, double spriteAngle)
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
        public void DefiningDesiredSprite(SpriteObstacle sprite, double spriteAngle)
        {
            if (sprite.CurrentAnimation.IsAnimation)
                TextureAnimation(sprite);
            else
                TextureNonAnimation(sprite, spriteAngle);
        }
        public double CalculationAngularDistance(SpriteObstacle sprite, Entity player)
        {
            double dx = sprite.WallX - player.GetEntityX();
            double dy = sprite.WallY - player.GetEntityY();
            sprite.Distance = Math.Sqrt(dx * dx + dy * dy);

            double spriteAngle = Math.Atan2(dy, dx);

            return spriteAngle;
        }

        public void DrawSprite(Screen screen, SpriteObstacle sprite, double verticalAngle, int x, int height)
        //verticalAngle - entity; x - sprite position; height - spriteHeight
        {
            if (sprite.CurrentRenderTexture is null)
                return;

            float scaledHeight = sprite.RenderSprite.GetGlobalBounds().Height;
            scaledHeight += (float)(sprite.setting.ShiftCubedZ * (sprite.CurrentRenderTexture.TextureHeight / sprite.Distance));


            float y = sprite.NormalizePositionY(screen, verticalAngle, scaledHeight / 2);

            sprite.RenderSprite = new SFML.Graphics.Sprite(sprite.CurrentRenderTexture.Texture);
            sprite.BlackoutObstacle(sprite.Distance);

            sprite.RenderSprite.Position = new Vector2f(x, y);

            sprite.RenderSprite.Scale = new Vector2f
                (
                (float)height / sprite.CurrentRenderTexture.TextureWidth,
                (float)height / sprite.CurrentRenderTexture.TextureHeight
                );

            ZBuffer.AddToZBuffer(sprite.RenderSprite, sprite.Distance);
        }
    }
}
