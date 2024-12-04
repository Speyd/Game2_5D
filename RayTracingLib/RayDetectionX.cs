using EntityLib;
using MapLib.Obstacles.DiversityObstacle.TexturedWallLib;
using MapLib.Obstacles.Texture;
using Render.InterfaceRender;
using ScreenLib;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracingLib.Detection
{
    static public class RayDetectionX
    {
        static float distanceToPoint = 0;
        static float distanceToWall = 0;

        static float sinAngle = 0;
        static float cosAngle = 0;
        static private Vector2f CalculateTextureHitPoint(TexturedWall wall, Entity entity)
        {
            float tempValue = float.MaxValue;

            if (cosAngle != 0)
            {
                float tVerticalLeft = (float)(wall.Left - entity.X) / cosAngle;
                if (tVerticalLeft >= 0)
                {
                    float hitYLeft = (float)entity.Y + tVerticalLeft * sinAngle;
                    if (hitYLeft >= wall.Top && hitYLeft <= wall.Bottom)
                        tempValue = tVerticalLeft;
                }

                float tVerticalRight = (float)(wall.Right - entity.X) / cosAngle;
                if (tVerticalRight >= 0)
                {
                    float hitYRight = (float)entity.Y + tVerticalRight * sinAngle;
                    if (hitYRight >= wall.Top && hitYRight <= wall.Bottom)
                        tempValue = Math.Min(tempValue, tVerticalRight);
                }
            }
            if (sinAngle != 0)
            {
                float tHorizontalTop = (float)(wall.Top - entity.Y) / sinAngle;
                if (tHorizontalTop >= 0)
                {
                    float hitXTop = (float)entity.X + tHorizontalTop * cosAngle;
                    if (hitXTop >= wall.Left && hitXTop <= wall.Right)
                        tempValue = Math.Min(tempValue, tHorizontalTop);
                }

                float tHorizontalBottom = (float)(wall.Bottom - entity.Y) / sinAngle;
                if (tHorizontalBottom >= 0)
                {
                    float hitXBottom = (float)entity.X + tHorizontalBottom * cosAngle;
                    if (hitXBottom >= wall.Left && hitXBottom <= wall.Right)
                        tempValue = Math.Min(tempValue, tHorizontalBottom);
                }
            }

            if (tempValue == float.MaxValue)
                return new Vector2f(-1, -1);

            float hitX = (float)entity.X + tempValue * cosAngle - (float)wall.X;
            float hitY = (float)entity.Y + tempValue * sinAngle - (float)wall.Y;
            distanceToPoint = tempValue;

            return new Vector2f(hitX, hitY);
        }

        static private TextureWallSide DetermineWallSide(TexturedWall wall, Entity entity)
        {
            if (entity.Y >= wall.Top && entity.Y <= wall.Bottom)
            {
                if (cosAngle > 0 && entity.X <= wall.Right)
                    return TextureWallSide.Right;
                else if (cosAngle < 0 && entity.X >= wall.Left)
                    return TextureWallSide.Left;
            }

            if (entity.X >= wall.Left && entity.X <= wall.Right)
            {
                if (sinAngle > 0 && entity.Y <= wall.Bottom)
                    return TextureWallSide.Bottom;
                else if (sinAngle < 0 && entity.Y >= wall.Top)
                    return TextureWallSide.Top;
            }

            return TextureWallSide.Error;
        }
        static private void SetTextureWall(ref TextureWallSide oldWallTexture, TextureWallSide newWallTexture)
        {
            if (oldWallTexture == TextureWallSide.Error)
                oldWallTexture = newWallTexture;
        }
        static private TextureWallSide RedefiningWallSides(TextureWallSide wallDetermine)
        {
            switch (wallDetermine)
            {
                case TextureWallSide.LeftCorner:
                    return TextureWallSide.Left;
                case TextureWallSide.RightCorner:
                    return TextureWallSide.Right;
                case TextureWallSide.TopCorner:
                    return TextureWallSide.Top;
                case TextureWallSide.BottomCorner:
                    return TextureWallSide.Bottom;
                default:
                    return wallDetermine;
            }
        }

        static private void CalculateDistanceToWall(TexturedWall wall, Entity entity, TextureWallSide wallDetermine)
        {
            double deltaX = wall.X - entity.X;
            double deltaY = wall.Y - entity.Y;
            distanceToWall = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

            if (wallDetermine == TextureWallSide.Top || wallDetermine == TextureWallSide.Left)
                distanceToWall -= -(Screen.Setting.Tile * 3);
        }

        static public HitPoint DetermineWallAllSides(TexturedWall wall, Entity entity)
        {
            sinAngle = (float)Math.Sin(entity.Angle);
            cosAngle = (float)Math.Cos(entity.Angle);


            TextureWallSide wallDetermine = TextureWallSide.Error;
            wallDetermine = DetermineWallSide(wall, entity);

            CalculateDistanceToWall(wall, entity, wallDetermine);


            Vector2f cornerHit = CalculateTextureHitPoint(wall, entity);
            if (cornerHit.X > cornerHit.Y)
            {
                cornerHit.X /= 100;
                if (cornerHit.X < cornerHit.Y)
                {
                    cornerHit.Y /= 100;
                    cornerHit.X = 0;
                    SetTextureWall(ref wallDetermine, TextureWallSide.LeftCorner);
                }
                else
                    SetTextureWall(ref wallDetermine, TextureWallSide.BottomCorner);
            }
            else
            {
                cornerHit.Y /= 100;
                if (cornerHit.X > cornerHit.Y)
                {
                    cornerHit.X /= 100;
                    cornerHit.Y = 0;
                    SetTextureWall(ref wallDetermine, TextureWallSide.TopCorner);
                }
                else
                    SetTextureWall(ref wallDetermine, TextureWallSide.RightCorner);
            }

            TextureWallSide textureWallDetermine = RedefiningWallSides(wallDetermine);
            return new HitPoint(cornerHit, distanceToPoint, distanceToWall, wallDetermine, textureWallDetermine);
        }
    }
}
