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
using HitBoxLib;
using HitBoxLib.PositionObject;

namespace RayTracingLib.Detection
{
    static public class RayDetectionX
    {
        static float distanceToPoint = 0;
        static float distanceToWall = 0;

        static float sinAngle = 0;
        static float cosAngle = 0;

        static float left = 0f;
        static float right = 0f;
        static float bottom = 0f;
        static float top = 0f;

        static private Vector2f CalculateTextureHitPoint(Obstacle obstacle, Entity entity)
        {
            float tempValue = float.MaxValue;

            if (cosAngle != 0)
            {
                float tVerticalLeft = (float)(left - entity.X.Axis) / cosAngle;
                if (tVerticalLeft >= 0)
                {
                    float hitYLeft = (float)entity.Y.Axis + tVerticalLeft * sinAngle;
                    if (hitYLeft >= top && hitYLeft <= bottom)
                        tempValue = tVerticalLeft;
                }

                float tVerticalRight = (float)(right - entity.X.Axis) / cosAngle;
                if (tVerticalRight >= 0)
                {
                    float hitYRight = (float)entity.Y.Axis + tVerticalRight * sinAngle;
                    if (hitYRight >= top && hitYRight <= bottom)
                        tempValue = Math.Min(tempValue, tVerticalRight);
                }
            }
            if (sinAngle != 0)
            {
                float tHorizontalTop = (float)(top - entity.Y.Axis) / sinAngle;
                if (tHorizontalTop >= 0)
                {
                    float hitXTop = (float)entity.X.Axis + tHorizontalTop * cosAngle;
                    if (hitXTop >= left && hitXTop <= right)
                        tempValue = Math.Min(tempValue, tHorizontalTop);
                }

                float tHorizontalBottom = (float)(bottom - entity.Y.Axis) / sinAngle;
                if (tHorizontalBottom >= 0)
                {
                    float hitXBottom = (float)entity.X.Axis + tHorizontalBottom * cosAngle;
                    if (hitXBottom >= left && hitXBottom <= right)
                        tempValue = Math.Min(tempValue, tHorizontalBottom);
                }
            }

            if (tempValue == float.MaxValue)
                return new Vector2f(-1, -1);

            float hitX = (float)entity.X.Axis + tempValue * cosAngle - (float)obstacle.X.Axis;
            float hitY = (float)entity.Y.Axis + tempValue * sinAngle - (float)obstacle.Y.Axis;
            distanceToPoint = tempValue;

            return new Vector2f(hitX, hitY);
        }
        static private ObjectSide DetermineWallSide(Obstacle obstacle, Entity entity)   
        {
            if (entity.Y.Axis >= top && entity.Y.Axis <= bottom)
            {
                if (cosAngle > 0 && entity.X.Axis <= right)
                    return ObjectSide.Right;
                else if (cosAngle < 0 && entity.X.Axis >= left)
                    return ObjectSide.Left;
            }

            if (entity.X.Axis >= left && entity.X.Axis <= right)
            {
                if (sinAngle > 0 && entity.Y.Axis <= bottom)
                    return ObjectSide.Bottom;
                else if (sinAngle < 0 && entity.Y.Axis >= top)
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
            double deltaX = obstacle.X.Axis - entity.X.Axis;
            double deltaY = obstacle.Y.Axis - entity.Y.Axis;
            distanceToWall = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

            if (wallDetermine == ObjectSide.Top || wallDetermine == ObjectSide.Left)
                distanceToWall -= -(Screen.Setting.Tile * 3);
        }
        static public HitPoint DetermineWallAllSides(Obstacle obstacle, Entity entity)
        {
            cosAngle = entity.Direction.X;
            sinAngle = entity.Direction.Y;

            left = (float)(obstacle.HitBox[HitBoxSideType.Left]?.Side ?? 0f);
            right = (float)(obstacle.HitBox[HitBoxSideType.Right]?.Side ?? 0f);
            bottom = (float)(obstacle.HitBox[HitBoxSideType.Bottom]?.Side ?? 0f);
            top = (float)(obstacle.HitBox[HitBoxSideType.Top]?.Side ?? 0f);


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
