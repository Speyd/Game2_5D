using EntityLib;
using Render.InterfaceRender;
using ScreenLib;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObstacleLib.TexturedWallLib;
using TextureLib;
using HitBoxLib;
using HitBoxLib.PositionObject;

namespace ObstacleLib.TexturedWallLib.Determine
{
    static internal class DetermineSide
    {
        static float sinAngle = 0;
        static float cosAngle = 0;

        static float left = 0f;
        static float right = 0f;
        static float bottom = 0f;
        static float top = 0f;


        static private Vector2f DetermineCornerWallSide(TexturedWall wall, Entity entity)
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

            float hitX = (float)entity.X.Axis + tempValue * cosAngle - (float)wall.X.Axis;
            float hitY = (float)entity.Y.Axis + tempValue * sinAngle - (float)wall.Y.Axis;

            return new Vector2f(hitX, hitY);
        }

        static private ObjectSide DetermineMineWallSide(TexturedWall wall, Entity entity)
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

        static public ObjectSide DetermineWallAllSides(TexturedWall wall, Entity entity)
        {
            cosAngle = entity.Direction.X;
            sinAngle = entity.Direction.Y;

            left = (float)(wall.HitBox[HitBoxSideType.Left]?.Side ?? 0f);
            right = (float)(wall.HitBox[HitBoxSideType.Right]?.Side ?? 0f);
            bottom = (float)(wall.HitBox[HitBoxSideType.Bottom]?.Side ?? 0f);
            top = (float)(wall.HitBox[HitBoxSideType.Top]?.Side ?? 0f);


            ObjectSide wallDetermine = ObjectSide.Error;
            wallDetermine = DetermineMineWallSide(wall, entity);
            if (wallDetermine != ObjectSide.Error)
                return wallDetermine;

            Vector2f cornerHit = DetermineCornerWallSide(wall, entity);
            if (cornerHit.X > cornerHit.Y)
            {
                cornerHit.X /= Screen.Setting.Tile;
                if (cornerHit.X < cornerHit.Y)
                    return ObjectSide.Left;
                else
                    return ObjectSide.Bottom;
            }
            else
            {
                cornerHit.Y /= Screen.Setting.Tile;
                if (cornerHit.X > cornerHit.Y)
                    return ObjectSide.Top;
                else
                    return ObjectSide.Right;
            }
        }

        static public ObjectSide DetermineWallAllSides(TexturedWall wall, Entity entity, double addAngle)
        {
            cosAngle = (float)Math.Cos(addAngle);
            sinAngle = (float)Math.Sin(addAngle);

            left = (float)(wall.HitBox[HitBoxSideType.Left]?.Side ?? 0f);
            right = (float)(wall.HitBox[HitBoxSideType.Right]?.Side ?? 0f);
            bottom = (float)(wall.HitBox[HitBoxSideType.Bottom]?.Side ?? 0f);
            top = (float)(wall.HitBox[HitBoxSideType.Top]?.Side ?? 0f);


            ObjectSide wallDetermine = ObjectSide.Error;
            wallDetermine = DetermineMineWallSide(wall, entity);

            if (wallDetermine != ObjectSide.Error)
                return wallDetermine;

            Vector2f cornerHit = DetermineCornerWallSide(wall, entity);
            if (cornerHit.X > cornerHit.Y)
            {
                cornerHit.X /= Screen.Setting.Tile;
                if (cornerHit.X < cornerHit.Y)
                    return ObjectSide.Left;
                else
                    return ObjectSide.Bottom;
            }
            else
            {
                cornerHit.Y /= Screen.Setting.Tile;
                if (cornerHit.X > cornerHit.Y)
                    return ObjectSide.Top;
                else
                    return ObjectSide.Right;
            }
        }
    }
}
