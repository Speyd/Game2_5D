using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapLib.Obstacles;
using EntityLib;
using MapLib;
using ScreenLib;
using Render.InterfaceRender;
using SFML.Graphics;
using MapLib.Obstacles.DiversityObstacle.SpriteLib;
using static SFML.Window.Mouse;


namespace RayTracingLib
{
    public static class Raycast
    {
        private static float startX;
        private static float tMaxX;
        private static float dx;

        private static float startY;
        private static float tMaxY;
        private static float dy;
        private static bool IsRayIntersectingWithSprite(SpriteObstacle sprite, Entity entity)
        {
            if (sprite.CurrentRenderTexture is null)
                return true;

            //-------------Z Coordinates------------
            float distanceToSprite = (float)Math.Sqrt(
            (sprite.X - entity.X) * (sprite.X - entity.X) +
            (sprite.Y - entity.Y) * (sprite.Y - entity.Y)
            );
            float entityViewZ = (float)(entity.CameraZ + Math.Tan(entity.VerticalAngle) * distanceToSprite);

            bool isCollidingZ = entityViewZ >= Math.Abs(sprite.Setting.ShiftCubedZ);


            //-------------X-Y Coordinates------------
            float currentRayX = startX + tMaxY * dx;
            float currentRayY = startY + tMaxX * dy;

            bool isCollidingX = currentRayX >= sprite.Left && currentRayX <= sprite.Right;
            bool isCollidingY = currentRayY >= sprite.Top && currentRayY <= sprite.Bottom;



            if ((isCollidingX || isCollidingY) == true && isCollidingZ == false)
                return false;
            else if ((isCollidingX || isCollidingY) == false && isCollidingZ == true)
                return false;
            else if ((isCollidingX || isCollidingY) == false && isCollidingZ == false)
                return false;
            else 
                return true;
        }

        private static bool CheckLumbago(List<Obstacle> obstacles, Entity entity)
        {
            foreach(var obstacle in obstacles)
            {
                if (obstacle is IWall)
                    return true;
                else if (obstacle is SpriteObstacle sprite)
                {
                    if (IsRayIntersectingWithSprite(sprite, entity))
                        return true;
                }
            }
            return false;
        }
        public static List<Obstacle> RaycastFun(Map map, Entity entity)
        {
             dx = (float)Math.Cos(entity.Angle);
             dy = (float)Math.Sin(entity.Angle);

            int tileSize = Screen.Setting.Tile;

             startX = (float)entity.X;
             startY = (float)entity.Y;

            int gridX = (int)(startX / tileSize) * tileSize;
            int gridY = (int)(startY / tileSize) * tileSize;

            int stepX = dx > 0 ? tileSize : -tileSize;
            int stepY = dy > 0 ? tileSize : -tileSize;

            float tDeltaX = Math.Abs(tileSize / dx);
            float tDeltaY = Math.Abs(tileSize / dy);


             tMaxX = dx > 0 ? (gridX + tileSize - startX) / dx : (startX - gridX) / -dx;
             tMaxY = dy > 0 ? (gridY + tileSize - startY) / dy : (startY - gridY) / -dy;

            while (true)
            {

                if (map.ObstaclesWithoutNull.ContainsKey((gridX, gridY)))
                {
                    if (CheckLumbago(map.ObstaclesWithoutNull[(gridX, gridY)], entity))
                        return map.Obstacles[(gridX, gridY)];
                }

                if (tMaxX < tMaxY)
                {
                    gridX += stepX;
                    tMaxX += tDeltaX;
                }
                else
                {
                    gridY += stepY;
                    tMaxY += tDeltaY;
                }

                if (gridX < 0 || gridY < 0 ||
                    gridX >= map.Setting.MapWidth * tileSize ||
                    gridY >= map.Setting.MapHeight * tileSize)
                {
                    return new List<Obstacle>();
                }
            }
        }
    }
}
