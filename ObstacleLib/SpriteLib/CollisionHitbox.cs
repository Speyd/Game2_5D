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
using HitBoxLib;

namespace ObstacleLib.SpriteLib.Hitbox
{
    public static class CollisionHitbox
    {

        const double TopAdjustmentFactor = 1.2;
        const double BottomAdjustmentFactor = 0.1;

        public static bool IsRayTouchesObjectX(SpriteObstacle sprite, Entity entity, float currentRayX)
        {
            return currentRayX > sprite.HitBox[HitBoxSideType.Left]?.Side &&
                   currentRayX < sprite.HitBox[HitBoxSideType.Right]?.Side;
        }
        public static bool IsRayTouchesObjectY(SpriteObstacle sprite, Entity entity, float currentRayY)
        {
            return currentRayY > sprite.HitBox[HitBoxSideType.Top]?.Side &&
                   currentRayY < sprite.HitBox[HitBoxSideType.Bottom]?.Side;
        }


        private static double CalculateMultTop(SpriteObstacle sprite, double multTexture, double distance)
        {
            double topOffset = (sprite.HitBox[HitBoxSideType.DownSide]?.Side ?? 0) / Screen.Setting.Tile;
            double mult = (TopAdjustmentFactor + topOffset) * Screen.ScreenRatio;
            mult += sprite.RatioZ / Screen.Setting.Tile + multTexture / 10 + distance / Screen.Setting.Tile;

            return mult;
        }
        private static double CalculateMultBottom(SpriteObstacle sprite, double multTexture, double distance)
        {
            double bottomOffset = (sprite.HitBox[HitBoxSideType.DownSide]?.Side ?? 0) / Screen.Setting.Tile;
            double mult = (BottomAdjustmentFactor + bottomOffset) * Screen.ScreenRatio;
            mult = mult - sprite.RatioZ / Screen.Setting.Tile * 2 - multTexture / 10 + distance / Screen.Setting.Tile;

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
            double distance = Math.Sqrt(Math.Pow(sprite.X.Axis - entity.X.Axis, 2) + Math.Pow(sprite.Y.Axis - entity.Y.Axis, 2)) / Screen.Setting.Tile;

            double Bottom = (sprite.RatioZ - sprite.Scale / 2) / distance;
            double Top = (sprite.RatioZ + sprite.Scale / 2) / distance;

            double angleEntity = -entity.VerticalAngle * distance - sprite.RatioZ / 100;

            double multTop = CalculateMultTop(sprite, multTexture, distance);
            double multBottom = CalculateMultBottom(sprite, multTexture, distance);

            double rayHitTop = CalculateRayHitBounds(Top, -multTop, angleEntity);
            double rayHitBottom = CalculateRayHitBounds(Bottom, multBottom, angleEntity);

            return (rayHitBottom >= Bottom) && (rayHitTop <= Top);
        }
    }
}
