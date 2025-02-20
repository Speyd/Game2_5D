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
        private static Obstacle? DetailedSearchInCell(List<Obstacle> obstacles, Entity entity, float baseStep)
        {
            // float distance = (float)Math.Sqrt((gridX - entity.X.Axis) * (gridX - entity.X.Axis) + (gridY - entity.Y.Axis) * (gridY - entity.Y.Axis));
            // float stepSize = baseStep * percentOfMainStep;

            // float entryDist = Math.Min(tMaxX, tMaxY);
            //// float length = (float)Math.Sqrt(dx * dx + dy * dy);


            // float checkX = gridX;
            // float checkY = gridY;


            // double dx = entity.X.Axis - entity.X.Axis;
            // double dy = entity.Y.Axis - entity.Y.Axis;
            // double spriteAngle = Math.Atan2(dy, dx);
            // double angleDifference = spriteAngle - entity.Angle;

            // //if (angleDifference > Math.PI)
            // //    angleDifference -= 2 * Math.PI;
            // //if (angleDifference < -Math.PI)
            // //    angleDifference += 2 * Math.PI;

            // float stepX = MathF.Cos((float)angleDifference) * stepSize;
            // float stepY = MathF.Sin((float)angleDifference) * stepSize;


            // var (rayPassable, nonRayPassable) = SplitObstaclesByRayPassability(obstacles);
            // if (rayPassable.Count == 0)
            // {
            //     return nonRayPassable.Count == 1 ?
            //         nonRayPassable.FirstOrDefault() :
            //         GetNearestObject(entity, nonRayPassable);
            // }
            // int i = 0;
            // while (checkX > gridX - 100 && checkX < gridX + 100 &&
            //        checkY > checkY - 100 && checkY < checkY + 100)
            // {
            //     var result = CheckingTouchingOfList(rayPassable, entity, x, y);
            //     if (result.Item1 && result.Item2 is not null)
            //         return result.Item2;

            //     checkX += stepX;
            //     checkY += stepY;

            //     //Console.WriteLine($"i: {i++}");
            //     //if (Math.Abs(stepX) < epsilon && Math.Abs(stepY) < epsilon)
            //     //    break;
            //     i++;
            // }


            double startX = entity.X.Axis;
            double startY = entity.Y.Axis;
            var (rayPassable, nonRayPassable) = SplitObstaclesByRayPassability(obstacles);
            if (rayPassable.Count == 0)
            {
                return nonRayPassable.Count == 1 ?
                    nonRayPassable.FirstOrDefault() :
                    GetNearestObject(entity, nonRayPassable);
            }

            //float Left = (float)(rayPassable.FirstOrDefault()?.HitBox.SegmentedHitbox.FirstOrDefault()?[HitBoxSideType.Left]?.Side ?? 0);
            //float Right = (float)(rayPassable.FirstOrDefault()?.HitBox.SegmentedHitbox.FirstOrDefault()?[HitBoxSideType.Right]?.Side ?? 0);
            //float Top = (float)(rayPassable.FirstOrDefault()?.HitBox.SegmentedHitbox.FirstOrDefault()?[HitBoxSideType.Top]?.Side ?? 0);
            //float Bottom = (float)(rayPassable.FirstOrDefault()?.HitBox.SegmentedHitbox.FirstOrDefault()?[HitBoxSideType.Bottom]?.Side ?? 0);
            //float Up = (float)(rayPassable.FirstOrDefault()?.HitBox.SegmentedHitbox.FirstOrDefault()?[HitBoxSideType.Up]?.Side ?? 0);
            //float Down = (float)(rayPassable.FirstOrDefault()?.HitBox.SegmentedHitbox.FirstOrDefault()?[HitBoxSideType.Down]?.Side ?? 0);
            float Left = (float)(rayPassable.FirstOrDefault()?.HitBox?[HitBoxSideType.Left]?.Side ?? 0);
            float Right = (float)(rayPassable.FirstOrDefault()?.HitBox?[HitBoxSideType.Right]?.Side ?? 0);
            float Top = (float)(rayPassable.FirstOrDefault()?.HitBox?[HitBoxSideType.Top]?.Side ?? 0);
            float Bottom = (float)(rayPassable.FirstOrDefault()?.HitBox?[HitBoxSideType.Bottom]?.Side ?? 0);
            float Up = (float)(rayPassable.FirstOrDefault()?.HitBox?[HitBoxSideType.Up]?.Side ?? 0);
            float Down = (float)(rayPassable.FirstOrDefault()?.HitBox?[HitBoxSideType.Down]?.Side ?? 0);
            var tValues = new System.Collections.Generic.List<(double, string)>();

            // Пересечение с вертикальными границами (x = xMin и x = xMax)
            if (dx != 0)
            {
                double t1 = (Left - startX) / dx;
                double t2 = (Right - startX) / dx;
                //if (t1 > 0)
                    tValues.Add((t1, "xMin"));
                //if (t2 > 0)
                    tValues.Add((t2, "xMax"));
            }

            if (dy != 0)
            {
                double t3 = (Top - startY) / dy;
                double t4 = (Bottom - startY) / dy;
               // if (t3 > 0)
                    tValues.Add((t3, "yMin"));
                //if (t4 > 0)
                    tValues.Add((t4, "yMax"));


            }

            tValues.Sort((a, b) => a.Item1.CompareTo(b.Item1));
            foreach (var tValue in tValues)
            {
                double t = tValue.Item1;
                if (t < 0) continue;

                double xInter = startX + t * dx;
                double yInter = startY + t * dy;

                //float mult = Math.Abs(dx) > Math.Abs(dy) && dx > 0? 0.5f : Math.Abs(dx) < Math.Abs(dy) && dx > 0? - 0.01f: 0.01f;
                //mult = dx < 0 && Math.Abs(dx) > entity.HalfFov ? 0.01f : mult;
                double marginX = 0; // (Right - Left) * (-dx / 10); // 5% от ширины
                double marginY = 0;//(Bottom - Top) * 0.05; // 5% от высоты

                bool X = xInter >= Left - marginX && xInter <= Right + marginX;
                bool Y = yInter >= Top - marginY && yInter <= Bottom + marginY;

                //Console.WriteLine($"dx: {dx}");
                //Console.WriteLine($"dy: {dy}");
                //Console.WriteLine($"entity.HalfFov: {entity.HalfFov}");


                if (X && Y)
                {
                    //Console.WriteLine($"Пересечение найдено: X: {xInter}, Y: {yInter}");
                    return rayPassable.FirstOrDefault();
                }
            }

            return null;
        }

        private static (bool, float, float) IsRayIntersectsBox(Entity entity, List<Obstacle> obstacles, float x, float y)
        {
            foreach (var obstacle in obstacles)
            {
                float minX = (float)(obstacle.HitBox.MainHitBox.Body[HitBoxLib.HitBoxSideType.Left]?.Side ?? 0);
                float maxX = (float)(obstacle.HitBox.MainHitBox.Body[HitBoxLib.HitBoxSideType.Right]?.Side ?? 0);
                float minY = (float)(obstacle.HitBox.MainHitBox.Body[HitBoxLib.HitBoxSideType.Top]?.Side ?? 0);
                float maxY = (float)(obstacle.HitBox.MainHitBox.Body[HitBoxLib.HitBoxSideType.Bottom]?.Side ?? 0);

                float rayX = x;
                float rayY = y;

                float dirX = (float)entity.Direction.X;
                float dirY = (float)entity.Direction.Y;

                // 1. Нормализация направления луча
                float length = MathF.Sqrt(dirX * dirX + dirY * dirY);
                if (length > 1e-6) // Проверяем, чтобы не делить на 0
                {
                    dirX /= length;
                    dirY /= length;
                }

                // 2. Используем epsilon для предотвращения деления на 0
                float epsilon = 1e-6f;
                float invDirX = 1.0f / (dirX + (dirX == 0 ? epsilon : 0));
                float invDirY = 1.0f / (dirY + (dirY == 0 ? epsilon : 0));

                // 3. Вычисление tMin и tMax
                float tMinX = (minX - rayX) * invDirX;
                float tMaxX = (maxX - rayX) * invDirX;
                if (tMinX > tMaxX) (tMinX, tMaxX) = (tMaxX, tMinX);

                float tMinY = (minY - rayY) * invDirY;
                float tMaxY = (maxY - rayY) * invDirY;
                if (tMinY > tMaxY) (tMinY, tMaxY) = (tMaxY, tMinY);

                float tEnter = Math.Max(tMinX, tMinY);
                float tExit = Math.Min(tMaxX, tMaxY);

                // 4. Проверяем попадание луча в объект
                if (tEnter <= tExit && tExit >= 0 || obstacle is SpriteObstacle)
                {
                    float intersectionX = rayX + tEnter * dirX;
                    float intersectionY = rayY + tEnter * dirY;

                    return (true, intersectionX, intersectionY);
                }
            }

            return (false, -1, -1);
        }

        private static (float, float) GetRayGridIntersection(float startX, float startY, float dirX, float dirY, int gridX, int gridY, int tileSize)
        {
            float tX = (dirX > 0) ? (gridX + tileSize - startX) / (dirX + epsilon) : (gridX - startX) / (dirX - epsilon);
            float tY = (dirY > 0) ? (gridY + tileSize - startY) / (dirY + epsilon) : (gridY - startY) / (dirY - epsilon);


            float tMin = Math.Min(tX, tY);  // Берем ближайшее пересечение
            float hitX = startX + tMin * dirX;
            float hitY = startY + tMin * dirY;

            return (hitX, hitY);
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
                //if (map.Obstacles.ContainsKey((gridX, gridY)) && !ignoreCoo.Contains((gridX, gridY)))
                //{
                //    //Console.WriteLine($"gridX: {gridX}, gridY: {gridY}, tMaxX: {tMaxX}, tMaxY: {tMaxY}");
                //    //Console.WriteLine($"dx: {dx}, dy: {dy}, stepX: {stepX}, stepY: {stepY}");
                //    (float hitX, float hitY) = GetRayGridIntersection(startX, startY, dx, dy, gridX, gridY, tileSize);

                //    var detailedResult = DetailedSearchInCell(map.Obstacles[(gridX, gridY)], entity, tileSize, hitX, hitY);

                //    if (detailedResult is not null)
                //        return detailedResult;
                //}
                //for (int xOffset = -scanRadius; xOffset <= scanRadius; xOffset ++)
                //{
                //    for (int yOffset = -scanRadius; yOffset <= scanRadius; yOffset ++)
                //    {
                //        var scanX = gridX + xOffset * tileSize;
                //        var scanY = gridY + yOffset * tileSize;

                //        if (ignoreCoo.Contains((scanX, scanY)))
                //            continue;

                //        if (map.Obstacles.ContainsKey((scanX, scanY)))
                //        {
                //            (bool, float, float) gg = IsRayIntersectsBox(entity, map.Obstacles[(scanX, scanY)]);
                //            if (gg.Item1)
                //            {
                //                var (rayPassable, nonRayPassable) = SplitObstaclesByRayPassability(map.Obstacles[(scanX, scanY)]);
                //                if (rayPassable.Count == 0)
                //                {
                //                    return nonRayPassable.Count == 1 ?
                //                        nonRayPassable.FirstOrDefault() :
                //                        GetNearestObject(entity, nonRayPassable);
                //                }
                //                var detailedResult = CheckingTouchingOfList(rayPassable, entity, gg.Item2, gg.Item3);// DetailedSearchInCell(map.Obstacles[(scanX, scanY)], entity, tileSize);

                //                if (detailedResult.Item2 != null)
                //                    return detailedResult.Item2;
                //            }
                //        }
                //    }
                //}
                for (int radius = 0; radius <= scanRadius; radius++)
                {
                    for (int xOffset = -radius; xOffset <= radius; xOffset++)
                    {
                        for (int yOffset = -radius; yOffset <= radius; yOffset++)
                        {
                            // Пропускаем точки, которые уже проверены в предыдущих радиусах
                            if (Math.Abs(xOffset) < radius && Math.Abs(yOffset) < radius)
                                continue;

                            var scanX = gridX + xOffset * tileSize;
                            var scanY = gridY + yOffset * tileSize;

                            if (map.Obstacles.ContainsKey((scanX, scanY)) && !ignoreCoo.Contains((scanX, scanY)))
                            {

                                var detailedResult = DetailedSearchInCell(map.Obstacles[(scanX, scanY)], entity, tileSize);

                                if (detailedResult is not null)
                                {
                                    if (detailedResult is SpriteObstacle p)
                                        p.IsRayTouchesObject(entity, 1, 1);
                                    return detailedResult;
                                }
                                Console.WriteLine($"scanX: {scanX}");
                                Console.WriteLine($"scanY: {scanY}");
                            }
                        }
                    }
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
