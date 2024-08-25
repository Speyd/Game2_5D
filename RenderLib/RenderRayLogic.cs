using EntityLib;
using MapLib;
using ScreenLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenderLib.LogicRender
{
    internal class RenderRayLogic(Entity entity, Map map, Screen screen, bool showLineSight)
    {
        public double findDegreeRay(int x)
        {
            double degreeRay = entity.EntityA + (entity.EntityFov / 2) - ((x * entity.EntityFov) / screen.ScreenWidth);
            return degreeRay;
        }

        private void rayCollision(int tempX, int tempY, ref bool hitWall, ref double distanceToWall)
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
                if (testSell == map.block)
                    hitWall = true;
                else if(showLineSight == true)
                    map.MapStr[tempY * map.MapWidth + tempX] = map.lineSight;
            }
        }

        private void rayLaunch(double rayX, double rayY, ref double distanceToWall)
        {
            distanceToWall = 0;
            bool hitWall = false;

            while (!hitWall && distanceToWall < entity.Depth)
            {
                distanceToWall += 0.1;
                int tempX = (int)(entity.EntityX + rayX * distanceToWall);
                int tempY = (int)(entity.EntityY + rayY * distanceToWall);

                rayCollision(tempX, tempY, ref hitWall, ref distanceToWall);
            }
        }

        public void render(int x, ref double distanceToWall)
        {
            double degreeRay = findDegreeRay(x);
            double rayX = Math.Sin(degreeRay);
            double rayY = Math.Cos(degreeRay);

            rayLaunch(rayX, rayY, ref distanceToWall);
        }
    }
}
