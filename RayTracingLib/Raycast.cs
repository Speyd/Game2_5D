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

        private static int gridX;
        private static int gridY;

        private const float epsilon = 0.000001f;
        private const float percentOfMainStep = 0.01f;

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
                double X = entity.X.Axis - obstacle.X.Axis;
                double Y = entity.Y.Axis - obstacle.Y.Axis;

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
        private static Obstacle? DetailedSearchInCell(List<Obstacle> obstacles, Entity entity, float baseStep)
        {
            float distance = (float)Math.Sqrt((gridX - entity.X.Axis) * (gridX - entity.X.Axis) + (gridY - entity.Y.Axis) * (gridY - entity.Y.Axis));
            float stepSize = baseStep * percentOfMainStep;

            float entryDist = Math.Min(tMaxX, tMaxY);
            float checkX = startX;
            float checkY = startY;

            float length = (float)Math.Sqrt(dx * dx + dy * dy);
            float stepX = (dx / length) * stepSize;
            float stepY = (dy / length) * stepSize;

            var (rayPassable, nonRayPassable) = SplitObstaclesByRayPassability(obstacles);
            if (rayPassable.Count == 0)
            {
                return nonRayPassable.Count == 1 ?
                    nonRayPassable.FirstOrDefault() :
                    GetNearestObject(entity, nonRayPassable);
            }

            while (checkX >= gridX - distance && checkX < gridX + distance &&
                   checkY >= gridY - distance && checkY < gridY + distance)
            {
                var result = CheckingTouchingOfList(rayPassable, entity, checkX, checkY);
                if (result.Item1 && result.Item2 is not null)
                {
                    Console.WriteLine(result.Item1);
                    return result.Item2;
                }
                checkX += stepX;
                checkY += stepY;

                if (Math.Abs(stepX) < epsilon && Math.Abs(stepY) < epsilon)
                    break;
            }

            return null;
        }
        public static Obstacle? RaycastFun(Map map, Entity entity, List<(float, float)> ignoreCoo)
        {
            dx = entity.Direction.X;
            dy = entity.Direction.Y;

            if (Math.Abs(dx) < epsilon) dx = dx < 0 ? -epsilon : epsilon;
            if (Math.Abs(dy) < epsilon) dy = dy < 0 ? -epsilon : epsilon;

            int tileSize = Screen.Setting.Tile;

            startX = (float)entity.X.Axis;
            startY = (float)entity.Y.Axis;

            gridX = (int)(startX / tileSize) * tileSize;
            gridY = (int)(startY / tileSize) * tileSize;

            int stepX = dx > 0 ? tileSize : -tileSize;
            int stepY = dy > 0 ? tileSize : -tileSize;

            float tDeltaX = Math.Abs(Screen.Setting.Tile / dx);
            float tDeltaY = Math.Abs(Screen.Setting.Tile / dy);

            tMaxX = dx > 0 ? (gridX + tileSize - startX) / dx : (startX - gridX) / -dx;
            tMaxY = dy > 0 ? (gridY + tileSize - startY) / dy : (startY - gridY) / -dy;

            while (true)
            {
                if (map.Obstacles.ContainsKey((gridX, gridY)) && !ignoreCoo.Contains((gridX, gridY)))
                {               
                    var detailedResult = DetailedSearchInCell(map.Obstacles[(gridX, gridY)], entity, tileSize);

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
