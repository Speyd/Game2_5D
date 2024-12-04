using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapLib.Obstacles;
using EntityLib;
using MapLib;
using ScreenLib;


namespace RayTracingLib
{
    public static class Raycast
    {
        public static Obstacle? RaycastFun(Map map, Entity entity)
        {
            float dx = (float)Math.Cos(entity.Angle);
            float dy = (float)Math.Sin(entity.Angle);

            int tileSize = Screen.Setting.Tile;

            float startX = (float)entity.X;
            float startY = (float)entity.Y;

            int gridX = (int)(startX / tileSize) * tileSize;
            int gridY = (int)(startY / tileSize) * tileSize;

            int stepX = dx > 0 ? tileSize : -tileSize;
            int stepY = dy > 0 ? tileSize : -tileSize;

            float tDeltaX = Math.Abs(tileSize / dx);
            float tDeltaY = Math.Abs(tileSize / dy);


            float tMaxX = dx > 0 ? (gridX + tileSize - startX) / dx : (startX - gridX) / -dx;
            float tMaxY = dy > 0 ? (gridY + tileSize - startY) / dy : (startY - gridY) / -dy;

            while (true)
            {

                if (map.Obstacles.ContainsKey((gridX, gridY)))
                    return map.Obstacles[(gridX, gridY)];

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
