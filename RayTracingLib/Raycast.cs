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
using ObstacleLib.SpriteLib;
using static OpenTK.Graphics.OpenGL.GL;
using static System.Runtime.InteropServices.JavaScript.JSType;
using HitBoxLib.PositionObject;
using System.Numerics;
using HitBoxLib;


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
        private static void DetailedSearchInCell(List<Obstacle> colisionObstacle, List<Obstacle> obstacles, Entity entity)
        {
            double startX = entity.X.Axis;
            double startY = entity.Y.Axis;

            foreach (var obstacle in obstacles)
            {
                float minX = (float)(obstacle.HitBox.MainHitBox[CoordinatePlane.X, SideSize.Smaller]?.Side ?? 0);
                float maxX = (float)(obstacle.HitBox.MainHitBox[CoordinatePlane.X, SideSize.Larger]?.Side ?? 0);
                float minY = (float)(obstacle.HitBox.MainHitBox[CoordinatePlane.Y, SideSize.Smaller]?.Side ?? 0);
                float maxY = (float)(obstacle.HitBox.MainHitBox[CoordinatePlane.Y, SideSize.Larger]?.Side ?? 0);

                List<double> tValues = new();

                if (dx != 0)
                {
                    double t1 = (minX - startX) / dx;
                    double t2 = (maxX - startX) / dx;

                    tValues.Add(t1);
                    tValues.Add(t2);
                }

                if (dy != 0)
                {
                    double t3 = (minY - startY) / dy;
                    double t4 = (maxY - startY) / dy;

                    tValues.Add(t3);
                    tValues.Add(t4);
                }
       
                foreach (var tValue in tValues)
                {
                    if (tValue < 0) continue;

                    double xInter = startX + tValue * dx;
                    double yInter = startY + tValue * dy;

                    bool X = xInter >= minX && xInter <= maxX;
                    bool Y = yInter >= minY && yInter <= maxY;

                    if (X && Y)
                    {
                        colisionObstacle.Add(obstacle);
                        break;
                    }
                }
            }
        }

        public static Obstacle? GetFirstTouchedObject(List<Obstacle> colisionObstacle, Entity entity)
        {
            Obstacle? nearestObstacle = null;
            double nearestDistance = double.MaxValue;

            double entityX = entity.X.Axis;
            double entityY = entity.Y.Axis;
            double dirX = entity.Direction.X;
            double dirY = entity.Direction.Y;

            foreach (var obstacle in colisionObstacle)
            {
                float minX = (float)(obstacle.HitBox.MainHitBox[CoordinatePlane.X, SideSize.Smaller]?.Side ?? 0);
                float maxX = (float)(obstacle.HitBox.MainHitBox[CoordinatePlane.X, SideSize.Larger]?.Side ?? 0);
                float minY = (float)(obstacle.HitBox.MainHitBox[CoordinatePlane.Y, SideSize.Smaller]?.Side ?? 0);
                float maxY = (float)(obstacle.HitBox.MainHitBox[CoordinatePlane.Y, SideSize.Larger]?.Side ?? 0);

                List<double> tValues = new();

                if (dirX != 0)
                {
                    double t1 = (minX - entityX) / dirX;
                    double t2 = (maxX - entityX) / dirX;
                    tValues.Add(t1);
                    tValues.Add(t2);
                }
                if (dirY != 0)
                {
                    double t3 = (minY - entityY) / dirY;
                    double t4 = (maxY - entityY) / dirY;
                    tValues.Add(t3);
                    tValues.Add(t4);
                }

                foreach (var t in tValues)
                {
                    if (t < 0) continue;

                    double interX = entityX + t * dirX;
                    double interY = entityY + t * dirY;


                    bool insideX = interX >= minX && interX <= maxX;
                    bool insideY = interY >= minY && interY <= maxY;

                    if (insideX && insideY)
                    {
                        if (t < nearestDistance)
                        {
                            bool yea = true;
                            if (obstacle is SpriteObstacle o)
                                yea = o.IsRayTouchesObject(entity, (float)interX, (float)interY);
                            if (yea == true)
                            {
                                nearestDistance = t;
                                nearestObstacle = obstacle;
                            }
                        }
                    }
                }
            }

            return nearestObstacle;
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
            int scanRadius = 2;
            while (true)
            {
                List<Obstacle> colisionObstacle = new();

                for (int radius = 0; radius <= scanRadius; radius++)
                {
                    for (int xOffset = -radius; xOffset <= radius; xOffset++)
                    {
                        for (int yOffset = -radius; yOffset <= radius; yOffset++)
                        {
                            var scanX = gridX + xOffset * tileSize;
                            var scanY = gridY + yOffset * tileSize;

                            if (map.Obstacles.ContainsKey((scanX, scanY)) && !ignoreCoo.Contains((scanX, scanY)))
                                DetailedSearchInCell(colisionObstacle, map.Obstacles[(scanX, scanY)], entity);
                        }
                    }
                }

                Obstacle? findObstacle = GetFirstTouchedObject(colisionObstacle, entity);

                if (findObstacle != null)
                    return findObstacle;

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
