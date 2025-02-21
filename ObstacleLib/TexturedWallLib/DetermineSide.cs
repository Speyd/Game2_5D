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
using Render.ResultAlgorithm;

namespace ObstacleLib.TexturedWallLib.Determine
{
    static internal class DetermineSide
    {

        static private Vector2f DetermineCornerWallSide(TexturedWall wall, Entity entity,
            float cosAngle, float sinAngle,
            float minX, float maxX, float maxY, float minY)
        {
            float tempValue = float.MaxValue;

            if (cosAngle != 0)
            {
                float tVerticalLeft = (float)(minX - entity.X.Axis) / cosAngle;
                if (tVerticalLeft >= 0)
                {
                    float hitYLeft = (float)entity.Y.Axis + tVerticalLeft * sinAngle;
                    if (hitYLeft >= minY && hitYLeft <= maxY)
                        tempValue = tVerticalLeft;
                }

                float tVerticalRight = (float)(maxX - entity.X.Axis) / cosAngle;
                if (tVerticalRight >= 0)
                {
                    float hitYRight = (float)entity.Y.Axis + tVerticalRight * sinAngle;
                    if (hitYRight >= minY && hitYRight <= maxY)
                        tempValue = Math.Min(tempValue, tVerticalRight);
                }
            }
            if (sinAngle != 0)
            {
                float tHorizontalTop = (float)(minY - entity.Y.Axis) / sinAngle;
                if (tHorizontalTop >= 0)
                {
                    float hitXTop = (float)entity.X.Axis + tHorizontalTop * cosAngle;
                    if (hitXTop >= minX && hitXTop <= maxX)
                        tempValue = Math.Min(tempValue, tHorizontalTop);
                }

                float tHorizontalBottom = (float)(maxY - entity.Y.Axis) / sinAngle;
                if (tHorizontalBottom >= 0)
                {
                    float hitXBottom = (float)entity.X.Axis + tHorizontalBottom * cosAngle;
                    if (hitXBottom >= minX && hitXBottom <= maxX)
                        tempValue = Math.Min(tempValue, tHorizontalBottom);
                }
            }

            if (tempValue == float.MaxValue)
                return new Vector2f(-1, -1);

            float hitX = (float)entity.X.Axis + tempValue * cosAngle - (float)wall.X.Axis;
            float hitY = (float)entity.Y.Axis + tempValue * sinAngle - (float)wall.Y.Axis;

            return new Vector2f(hitX, hitY);
        }

        static private ObjectSide DetermineMineWallSide(TexturedWall wall, Entity entity,
            float cosAngle, float sinAngle,
            float left, float right, float bottom, float top)
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

        static private ObjectSide Determine(TexturedWall wall, Entity entity, float cosAngle, float sinAngle)
        {
            float minX = (float)(wall.HitBox.MainHitBox[CoordinatePlane.X, SideSize.Smaller]?.Side ?? 0f);
            float maxX = (float)(wall.HitBox.MainHitBox[CoordinatePlane.X, SideSize.Larger]?.Side ?? 0f);
            float maxY = (float)(wall.HitBox.MainHitBox[CoordinatePlane.Y, SideSize.Larger]?.Side ?? 0f);
            float minY = (float)(wall.HitBox.MainHitBox[CoordinatePlane.Y, SideSize.Smaller]?.Side ?? 0f);


            ObjectSide wallDetermine = ObjectSide.Error;
            wallDetermine = DetermineMineWallSide(wall, entity, cosAngle, sinAngle, minX, maxX, maxY, minY);
            if (wallDetermine != ObjectSide.Error)
                return wallDetermine;

            Vector2f cornerHit = DetermineCornerWallSide(wall, entity, cosAngle, sinAngle, minX, maxX, maxY, minY);
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
        static public ObjectSide DetermineWallAllSides(TexturedWall wall, Entity entity, Result? result = null)
        {
            float cosAngle = (float)(result?.CosCarAngle ?? entity.Direction.X);
            float sinAngle = (float)(result?.SinCarAngle ?? entity.Direction.Y);

            return Determine(wall, entity, cosAngle, sinAngle);
        }
    }
}
