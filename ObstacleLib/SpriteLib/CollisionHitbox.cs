using EntityLib;
using Render.RenderInterface;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SFML.Window.Mouse;
using ScreenLib;
using Render.ResultAlgorithm;

namespace ObstacleLib.SpriteLib.Hitbox
{
    public static class CollisionHitbox
    {

        const double TopAdjustmentFactor = 1.2;
        const double BottomAdjustmentFactor = 0.1;

        public static bool IsRayTouchesObjectX(SpriteObstacle sprite, Entity entity, float currentRayX)
        {
            return currentRayX > sprite.Left && currentRayX < sprite.Right;
        }
        public static bool IsRayTouchesObjectY(SpriteObstacle sprite, Entity entity, float currentRayY)
        {
            return currentRayY > sprite.Top && currentRayY < sprite.Bottom;
        }


        private static double CalculateMultTop(SpriteObstacle sprite, double multTexture)
        {
            double mult = TopAdjustmentFactor * Screen.MultHeight / Screen.MultWidth;
            mult += sprite.Z / 100 + multTexture / 10;

            return mult;
        }
        private static double CalculateMultBottom(SpriteObstacle sprite, double multTexture)
        {
            double mult = BottomAdjustmentFactor * Screen.MultHeight / Screen.MultWidth;
            mult = mult - sprite.Z / 100 * 2 - multTexture / 10;

            return mult;
        }

        private static double CalculateRayHitBounds(double bounds, double mult, double angleEntity)
        {
            return bounds + mult + angleEntity;
        }

        public static bool IsRayTouchesObjectZ(SpriteObstacle sprite, Entity entity)
        {
            if (sprite.CurrentRenderTexture is null)
                return true;

            double multTexture = IRayPassability.BaseRayPassObjectHeight / sprite.CurrentRenderTexture.Height;
            double distance = Math.Sqrt(Math.Pow(sprite.X - entity.X, 2) + Math.Pow(sprite.Y - entity.Y, 2)) / Screen.Setting.Tile;  
            

            double Bottom = (sprite.Z - sprite.Scale / 2) / distance;
            double Top = (sprite.Z + sprite.Scale / 2) / distance;

            //double Z_ray = -entity.VerticalAngle * distance - sprite.Z / 100;
            //double multTop = 1.2 * Screen.MultHeight / Screen.MultWidth + sprite.Z / 100 + multTexture / 10;
            //double multBottom = 0.1 * Screen.MultHeight / Screen.MultWidth - sprite.Z / 100 * 2 - multTexture / 10;

            //double aTop = Top - multTop + Z_ray;
            //double aBottom = Bottom + multBottom + Z_ray;
            double angleEntity = -entity.VerticalAngle * distance - sprite.Z / 100;

            double multTop = CalculateMultTop(sprite, multTexture);
            double multBottom = CalculateMultBottom(sprite, multTexture);

            double rayHitTop = CalculateRayHitBounds(Top, -multTop, angleEntity);
            double rayHitBottom = CalculateRayHitBounds(Bottom, multBottom, angleEntity);


            //Console.WriteLine($"\nTop: {Top}");
            //Console.WriteLine($"Bottom: {Bottom}");
            //Console.WriteLine($"-----------------------------------");
            //Console.WriteLine($"multTexture: {multTexture}");
            //Console.WriteLine($"multBottom: {multBottom}");
            //Console.WriteLine($"aTop: {aTop}");
            //Console.WriteLine($"aBottom: {aBottom}");
            //Console.WriteLine($"Angle: {Z_ray}");

            //return (aBottom >= Bottom) && (aTop <= Top);
            return (rayHitBottom >= Bottom) && (rayHitTop <= Top);
            //if (playerBottomAngle >= Bottom && playerTopAngle <= Top)
            //    return true;
            //else
            //    return false;
        }
    }
}
