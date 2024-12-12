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
using Render;


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
       
        private static (bool, Obstacle?) CheckingTouchingOfList(List<Obstacle> obstacles, Entity entity)
        {
            foreach(var obstacle in obstacles)
            {
                if (obstacle is IRayPassability rayPassability)
                {
                    float currentRayX = startX + tMaxY * dx;
                    float currentRayY = startY + tMaxX * dy;

                    if (rayPassability.IsRayTouchesObject(entity, currentRayX, currentRayY))
                        return (true, obstacle);
                }
                else
                    return (true, obstacle);
            }
            return (false, null);
        }
        public static Obstacle? RaycastFun(Map map, Entity entity)
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
                    var obstInfo = CheckingTouchingOfList(map.ObstaclesWithoutNull[(gridX, gridY)], entity);

                    if(obstInfo.Item1 && obstInfo.Item2 is not null)
                        return obstInfo.Item2;
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
                    return null;
                }
            }
        }
    }
}
