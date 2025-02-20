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
using SFML.Window;

namespace ObstacleLib.SpriteLib.Hitbox
{
    public static class CollisionHitbox
    {

        const double UpAdjustmentFactor = -0.3;
        const double DownAdjustmentFactor = 0.3;

        public static bool IsRayTouchesObjectX(Box hitBox, Entity entity, float currentRayX)
        {
            return currentRayX > hitBox[HitBoxSideType.Left]?.Side &&
                   currentRayX < hitBox[HitBoxSideType.Right]?.Side;
        }
        public static bool IsRayTouchesObjectY(Box hitBox, Entity entity, float currentRayY)
        {
            return currentRayY > hitBox[HitBoxSideType.Top]?.Side &&
                   currentRayY < hitBox[HitBoxSideType.Bottom]?.Side;
        }


        private static double CalculateHeightMult(SpriteObstacle sprite,
            Box hitBox, HitBoxSideType type,
            double baseTypeMult, double multTexture,
            double distance)
        {
            double offset = (hitBox[type]?.Offset ?? 0) / Screen.Setting.Tile;
            double mult = (baseTypeMult + offset) * Screen.ScreenRatio;
            mult += sprite.RatioZ / Screen.Setting.Tile + multTexture / 10 + distance / Screen.Setting.Tile;

            return mult;
        }
        private static double CalculateRayHitBounds(double bounds, double mult, double angleEntity)
        {
            return bounds + mult + angleEntity;
        }
        public static bool IsRayTouchesObjectZ(SpriteObstacle sprite, Box hitBox, Entity entity)
        {
            if (sprite.CurrentRenderTexture is null)
                return true;

           // Console.WriteLine(hitBox.Body[HitBoxSideType.Down]?.Side);
            double multTexture = IRayPassability.BaseRayPassObjectHeight / sprite.CurrentRenderTexture.Height;
            double distance = Math.Sqrt(Math.Pow(sprite.X.Axis - entity.X.Axis, 2) + Math.Pow(sprite.Y.Axis - entity.Y.Axis, 2)) / Screen.Setting.Tile;

            double Down = (sprite.Z.Axis) / distance;
            double Up = (sprite.Z.Axis) / distance;

            double angleEntity = -entity.VerticalAngle * distance - sprite.Z.Axis / Screen.Setting.Tile;

            double multUp = CalculateHeightMult(sprite, hitBox, HitBoxSideType.Up, UpAdjustmentFactor, multTexture, distance);
            double multDown = CalculateHeightMult(sprite, hitBox, HitBoxSideType.Down, DownAdjustmentFactor, multTexture, distance);

            double rayHitUp = CalculateRayHitBounds(Up, -multUp, angleEntity);
            double rayHitDown = CalculateRayHitBounds(Down, -multDown, angleEntity);

            return (rayHitDown >= Down) && (rayHitUp <= Up);
        }
    }
}
