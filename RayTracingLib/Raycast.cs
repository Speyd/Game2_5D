using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityLib;
using MapLib;
using ScreenLib;
using Render.InterfaceRender;
using SFML.Graphics;
using ObstacleLib;
using static SFML.Window.Mouse;
using Render.RenderInterface;
using System.Reflection.Metadata;
using EntityLib.Player;
using SFML.Window;


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

        private static float percentOfMainStep = 0.01f;
        private static (bool, Obstacle?) CheckingTouchingOfList(List<Obstacle> obstacles, Entity entity, float currentRayX, float currentRayY)
        {
            foreach (var obstacle in obstacles)
            {

                if (obstacle is IRayPassability rayPassability)
                {
                    if (rayPassability.IsRayTouchesObject(entity, currentRayX, currentRayY))
                        return (true, obstacle);
                }
                else
                    return (true, obstacle);
            }
            return (false, null);
        }

        private static Obstacle? GetNearestObject(Entity entity, List<Obstacle> nonRayPassable)
        {
            (double, Obstacle?) nearObj = (-1, null);

            foreach(var obstacle in nonRayPassable)
            {
                double X = entity.X - obstacle.X;
                double Y = entity.Y - obstacle.Y;

                double dist = Math.Sqrt(X * X + Y * Y);

                if (nearObj.Item1 == -1 || nearObj.Item1 > dist)
                {
                    nearObj = (dist, obstacle);
                }
            }

            return nearObj.Item2;
        }

        private static (List<Obstacle> rayPassable, List<Obstacle> nonRayPassable) SplitObstaclesByRayPassability(List<Obstacle> obstacles)
        {
            var rayPassable = obstacles.Where(o => o is IRayPassability).ToList();
            var nonRayPassable = obstacles.Where(o => o is not IRayPassability).ToList();
            return (rayPassable, nonRayPassable);
        }

        private static Obstacle? DetailedSearchInCell(List<Obstacle> obstacles, Entity entity, int cellX, int cellY, float baseStep)
        {
            float step = baseStep * percentOfMainStep;
            int subTileSize = (int)(baseStep * percentOfMainStep);

            float localStartX = startX + dx * (tMaxX < tMaxY ? tMaxX : tMaxY);
            float localStartY = startY + dy * (tMaxX < tMaxY ? tMaxX : tMaxY);


            float localDeltaX = Math.Abs(subTileSize / dx);
            float localDeltaY = Math.Abs(subTileSize / dy);

            float localGridX = localStartX;
            float localGridY = localStartY;

            float localStepX = dx > 0 ? -step : step;
            float localStepY = dy > 0 ? -step : step;


            float localMaxX = dx > 0 ? (localStartX + subTileSize - localStartX) / dx : (localStartX - localStartX) / -dx;
            float localMaxY = dy > 0 ? (localStartY + subTileSize - localStartY) / dy : (localStartY - localStartY) / -dy;


            var (rayPassable, nonRayPassable) = SplitObstaclesByRayPassability(obstacles);
            if (rayPassable.Count == 0)
            {
                return nonRayPassable.Count == 1 ? 
                    nonRayPassable.FirstOrDefault() :
                    GetNearestObject(entity, nonRayPassable);
            }


            while (true)
            {
                var result = CheckingTouchingOfList(rayPassable, entity, localGridX, localGridY);
                if (result.Item1 && result.Item2 is not null)
                    return result.Item2;
                

                if (localMaxX < localMaxY)
                {
                    localGridX += localStepX;
                    localMaxX += localDeltaX;
                }
                else
                {
                    localGridY += localStepY;
                    localMaxY += localDeltaY;
                }

                if (localGridX <= cellX - Screen.Setting.Tile || localGridX >= cellX + Screen.Setting.Tile ||
                    localGridY <= cellY - Screen.Setting.Tile || localGridY >= cellY + Screen.Setting.Tile)
                {
                    break;
                }
            }

            return null;
        }
        public static Obstacle? RaycastFun(Map map, Entity entity)
        {
            float epsilon = 0.000001f;

            dx = (float)Math.Cos(entity.Angle);
            dy = (float)Math.Sin(entity.Angle);

            if (Math.Abs(dx) < epsilon) dx = dx < 0 ? -epsilon : epsilon;
            if (Math.Abs(dy) < epsilon) dy = dy < 0 ? -epsilon : epsilon;

            int tileSize = Screen.Setting.Tile;

            startX = (float)entity.X;
            startY = (float)entity.Y;

            int gridX = (int)(startX / tileSize) * tileSize;
            int gridY = (int)(startY / tileSize) * tileSize;

            int stepX = dx > 0 ? tileSize : -tileSize;
            int stepY = dy > 0 ? tileSize : -tileSize;

            float tDeltaX = Math.Abs(Screen.Setting.Tile / dx);
            float tDeltaY = Math.Abs(Screen.Setting.Tile / dy);

            tMaxX = dx > 0 ? (gridX + tileSize - startX) / dx : (startX - gridX) / -dx;
            tMaxY = dy > 0 ? (gridY + tileSize - startY) / dy : (startY - gridY) / -dy;

            while (true)
            {
                if (map.ExistingObstacles.ContainsKey((gridX, gridY)))
                {               
                    var detailedResult = DetailedSearchInCell(map.ExistingObstacles[(gridX, gridY)], entity, gridX, gridY, tileSize);

                    if (detailedResult is not null)
                        return detailedResult;
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
