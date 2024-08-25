using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityLib;
using MapLib;
using RenderLib.LogicRender;

namespace ScreenLib.RenderLib
{

    internal class Render
    {
        public Entity entity { get; init; }
        public Map map { get; init; }
        private readonly LogicRender logicRender;

        public Render(Map map, Entity entity)
        {
            this.entity = entity;
            this.map = map;

            //logicRender = new LogicRender(entity, map);
        }

        public void render()
        {


            int ceiling = (int)(screen.ScreenHeight / 2d - screen.ScreenHeight / distanceWall);
            int floor = screen.ScreenHeight - ceiling;

            char blockChar;

            if (distanceWall <= Depth / 4d)
                blockChar = '\u2588';
            else if (distanceWall <= Depth / 3d)
                blockChar = '\u2593';
            else if (distanceWall <= Depth / 2d)
                blockChar = '\u2592';
            else if (distanceWall < Depth)
                blockChar = '\u2591';
            else
                blockChar = ' ';

            for (int y = 0; y < screen.ScreenHeight; y++)
            {
                if (y <= ceiling)
                {
                    screen.ScreenChr[y * screen.ScreenWidth + x] = ' ';
                }
                else if (y > ceiling && y <= floor)
                {
                    screen.ScreenChr[y * screen.ScreenWidth + x] = blockChar;
                }
                else
                {
                    screen.ScreenChr[y * screen.ScreenWidth + x] = '.';
                }
            }
        }

    }
}
