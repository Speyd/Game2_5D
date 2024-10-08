﻿using EntityLib;
using MapLib;
using ScreenLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RenderLib.LogicRender
{
    internal class RenderRayLogic(Entity entity, Map map, Screen screen, bool showLineSight)
    {
        public static readonly double boundAngle = 0.03;

        public static double findDegreeRay(int x, Entity entity, Screen screen)
        {
            double degreeRay = entity.EntityA + (entity.EntityFov / 2) - ((x * entity.EntityFov) / screen.ScreenWidth);
            return degreeRay;
        }

        private void rayCollision(int tempX, int tempY,
            double rayX, double rayY,
            ref bool isBound, ref bool hitWall,
            ref double distanceToWall)
        {
            if (tempX < 0 || tempX >= entity.Depth + entity.EntityX ||
                    tempY < 0 || tempY >= entity.Depth + entity.EntityY)
            {
                hitWall = true;
                distanceToWall = entity.Depth;
            }
            else
            {
                char testSell = map.MapStr[tempY * map.MapWidth + tempX];
                bool checkPassability = map.objects[testSell]?.Passability ?? false;

                if (checkPassability == false)
                {                   
                    hitWall = true;
                    drawingEdges(ref isBound, tempX, tempY, rayX, rayY, distanceToWall);
                }
                else if (showLineSight == true)
                    map.MapStr[tempY * map.MapWidth + tempX] = map.lineSight.Symbol;
            }
        }

        private void drawingEdges(ref bool isBound, 
            double tempX, double tempY,
            double rayX, double rayY,
            double distanceToWall)
        {
           var sidesObject = new List<(double module, double cos)>();

            for (int xSide = 0; xSide < 2; xSide++)
            {
                for (int ySide = 0; ySide < 2; ySide++)
                {
                    double xRay = tempX + xSide - entity.EntityX;
                    double yRay = tempY + ySide - entity.EntityY;

                    double moduleRay = Math.Sqrt(xRay * xRay + yRay * yRay);
                    double corner = rayX * xRay / moduleRay + rayY * yRay / moduleRay;

                    sidesObject.Add((moduleRay, corner));
                }
            }

            sidesObject = sidesObject.OrderBy(v => v.module).ToList();

            double boundAngle = 0.03 / distanceToWall;

            if (Math.Acos(sidesObject[0].cos) < boundAngle ||
                Math.Acos(sidesObject[1].cos) < boundAngle)
            {
                isBound = true;
            }
        }

        private void determiningCoordinatesRay(
            in double rayX, in double rayY,
            ref int tempX, ref int tempY, 
            ref double distanceToWall)
        {
            distanceToWall += 0.1;
            tempX = (int)(entity.EntityX + rayX * distanceToWall);
            tempY = (int)(entity.EntityY + rayY * distanceToWall);
        }
        private void tracingAllObjects(ref Dictionary<double, MapLib.Object?> obj,
            in double rayX, in double rayY,
            int tempX, int tempY,
            double distanceToWall)
        {
            obj.Add(distanceToWall, map.getObject(tempX, tempY));
            double tempDistanceToWall = distanceToWall;

            while (tempDistanceToWall < entity.Depth)
            {
                determiningCoordinatesRay(rayX, rayY, ref tempX, ref tempY, ref tempDistanceToWall);

                if (tempX < 0 || tempX >= entity.Depth + entity.EntityX ||
                    tempY < 0 || tempY >= entity.Depth + entity.EntityY)
                {
                    continue;
                }
                else
                {
                    if (tempY * map.MapWidth + tempX >= map.MapStr.Length || tempY * map.MapWidth + tempX < 0)
                        continue;

                    char testSell = map.MapStr[tempY * map.MapWidth + tempX];
                    bool checkPassability = map.objects[testSell]?.Passability ?? false;


                    if (checkPassability == false)
                    {
                        if (!obj.ContainsKey(tempDistanceToWall) && tempX >= 0 && tempX < map.MapHeight &&
                            tempY >= 0 && tempY < map.MapWidth)
                            obj.Add(tempDistanceToWall, map.getObject(tempX, tempY));
                    }
                }
            }
        }

        private void rayLaunch(ref Dictionary<double, MapLib.Object?> obj,
            double rayX, double rayY,
            ref bool isBound, ref double distanceToWall)
        {
            distanceToWall = 0;
            bool hitWall = false;
            int tempX = 0;
            int tempY = 0;

            while (!hitWall && distanceToWall < entity.Depth)
            {
                determiningCoordinatesRay(rayX, rayY, ref tempX, ref tempY, ref distanceToWall);
                rayCollision(tempX, tempY, rayX, rayY, ref isBound, ref hitWall, ref distanceToWall);            
            }

            if(hitWall == true)
                tracingAllObjects(ref obj, rayX, rayY, tempX, tempY, distanceToWall);
        }

        public void render(ref Dictionary<double, MapLib.Object?> obj, int x, ref bool isBound, ref double distanceToWall)
        {
            double degreeRay = findDegreeRay(x, entity, screen);

            double rayX = Math.Sin(degreeRay);
            double rayY = Math.Cos(degreeRay);

            isBound = false;

            rayLaunch(ref obj, rayX, rayY, ref isBound, ref distanceToWall);
        }
    }
}
