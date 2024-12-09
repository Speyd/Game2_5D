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
        private static bool IsRayIntersectingWithSprite(SpriteObstacle sprite)
        {
            //const float epsilon = 1e-6f;

            //float left = (float)(sprite.Left);// - (Screen.Setting.Tile * (SpriteObstacle.percentScaleMult * Screen.MultWidth) / 100));
            //float right = (float)(sprite.Right);// - (Screen.Setting.Tile * (SpriteObstacle.percentScaleMult * Screen.MultWidth) / 100));
            //float top = (float)(sprite.Top);// - (Screen.Setting.Tile * (SpriteObstacle.percentScaleMult * Screen.MultWidth) / 100));
            //float bottom = (float)(sprite.Bottom);// - (Screen.Setting.Tile * (SpriteObstacle.percentScaleMult * Screen.MultWidth) / 100));

            float currentRayX = startX + tMaxY * dx;
            float currentRayY = startY + tMaxX * dy;

            Console.WriteLine($"Ray Position: X = {currentRayX}, Y = {currentRayY}");
            Console.WriteLine($"sprite Position: X = {sprite.X}, Y = {sprite.Y}");

            bool isCollidingX = currentRayX > sprite.Left - 50 && currentRayX < sprite.Right - 50;
            bool isCollidingY = currentRayY > sprite.Top - 50 && currentRayY < sprite.Bottom - 50;
            Console.WriteLine($"isCollidingX: {isCollidingX}, isCollidingY: {isCollidingY}");

            return isCollidingX && isCollidingY;
        }
        private static bool CheckLumbago(List<Obstacle> obstacles)
        {
            foreach(var obstacle in obstacles)
            {
                if (obstacle is IWall)
                    return true;
                else if(obstacle is SpriteObstacle sprite)
                {
                    Console.WriteLine("1");
                    return IsRayIntersectingWithSprite(sprite) == false;
                }
            }
            return true;
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

            //if (map.ObstaclesWithoutNull.ContainsKey((gridX, gridY)))
            //{
            //    if (CheckLumbago(map.ObstaclesWithoutNull[(gridX, gridY)]))
            //        return map.Obstacles[(gridX, gridY)];
            //}

            while (true)
            {

                if (map.ObstaclesWithoutNull.ContainsKey((gridX, gridY)))
                {
                    if (CheckLumbago(map.ObstaclesWithoutNull[(gridX, gridY)]))
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
