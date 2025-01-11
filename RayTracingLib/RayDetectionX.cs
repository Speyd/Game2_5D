using EntityLib;
using ObstacleLib;
using Render.InterfaceRender;
using ScreenLib;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextureLib;

namespace RayTracingLib.Detection
{
    static public class RayDetectionX
    {
        static float distanceToPoint = 0;
        static float distanceToWall = 0;

        static float sinAngle = 0;
        static float cosAngle = 0;
        static private Vector2f CalculateTextureHitPoint(Obstacle obstacle, Entity entity)
        {
            float tempValue = float.MaxValue;

            if (cosAngle != 0)
            {
                float tVerticalLeft = (float)(obstacle.Left - entity.X) / cosAngle;
                if (tVerticalLeft >= 0)
                {
                    float hitYLeft = (float)entity.Y + tVerticalLeft * sinAngle;
                    if (hitYLeft >= obstacle.Top && hitYLeft <= obstacle.Bottom)
                        tempValue = tVerticalLeft;
                }

                float tVerticalRight = (float)(obstacle.Right - entity.X) / cosAngle;
                if (tVerticalRight >= 0)
                {
                    float hitYRight = (float)entity.Y + tVerticalRight * sinAngle;
                    if (hitYRight >= obstacle.Top && hitYRight <= obstacle.Bottom)
                        tempValue = Math.Min(tempValue, tVerticalRight);
                }
            }
            if (sinAngle != 0)
            {
                float tHorizontalTop = (float)(obstacle.Top - entity.Y) / sinAngle;
                if (tHorizontalTop >= 0)
                {
                    float hitXTop = (float)entity.X + tHorizontalTop * cosAngle;
                    if (hitXTop >= obstacle.Left && hitXTop <= obstacle.Right)
                        tempValue = Math.Min(tempValue, tHorizontalTop);
                }

                float tHorizontalBottom = (float)(obstacle.Bottom - entity.Y) / sinAngle;
                if (tHorizontalBottom >= 0)
                {
                    float hitXBottom = (float)entity.X + tHorizontalBottom * cosAngle;
                    if (hitXBottom >= obstacle.Left && hitXBottom <= obstacle.Right)
                        tempValue = Math.Min(tempValue, tHorizontalBottom);
                }
            }

            if (tempValue == float.MaxValue)
                return new Vector2f(-1, -1);

            float hitX = (float)entity.X + tempValue * cosAngle - (float)obstacle.X;
            float hitY = (float)entity.Y + tempValue * sinAngle - (float)obstacle.Y;
            distanceToPoint = tempValue;

            return new Vector2f(hitX, hitY);
        }
        static private ObjectSide DetermineWallSide(Obstacle obstacle, Entity entity)
        {
            if (entity.Y >= obstacle.Top && entity.Y <= obstacle.Bottom)
            {
                if (cosAngle > 0 && entity.X <= obstacle.Right)
                    return ObjectSide.Right;
                else if (cosAngle < 0 && entity.X >= obstacle.Left)
                    return ObjectSide.Left;
            }

            if (entity.X >= obstacle.Left && entity.X <= obstacle.Right)
            {
                if (sinAngle > 0 && entity.Y <= obstacle.Bottom)
                    return ObjectSide.Bottom;
                else if (sinAngle < 0 && entity.Y >= obstacle.Top)
                    return ObjectSide.Top;
            }

            return ObjectSide.Error;
        }
        static private void SetTextureWall(ref ObjectSide oldWallTexture, ObjectSide newWallTexture)
        {
            if (oldWallTexture == ObjectSide.Error)
                oldWallTexture = newWallTexture;
        }
        static private ObjectSide RedefiningWallSides(ObjectSide wallDetermine)
        {
            switch (wallDetermine)
            {
                case ObjectSide.LeftCorner:
                    return ObjectSide.Left;
                case ObjectSide.RightCorner:
                    return ObjectSide.Right;
                case ObjectSide.TopCorner:
                    return ObjectSide.Top;
                case ObjectSide.BottomCorner:
                    return ObjectSide.Bottom;
                default:
                    return wallDetermine;
            }
        }
        static private void CalculateDistanceToWall(Obstacle obstacle, Entity entity, ObjectSide wallDetermine)
        {
            double deltaX = obstacle.X - entity.X;
            double deltaY = obstacle.Y - entity.Y;
            distanceToWall = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

            if (wallDetermine == ObjectSide.Top || wallDetermine == ObjectSide.Left)
                distanceToWall -= -(Screen.Setting.Tile * 3);
        }
        static public HitPoint DetermineWallAllSides(Obstacle obstacle, Entity entity)
        {
            cosAngle = entity.Direction.X;
            sinAngle = entity.Direction.Y;


            ObjectSide wallDetermine = ObjectSide.Error;
            wallDetermine = DetermineWallSide(obstacle, entity);

            CalculateDistanceToWall(obstacle, entity, wallDetermine);

            Vector2f cornerHit = CalculateTextureHitPoint(obstacle, entity);
            if (cornerHit.X > cornerHit.Y)
            {
                cornerHit.X /= Screen.Setting.Tile;
                if (cornerHit.X < cornerHit.Y)
                {
                    cornerHit.Y /= Screen.Setting.Tile;
                    cornerHit.X = 0;
                    SetTextureWall(ref wallDetermine, ObjectSide.LeftCorner);
                }
                else
                    SetTextureWall(ref wallDetermine, ObjectSide.BottomCorner);
            }
            else
            {
                cornerHit.Y /= Screen.Setting.Tile;
                if (cornerHit.X > cornerHit.Y)
                {
                    cornerHit.X /= Screen.Setting.Tile;
                    cornerHit.Y = 0;
                    SetTextureWall(ref wallDetermine, ObjectSide.TopCorner);
                }
                else
                    SetTextureWall(ref wallDetermine, ObjectSide.RightCorner);
            }

            ObjectSide textureWallDetermine = RedefiningWallSides(wallDetermine);
            return new HitPoint(cornerHit, distanceToPoint, distanceToWall, wallDetermine, textureWallDetermine);
        }
    }
}
