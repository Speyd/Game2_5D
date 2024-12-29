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

namespace ObstacleLib.TexturedWallLib.Determine
{
    static internal class DetermineSide
    {
        static float sinAngle = 0;
        static float cosAngle = 0;

        static private Vector2f DetermineCornerWallSide(TexturedWall wall, Entity entity)
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

            return new Vector2f(hitX, hitY);
        }

        static private ObjectSide DetermineMineWallSide(TexturedWall wall, Entity entity)
        {
            if (entity.Y >= wall.Top && entity.Y <= wall.Bottom)
            {
                if (cosAngle > 0 && entity.X <= wall.Right)
                    return ObjectSide.Right;
                else if (cosAngle < 0 && entity.X >= wall.Left)
                    return ObjectSide.Left;
            }

            if (entity.X >= wall.Left && entity.X <= wall.Right)
            {
                if (sinAngle > 0 && entity.Y <= wall.Bottom)
                    return ObjectSide.Bottom;
                else if (sinAngle < 0 && entity.Y >= wall.Top)
                    return ObjectSide.Top;
            }

            return ObjectSide.Error;
        }

        static public ObjectSide DetermineWallAllSides(TexturedWall wall, Entity entity)
        {
            cosAngle = entity.Direction.X;
            sinAngle = entity.Direction.Y;


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
